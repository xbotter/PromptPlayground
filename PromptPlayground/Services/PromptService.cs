using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.Skills.Core;
using PromptPlayground.ViewModels.ConfigViewModels;
using PromptPlayground.ViewModels.ConfigViewModels.Embedding;
using PromptPlayground.ViewModels.ConfigViewModels.LLM;
using PromptPlayground.ViewModels.ConfigViewModels.VectorDB;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromptPlayground.Services
{
    public class PromptService
    {
        private readonly IKernel _kernel;
        private readonly ILLMConfigViewModel model;
        private readonly IConfigAttributesProvider provider;

        public PromptService(IConfigAttributesProvider provider)
        {
            this.provider = provider;

            this.model = provider.GetLLM() ?? throw new Exception("无法创建Kernel，请检查LLM配置");

            var builder = model.CreateKernelBuilder();

            if (provider.GetEmbedding() is IEmbeddingConfigViewModel embedding)
            {
                embedding.RegisterEmbedding(builder);

                if (provider.GetVectorDb() is IVectorDbConfigViewModel vectorDb)
                {
                    vectorDb.RegisterMemory(builder);
                }
            }

            _kernel = builder.Build();

            if (_kernel.Memory is not null)
            {
                _kernel.ImportSkill(new TextMemorySkill(_kernel.Memory));
            }

            _kernel.ImportSkill(new TimeSkill());

        }

        public async Task<GenerateResult> RunAsync(string prompt, PromptTemplateConfig config, SKContext context, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            var func = _kernel.CreateSemanticFunction(prompt, config);

            if (provider.GetVectorDb() is IVectorDbConfigViewModel vectorDb)
            {
                context[TextMemorySkill.CollectionParam] = vectorDb.Collection;
                context[TextMemorySkill.LimitParam] = vectorDb.Limit.ToString();
                context[TextMemorySkill.RelevanceParam] = vectorDb.Relevance.ToString();
            }

            var result = await func.InvokeAsync(context);
            sw.Stop();
            if (!result.ErrorOccurred)
            {
                var usage = model.GetUsage(result.ModelResults.Last());

                return new GenerateResult()
                {
                    Text = result.Result,
                    Elapsed = sw.Elapsed,
                    Error = result.LastErrorDescription,
                    TokenUsage = usage
                };
            }
            else
            {
                return new GenerateResult()
                {
                    Text = result.Result,
                    Elapsed = sw.Elapsed,
                    Error = result.LastErrorDescription,
                };
            }
        }

        public SKContext CreateContext() => _kernel.CreateNewContext();
    }


}
