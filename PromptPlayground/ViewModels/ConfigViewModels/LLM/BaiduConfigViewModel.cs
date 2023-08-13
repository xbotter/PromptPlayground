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
            RequireAttribute(ClientId);
            RequireAttribute(Secret);
        }
        public KernelBuilder CreateKernelBuilder()
        {
            Requires.NotNullOrWhiteSpace(ClientId, nameof(ClientId));
            Requires.NotNullOrWhiteSpace(ClientId, nameof(ClientId));

            return Kernel.Builder
                .WithERNIEBotChatCompletionService(GetAttribute(ClientId), GetAttribute(Secret))
                ;
        }
        public ResultTokenUsage? GetUsage(ModelResult result)
        {
            var completions = result.GetResult<ChatResponse>();
            return new ResultTokenUsage(completions.Usage.TotalTokens, completions.Usage.PromptTokens, completions.Usage.CompletionTokens);
        }
    }
}
