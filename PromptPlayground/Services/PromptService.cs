using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using PromptPlayground.ViewModels;
using PromptPlayground.ViewModels.ConfigViewModels;
using PromptPlayground.ViewModels.ConfigViewModels.LLM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PromptPlayground.Services
{
    public class PromptService
    {
        private readonly ILLMConfigViewModel _model;
        private readonly List<PluginViewModel> _plugins;

        public PromptService(IConfigAttributesProvider provider, List<PluginViewModel> plugins)
        {
            this._model = provider.GetLLM() ?? throw new Exception("无法创建Kernel，请检查LLM配置");
            this._plugins = plugins;
        }

        private Kernel Build()
        {

            var builder = _model.CreateKernelBuilder();

            var _kernel = builder.Build();

            foreach (var plugin in _plugins)
            {
                if (plugin.Folder != null && Directory.Exists(plugin.Folder))
                {
                    _kernel.ImportPluginFromPromptDirectory(plugin.Folder);
                }
                else
                {
                    foreach (var function in plugin.Functions)
                    {
                        if (!string.IsNullOrWhiteSpace(function.Folder))
                        {
                            _kernel.CreateFunctionFromPrompt(function.Prompt, function.PromptConfig.DefaultExecutionSettings, function.Name, function.PromptConfig.Description);
                        }
                    }
                }
            }

            _kernel.ImportPluginFromType<TimePlugin>();

            return _kernel;
        }

        public async IAsyncEnumerable<GenerateResult> RunStreamAsync(SemanticFunctionViewModel semanticFunction,
            KernelArguments arguments,
            [EnumeratorCancellation]
            CancellationToken cancellationToken = default)
        {
            var _kernel = Build();
            var promptFilter = new KernelFilter();
            _kernel.PromptFilters.Add(promptFilter);
            _kernel.FunctionFilters.Add(promptFilter);

            var sw = Stopwatch.StartNew();

            var func = _kernel.CreateFunctionFromPrompt(semanticFunction.Prompt, semanticFunction.PromptConfig.DefaultExecutionSettings);

            var results = func.InvokeStreamingAsync(_kernel, arguments, cancellationToken);

            var sb = new StringBuilder();
            await foreach (var result in results)
            {
                sb.Append(result.ToString());
                yield return new GenerateResult()
                {
                    Text = sb.ToString(),
                    Elapsed = sw.Elapsed,
                    PromptRendered = promptFilter.PromptRendered
                };
            }
            sw.Stop();
        }

        public async Task<GenerateResult> RunAsync(SemanticFunctionViewModel semanticFunction,
            KernelArguments arguments,
            CancellationToken cancellationToken = default)
        {
            var _kernel = Build();

            var promptFilter = new KernelFilter();
            _kernel.PromptFilters.Add(promptFilter);
            _kernel.FunctionFilters.Add(promptFilter);

            var sw = Stopwatch.StartNew();
            var func = _kernel.CreateFunctionFromPrompt(semanticFunction.Prompt, semanticFunction.PromptConfig.DefaultExecutionSettings);

            var result = await func.InvokeAsync(_kernel, arguments, cancellationToken);
            sw.Stop();
            try
            {
                var usage = _model.GetUsage(result);
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
