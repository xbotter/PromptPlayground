using PromptPlayground.ViewModels;

namespace PromptPlayground.Views.Args
{
    public class FunctionSelectedArgs
    {
        public FunctionSelectedArgs(SemanticFunctionViewModel function)
        {
            SelectedFunction = function;
        }
        public SemanticFunctionViewModel SelectedFunction { get; set; }
    }
}