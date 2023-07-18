using Microsoft;
using Microsoft.SemanticKernel;

namespace PromptPlayground.ViewModels.LLMConfigViewModels
{
    public class BaiduConfigViewModel : LLMConfigViewModelBase
    {
        const string ClientId = nameof(ClientId);
        const string Secret = nameof(Secret);

        public override string Name => "Baidu";

        public BaiduConfigViewModel()
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
