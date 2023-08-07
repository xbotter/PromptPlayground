using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels.ConfigViewModels.Embedding
{
    public interface IEmbeddingConfigViewModel : IConfigViewModel
    {
        void RegisterEmbedding(KernelBuilder builder);
    }
}
