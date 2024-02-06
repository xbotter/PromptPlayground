using PromptPlayground.Messages;

namespace PromptPlayground.ViewModels
{
    public class PluginOpenMessage : FileOrFolderOpenMessage
    {
        public PluginOpenMessage(string path) : base(path) { }
    }
}
