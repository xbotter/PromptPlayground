using Microsoft.SemanticKernel;


namespace PromptPlayground.ViewModels.ConfigViewModels.VectorDB
{
    public interface IVectorDbConfigViewModel : IConfigViewModel
    {
        void RegisterMemory(KernelBuilder kernel);
    }
}
