using PromptPlayground.Messages;

namespace PromptPlayground.ViewModels
{
    public class FunctionOpenMessage : FileOrFolderOpenMessage
    {
        public FunctionOpenMessage(string folder) : base(folder)
        {

        }
    }
}
