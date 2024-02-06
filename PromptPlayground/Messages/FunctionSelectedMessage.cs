using PromptPlayground.ViewModels;

namespace PromptPlayground.Messages
{
    public class FunctionSelectedMessage
    {
        public SemanticPluginViewModel Function { get; set; }

        public FunctionSelectedMessage(SemanticPluginViewModel viewModel)
        {
            this.Function = viewModel;
        }
    }
}
