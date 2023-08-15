using PromptPlayground.Messages;

namespace PromptPlayground.ViewModels
{
    public class SkillOpenMessage : FileOrFolderOpenMessage
    {
        public SkillOpenMessage(string path) : base(path) { }
    }
}
