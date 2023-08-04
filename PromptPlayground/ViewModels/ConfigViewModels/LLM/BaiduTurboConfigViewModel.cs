using ERNIE_Bot.SDK.Models;
using Microsoft;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using PromptPlayground.Services;

namespace PromptPlayground.ViewModels.ConfigViewModels.LLM
{
    public class BaiduTurboConfigViewModel : LLMConfigViewModelBase
    {
        const string ClientId = ConfigAttribute.BaiduClientId;
        const string Secret = ConfigAttribute.BaiduSecret;

        public override string Name => "Baidu Turbo";

        public BaiduTurboConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
            RequireAttribute(ClientId);
            RequireAttribute(Secret);
        }
        public override IKernel CreateKernel()
        {
            Requires.NotNullOrWhiteSpace(ClientId, nameof(ClientId));
            Requires.NotNullOrWhiteSpace(ClientId, nameof(ClientId));

            return Kernel.Builder
            .WithERNIEBotTurboChatCompletionService(GetAttribute(ClientId), GetAttribute(Secret))
                .Build();
        }
        public override ResultTokenUsage? GetUsage(ModelResult result)
        {
            var completions = result.GetResult<ChatResponse>();
            return new ResultTokenUsage()
            {
                Total = completions.Usage.TotalTokens,
                Prompt = completions.Usage.PromptTokens,
                Completion = completions.Usage.CompletionTokens
            };
        }
    }
}
