using PromptPlayground.Messages;

namespace PromptPlayground.ViewModels
{
    public class PluginOpenMessage : FileOrFolderPathMessage
    {
        public PluginOpenMessage(string path) : base(path) { }
    }

    public class PluginCloseMessage : FileOrFolderPathMessage
    {
        public PluginCloseMessage(string path) : base(path) { }
    }
}
