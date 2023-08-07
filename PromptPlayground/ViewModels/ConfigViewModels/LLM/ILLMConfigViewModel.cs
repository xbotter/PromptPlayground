using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using PromptPlayground.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PromptPlayground.ViewModels.ConfigViewModels.LLM
{
    public interface ILLMConfigViewModel : IConfigViewModel
    {
        public KernelBuilder CreateKernelBuilder();
        public ResultTokenUsage? GetUsage(ModelResult resultModel);
    }
}
