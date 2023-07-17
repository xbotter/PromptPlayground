using Microsoft;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
