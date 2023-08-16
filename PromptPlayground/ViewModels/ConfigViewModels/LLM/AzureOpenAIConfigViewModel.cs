using Azure.AI.OpenAI;
using Microsoft;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using PromptPlayground.Services;

namespace PromptPlayground.ViewModels.ConfigViewModels.LLM
{
    public class AzureOpenAIConfigViewModel : ConfigViewModelBase, ILLMConfigViewModel
    {
        public AzureOpenAIConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
            RequireAttribute(ConfigAttribute.AzureDeployment);
            RequireAttribute(ConfigAttribute.AzureEndpoint);
            RequireAttribute(ConfigAttribute.AzureSecret);
        }
        public override string Name => "Azure OpenAI";

        public KernelBuilder CreateKernelBuilder()
        {
            Requires.NotNullOrWhiteSpace(GetAttribute(ConfigAttribute.AzureDeployment), ConfigAttribute.AzureDeployment);
            Requires.NotNullOrWhiteSpace(GetAttribute(ConfigAttribute.AzureEndpoint), ConfigAttribute.AzureEndpoint);
            Requires.NotNullOrWhiteSpace(GetAttribute(ConfigAttribute.AzureSecret), ConfigAttribute.AzureSecret);

            return Kernel.Builder
                .WithAzureChatCompletionService(GetAttribute(ConfigAttribute.AzureDeployment), GetAttribute(ConfigAttribute.AzureEndpoint), GetAttribute(ConfigAttribute.AzureSecret))
                 ;
        }

        public ResultTokenUsage? GetUsage(ModelResult result)
        {
            var completions = result.GetResult<ChatCompletions>();
            return new ResultTokenUsage(completions.Usage.TotalTokens, completions.Usage.PromptTokens, completions.Usage.CompletionTokens);
        }
    }

}
