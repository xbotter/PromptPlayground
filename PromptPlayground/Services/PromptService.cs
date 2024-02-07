using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using PromptPlayground.ViewModels.ConfigViewModels;
using PromptPlayground.ViewModels.ConfigViewModels.LLM;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromptPlayground.Services
{
    public class PromptService
    {
        private readonly ILLMConfigViewModel model;

        public PromptService(IConfigAttributesProvider provider)
        {
            this.model = provider.GetLLM() ?? throw new Exception("无法创建Kernel，请检查LLM配置");
        }

        private Kernel Build()
        {

            var builder = model.CreateKernelBuilder();

            var _kernel = builder.Build();

            _kernel.ImportPluginFromType<TimePlugin>();


            return _kernel;
        }

        public async Task<GenerateResult> RunAsync(string prompt, PromptExecutionSettings? config, KernelArguments arguments, CancellationToken cancellationToken = default)
        {
            var _kernel = Build();
            var promptFilter = new KernelFilter();
            _kernel.PromptFilters.Add(promptFilter);
            _kernel.FunctionFilters.Add(promptFilter);

            var sw = Stopwatch.StartNew();
            var func = _kernel.CreateFunctionFromPrompt(prompt, config);

            var result = await func.InvokeAsync(_kernel, arguments, cancellationToken);
            sw.Stop();
            try
            {
                var usage = model.GetUsage(result);
                return new GenerateResult()
                {
                    Text = result.GetValue<string>() ?? "",
                    TokenUsage = usage,
                    Elapsed = sw.Elapsed,
                    PromptRendered = promptFilter.PromptRendered
                };
            }
            catch (KernelException ex)
            {
                return new GenerateResult()
                {
                    Text = ex.Message,
                    Elapsed = sw.Elapsed,
                    Error = ex.Message,
                };
            }
        }

        public KernelArguments CreateArguments() => new KernelArguments();
    }

    public class KernelFilter : IPromptFilter, IFunctionFilter
    {
        public string? PromptRendered { get; set; }
        public void OnFunctionInvoked(FunctionInvokedContext context)
        {

        }

        public void OnFunctionInvoking(FunctionInvokingContext context)
        {

        }

        public void OnPromptRendered(PromptRenderedContext context)
        {
            PromptRendered = context.RenderedPrompt;
        }

        public void OnPromptRendering(PromptRenderingContext context)
        {

        }
    }


}
