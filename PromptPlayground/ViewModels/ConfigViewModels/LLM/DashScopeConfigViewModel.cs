using Azure.AI.OpenAI;
using DashScope;
using DashScope.Models;
using Microsoft.SemanticKernel;
using PromptPlayground.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels.ConfigViewModels.LLM
{
    internal class DashScopeConfigViewModel : ConfigViewModelBase, ILLMConfigViewModel
    {
        public override string Name => "DashScope";

        public DashScopeConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
            RequireAttribute(ConfigAttribute.DashScopeApiKey);
            RequireAttribute(ConfigAttribute.DashScopeModel);
        }

        public IKernelBuilder CreateKernelBuilder()
        {
            var apiKey = GetAttribute(ConfigAttribute.DashScopeApiKey);
            var model = GetAttribute(ConfigAttribute.DashScopeModel);

            return Kernel.CreateBuilder().WithDashScopeCompletionService(apiKey, model);
        }

        public ResultTokenUsage? GetUsage(FunctionResult resultModel)
        {
            var usage = resultModel.Metadata?["Usage"] as CompletionUsage;
            if (usage == null)
            {
                return null;
            }
            return new ResultTokenUsage(usage.InputTokens + usage.OutputTokens, usage.InputTokens, usage.OutputTokens);
        }
    }
}
