using Avalonia.Interactivity;
using Microsoft.SemanticKernel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels.ConfigViewModels.VectorDB
{
    public interface IVectorDbConfigViewModel : IConfigViewModel
    {
        void RegisterMemory(KernelBuilder kernel);
        string Collection { get; set; }
        int Limit { get; set; }
    }
}
