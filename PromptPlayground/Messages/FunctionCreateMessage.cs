namespace PromptPlayground.ViewModels
{
    public class FunctionCreateMessage
    {
        public FunctionCreateMessage(SemanticFunctionViewModel? function = null)
        {
            Function = function;
        }

        public SemanticFunctionViewModel? Function { get; }
    }
}
