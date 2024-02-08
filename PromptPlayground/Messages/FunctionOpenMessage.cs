using PromptPlayground.Messages;

namespace PromptPlayground.ViewModels
{
    public class FunctionOpenMessage : FileOrFolderPathMessage
    {
        public FunctionOpenMessage(string folder) : base(folder)
        {

        }
    }

    public class FunctionSavedMessage : FileOrFolderPathMessage
    {
        public FunctionSavedMessage(string folder) : base(folder)
        {

        }
    }
}
