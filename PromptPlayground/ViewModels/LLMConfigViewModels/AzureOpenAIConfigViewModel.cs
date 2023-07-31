using Azure.AI.OpenAI;
using Microsoft;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using PromptPlayground.Services;

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

        public override ResultTokenUsage? GetUsage(ModelResult result)
        {
            var completions = result.GetResult<ChatCompletions>();
            return new ResultTokenUsage()
            {
                Total = completions.Usage.TotalTokens,
                Prompt = completions.Usage.PromptTokens,
                Completion = completions.Usage.CompletionTokens
            };
        }
    }

}
