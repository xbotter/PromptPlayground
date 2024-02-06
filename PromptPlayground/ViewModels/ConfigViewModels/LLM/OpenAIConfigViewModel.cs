using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
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

        public IKernelBuilder CreateKernelBuilder()
        {
            var model = GetAttribute(ConfigAttribute.OpenAIModel);
            var apiKey = GetAttribute(ConfigAttribute.OpenAIApiKey);

            return Kernel.CreateBuilder()
                    .AddOpenAIChatCompletion(model, apiKey);
        }

        public ResultTokenUsage? GetUsage(FunctionResult result)
        {
            var usage = result.Metadata?["Usage"] as CompletionsUsage;
            if (usage == null)
            {
                return null;
            }
            return new ResultTokenUsage(usage.TotalTokens, usage.PromptTokens, usage.CompletionTokens);
        }
    }
}
