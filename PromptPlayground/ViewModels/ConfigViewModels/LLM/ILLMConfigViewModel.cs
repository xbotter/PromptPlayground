using Microsoft.SemanticKernel;
using PromptPlayground.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PromptPlayground.ViewModels.ConfigViewModels.LLM
{
    public interface ILLMConfigViewModel : IConfigViewModel
    {
        public IKernelBuilder CreateKernelBuilder();
        public ResultTokenUsage? GetUsage(FunctionResult resultModel);
    }
}
