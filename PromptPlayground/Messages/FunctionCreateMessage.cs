namespace PromptPlayground.ViewModels
{
    public class FunctionCreateMessage
    {
        public FunctionCreateMessage(SemanticPluginViewModel? function = null)
        {
            Function = function;
        }

        public SemanticPluginViewModel? Function { get; }
    }
}
