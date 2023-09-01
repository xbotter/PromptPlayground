using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using PromptPlayground.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels.ConfigViewModels.LLM
{
    internal class OpenAIConfigViewModel : ConfigViewModelBase, ILLMConfigViewModel
    {
        public OpenAIConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
            RequireAttribute(ConfigAttribute.OpenAIModel);
            RequireAttribute(ConfigAttribute.OpenAIApiKey);
        }

        public override string Name => "OpenAI";

        public KernelBuilder CreateKernelBuilder()
        {
            var model = GetAttribute(ConfigAttribute.OpenAIModel);
            var apiKey = GetAttribute(ConfigAttribute.OpenAIApiKey);

            return Kernel.Builder
                         .WithOpenAIChatCompletionService(model, apiKey);
        }

        public ResultTokenUsage? GetUsage(ModelResult result)
        {
            var completions = result.GetResult<ChatCompletions>();
            return new ResultTokenUsage(completions.Usage.TotalTokens, completions.Usage.PromptTokens, completions.Usage.CompletionTokens);
        }
    }
}
