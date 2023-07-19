using Microsoft;
using Microsoft.SemanticKernel;

namespace PromptPlayground.ViewModels.LLMConfigViewModels
{
    public class BaiduConfigViewModel : LLMConfigViewModelBase
    {
        const string ClientId = ConfigAttribute.BaiduClientId;
        const string Secret = ConfigAttribute.BaiduSecret;

        public override string Name => "Baidu";

        public BaiduConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
            RequireAttribute(ClientId);
            RequireAttribute(Secret);
        }
        public override IKernel CreateKernel()
        {
            Requires.NotNullOrWhiteSpace(ClientId, nameof(ClientId));
            Requires.NotNullOrWhiteSpace(ClientId, nameof(ClientId));

            return Kernel.Builder
            .WithERNIEBotChatCompletionService(GetAttribute(ClientId), GetAttribute(Secret))
                .Build();
        }
    }
}
