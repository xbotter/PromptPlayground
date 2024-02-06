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
        private readonly Kernel _kernel;
        private readonly ILLMConfigViewModel model;

        public PromptService(IConfigAttributesProvider provider)
        {
            this.model = provider.GetLLM() ?? throw new Exception("无法创建Kernel，请检查LLM配置");

            var builder = model.CreateKernelBuilder();


            _kernel = builder.Build();

            _kernel.ImportPluginFromType<TimePlugin>();

        }

        public async Task<GenerateResult> RunAsync(string prompt, PromptExecutionSettings? config, KernelArguments arguments, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            var func = _kernel.CreateFunctionFromPrompt(prompt, config);

            var result = await func.InvokeAsync(_kernel, arguments);
            sw.Stop();
            try
            {
                var usage = model.GetUsage(result);
                return new GenerateResult()
                {
                    Text = result.GetValue<string>() ?? "",
                    TokenUsage = usage,
                    Elapsed = sw.Elapsed,

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


}
