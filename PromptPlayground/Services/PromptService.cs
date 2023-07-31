using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using PromptPlayground.ViewModels.LLMConfigViewModels;
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

        public PromptService(ILLMConfigViewModel Model)
        {
            model = Model;
            _kernel = model.CreateKernel();
        }

        public async Task<GenerateResult> RunAsync(string prompt, PromptTemplateConfig config, SKContext context, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            var func = _kernel.CreateSemanticFunction(prompt, config);
            var result = await func.InvokeAsync(context);

            var usage = model.GetUsage(result.ModelResults.Last());

            sw.Stop();
            return new GenerateResult()
            {
                Text = result.Result,
                Elapsed = sw.Elapsed,
                Error = result.LastErrorDescription,
                TokenUsage = usage
            };
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
        public string TokenUsageText => TokenUsage!=null ? $"({TokenUsage.Total}/{TokenUsage.Prompt}/{TokenUsage.Completion})" : string.Empty;

        public bool HasError => !string.IsNullOrWhiteSpace(Error);
    }
    public class ResultTokenUsage
    {
        public int Total { get; set; }
        public int Prompt { get; set; }
        public int Completion { get; set; }
    }

}
