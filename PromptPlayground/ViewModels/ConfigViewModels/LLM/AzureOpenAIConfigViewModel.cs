using Azure.AI.OpenAI;
using Microsoft;
using Microsoft.SemanticKernel;
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

		public IKernelBuilder CreateKernelBuilder()
		{
			Requires.NotNullOrWhiteSpace(GetAttribute(ConfigAttribute.AzureDeployment), ConfigAttribute.AzureDeployment);
			Requires.NotNullOrWhiteSpace(GetAttribute(ConfigAttribute.AzureEndpoint), ConfigAttribute.AzureEndpoint);
			Requires.NotNullOrWhiteSpace(GetAttribute(ConfigAttribute.AzureSecret), ConfigAttribute.AzureSecret);

			return Kernel.CreateBuilder()
				.AddAzureOpenAIChatCompletion(GetAttribute(ConfigAttribute.AzureDeployment),
				GetAttribute(ConfigAttribute.AzureEndpoint),
				GetAttribute(ConfigAttribute.AzureSecret));
		}

		public ResultTokenUsage? GetUsage(FunctionResult result)
		{
			var usage = result.Metadata?["Usage"] as CompletionsUsage;
			if (usage != null)
			{
				return new ResultTokenUsage(usage.TotalTokens, usage.PromptTokens, usage.CompletionTokens);
			}
			return null;
		}
	}

}
