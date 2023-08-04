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
    public class GenerateResult
    {
        public string Text { get; set; } = string.Empty;
        public TimeSpan Elapsed { get; set; }
        public string ElapsedText => $"{Elapsed.TotalSeconds:0.000} s";
        public string Error { get; set; } = string.Empty;
        public ResultTokenUsage? TokenUsage { get; set; }
        public string TokenUsageText => TokenUsage != null ? $"{TokenUsage.Total}({TokenUsage.Prompt}|{TokenUsage.Completion})" : string.Empty;

        public bool HasError => !string.IsNullOrWhiteSpace(Error);
    }
    public class ResultTokenUsage
    {
        public int Total { get; set; }
        public int Prompt { get; set; }
        public int Completion { get; set; }
    }

}
