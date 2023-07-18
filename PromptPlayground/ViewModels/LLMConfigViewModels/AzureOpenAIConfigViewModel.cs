using Microsoft;
using Microsoft.SemanticKernel;

namespace PromptPlayground.ViewModels.LLMConfigViewModels
{
    public class AzureOpenAIConfigViewModel : LLMConfigViewModelBase
    {
        const string Deployment = nameof(Deployment);
        const string Endpoint = nameof(Endpoint);
        const string Secret = nameof(Secret);
        public AzureOpenAIConfigViewModel()
        {
            RequireAttribute(Deployment);
            RequireAttribute(Endpoint);
            RequireAttribute(Secret);
        }
        public override string Name => "Azure OpenAI";

        public override IKernel CreateKernel()
        {
            Requires.NotNullOrWhiteSpace(GetAttribute(Deployment), Deployment);
            Requires.NotNullOrWhiteSpace(GetAttribute(Endpoint), Endpoint);
            Requires.NotNullOrWhiteSpace(GetAttribute(Secret), Secret);
            return Kernel.Builder
            .WithAzureChatCompletionService(GetAttribute(Deployment), GetAttribute(Endpoint), GetAttribute(Secret))
            .Build();
        }
    }

}
