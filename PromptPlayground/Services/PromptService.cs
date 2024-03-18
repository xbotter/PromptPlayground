﻿using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using PromptPlayground.ViewModels.ConfigViewModels;
using PromptPlayground.ViewModels.ConfigViewModels.LLM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PromptPlayground.Services
{
    public class PromptService
    {
        private readonly TimeProvider _timeProvider = TimeProvider.System;
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

        private static IPromptTemplateFactory? CreatePromptTemplateFactory(string templateFormat)
        {
            return templateFormat switch
            {
                "handlebars" => new HandlebarsPromptTemplateFactory(),
                _ => null
            };
        }

        public async IAsyncEnumerable<GenerateResult> RunStreamAsync(string prompt,
            string templateFormat,
            PromptExecutionSettings? config,
            KernelArguments arguments,
            [EnumeratorCancellation]
            CancellationToken cancellationToken = default)
        {
            var _kernel = Build();
            var promptFilter = new KernelFilter();
            _kernel.PromptFilters.Add(promptFilter);
            _kernel.FunctionFilters.Add(promptFilter);

            var templateFactory = CreatePromptTemplateFactory(templateFormat);

            var startTimestamp = _timeProvider.GetTimestamp();
            var func = _kernel.CreateFunctionFromPrompt(prompt, config, templateFormat: templateFormat, promptTemplateFactory: templateFactory);

            var results = func.InvokeStreamingAsync(_kernel, arguments, cancellationToken);
            var sb = new StringBuilder();
            await foreach (var result in results.ConfigureAwait(false))
            {
                sb.Append(result.ToString());
                yield return new GenerateResult()
                {
                    Text = sb.ToString(),
                    Elapsed = _timeProvider.GetElapsedTime(startTimestamp),
                    PromptRendered = promptFilter.PromptRendered
                };
            }
        }

        public async Task<GenerateResult> RunAsync(string prompt,
            string templateFormat,
            PromptExecutionSettings? config,
            KernelArguments arguments,
            CancellationToken cancellationToken = default)
        {
            var _kernel = Build();
            var promptFilter = new KernelFilter();
            _kernel.PromptFilters.Add(promptFilter);
            _kernel.FunctionFilters.Add(promptFilter);

            var templateFactory = CreatePromptTemplateFactory(templateFormat);

            var startTimestamp = _timeProvider.GetTimestamp();
            var func = _kernel.CreateFunctionFromPrompt(prompt, config, templateFormat: templateFormat, promptTemplateFactory: templateFactory);

            var result = await func.InvokeAsync(_kernel, arguments, cancellationToken);
            var elapsed = _timeProvider.GetElapsedTime(startTimestamp);
            try
            {
                var usage = model.GetUsage(result);
                return new GenerateResult()
                {
                    Text = result.GetValue<string>() ?? "",
                    TokenUsage = usage,
                    Elapsed = elapsed,
                    PromptRendered = promptFilter.PromptRendered
                };
            }
            catch (KernelException ex)
            {
                return new GenerateResult()
                {
                    Text = ex.Message,
                    Elapsed = elapsed,
                    Error = ex.Message,
                };
            }
        }

        public static KernelArguments CreateArguments() => [];
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
