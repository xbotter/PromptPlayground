using ERNIE_Bot.SDK;
using ERNIE_Bot.SDK.Models;
using Microsoft;
using Microsoft.SemanticKernel;
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
		public IKernelBuilder CreateKernelBuilder()
		{
			return Kernel.CreateBuilder()
				.WithERNIEBotChatCompletionService(GetAttribute(ClientId), GetAttribute(Secret), modelEndpoint: ModelEndpoint);
		}
		public ResultTokenUsage? GetUsage(FunctionResult result)
		{
			var usage = result.Metadata?["Usage"] as UsageData;
			if (usage == null)
			{
				return null;
			}
			return new ResultTokenUsage(usage.TotalTokens, usage.PromptTokens, usage.CompletionTokens);
		}

		private ModelEndpoint ModelEndpoint =>
			GetAttribute(ConfigAttribute.BaiduModel) switch
			{
				"BLOOMZ_7B" => ModelEndpoints.BLOOMZ_7B,
				"Ernie-Bot-turbo" => ModelEndpoints.ERNIE_Bot_Turbo,
				_ => ModelEndpoints.ERNIE_Bot
			};
	}
}
