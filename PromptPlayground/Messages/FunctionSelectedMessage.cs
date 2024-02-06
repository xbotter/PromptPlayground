using PromptPlayground.ViewModels;

namespace PromptPlayground.Messages
{
    public class FunctionSelectedMessage
    {
        public SemanticFunctionViewModel Function { get; set; }

        public FunctionSelectedMessage(SemanticFunctionViewModel viewModel)
        {
            this.Function = viewModel;
        }
    }
}
