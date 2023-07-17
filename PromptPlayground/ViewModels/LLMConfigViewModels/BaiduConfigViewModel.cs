using Microsoft;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
