using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using PromptPlayground.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels.ConfigViewModels.LLM
{
    internal class OpenAIConfigViewModel : ConfigViewModelBase, ILLMConfigViewModel
    {
        public OpenAIConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
        }

        public override string Name => throw new NotImplementedException();

        public KernelBuilder CreateKernelBuilder()
        {
            throw new NotImplementedException();
        }

        public ResultTokenUsage? GetUsage(ModelResult resultModel)
        {
            throw new NotImplementedException();
        }
    }
}
