﻿using ERNIE_Bot.SDK;
using ERNIE_Bot.SDK.Models;
using Microsoft;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using PromptPlayground.Services;

namespace PromptPlayground.ViewModels.ConfigViewModels.LLM
{
    public class BaiduConfigViewModel : ConfigViewModelBase, ILLMConfigViewModel
    {
        const string ClientId = ConfigAttribute.BaiduClientId;
        const string Secret = ConfigAttribute.BaiduSecret;

        public override string Name => "Baidu";

        public BaiduConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
            RequireAttribute(ConfigAttribute.BaiduModel);
            RequireAttribute(ClientId);
            RequireAttribute(Secret);
        }
        public KernelBuilder CreateKernelBuilder()
        {
            return Kernel.Builder
                .WithERNIEBotChatCompletionService(GetAttribute(ClientId), GetAttribute(Secret), modelEndpoint: ModelEndpoint);
        }
        public ResultTokenUsage? GetUsage(ModelResult result)
        {
            var completions = result.GetResult<ChatResponse>();
            return new ResultTokenUsage(completions.Usage.TotalTokens, completions.Usage.PromptTokens, completions.Usage.CompletionTokens);
        }

        private string ModelEndpoint =>
            GetAttribute(ConfigAttribute.BaiduModel) switch
            {
                "BLOOMZ_7B" => ModelEndpoints.BLOOMZ_7B,
                "Ernie-Bot-turbo" => ModelEndpoints.ERNIE_Bot_Turbo,
                _ => ModelEndpoints.ERNIE_Bot
            };
    }
}
