using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PromptPlayground.Services
{
    public class PromptService
    {
        private readonly IKernel _kernel;

        public PromptService(IKernel kernel)
        {
            this._kernel = kernel;
        }

        public async Task<GenerateResult> RunAsync(string prompt, PromptTemplateConfig config, SKContext context, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            var func = _kernel.CreateSemanticFunction(prompt, config);
            var result = await func.InvokeAsync(context);
            sw.Stop();
            return new GenerateResult()
            {
                Text = result.Result,
                Elapsed = sw.Elapsed,
            };
        }
    }
    public class GenerateResult
    {
        public string Text { get; set; } = string.Empty;
        public TimeSpan Elapsed { get; set; }
    }
}
