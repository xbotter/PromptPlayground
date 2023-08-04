using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels.ConfigViewModels.Embedding
{
    public class AzureOpenAIEmbeddingConfigViewModel : ConfigViewModelBase, IEmbeddingConfigViewModel
    {
        public override string Name => "Azure OpenAI";

        public AzureOpenAIEmbeddingConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
            RequireAttribute(ConfigAttribute.AzureEmbeddingDeployment);
            RequireAttribute(ConfigAttribute.AzureEndpoint);
            RequireAttribute(ConfigAttribute.AzureSecret);
        }

        public void RegisterEmbedding(KernelBuilder builder)
        {
            var deployment = GetAttribute(ConfigAttribute.AzureEmbeddingDeployment);
            var endpoint = GetAttribute(ConfigAttribute.AzureEndpoint);
            var secret = GetAttribute(ConfigAttribute.AzureSecret);


            builder.WithAzureTextEmbeddingGenerationService(deployment, endpoint, secret);
        }
    }
}
