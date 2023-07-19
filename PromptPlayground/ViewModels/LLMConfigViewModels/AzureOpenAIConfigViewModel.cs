using Microsoft;
using Microsoft.SemanticKernel;

namespace PromptPlayground.ViewModels.LLMConfigViewModels
{
    public class AzureOpenAIConfigViewModel : LLMConfigViewModelBase
    {

        public AzureOpenAIConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
            RequireAttribute(ConfigAttribute.AzureDeployment);
            RequireAttribute(ConfigAttribute.AzureEndpoint);
            RequireAttribute(ConfigAttribute.AzureSecret);
        }
        public override string Name => "Azure OpenAI";

        public override IKernel CreateKernel()
        {
            Requires.NotNullOrWhiteSpace(GetAttribute(ConfigAttribute.AzureDeployment), ConfigAttribute.AzureDeployment);
            Requires.NotNullOrWhiteSpace(GetAttribute(ConfigAttribute.AzureEndpoint), ConfigAttribute.AzureEndpoint);
            Requires.NotNullOrWhiteSpace(GetAttribute(ConfigAttribute.AzureSecret), ConfigAttribute.AzureSecret);
            return Kernel.Builder
            .WithAzureChatCompletionService(GetAttribute(ConfigAttribute.AzureDeployment), GetAttribute(ConfigAttribute.AzureEndpoint), GetAttribute(ConfigAttribute.AzureSecret))
            .Build();
        }
    }

}
