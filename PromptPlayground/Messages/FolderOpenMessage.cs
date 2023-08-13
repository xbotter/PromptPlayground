namespace PromptPlayground.ViewModels
{
    public class FolderOpenMessage
    {
        public FolderOpenMessage(string? folder)
        {
            Folder = folder;
        }
        public string? Folder { get; set; }
    }
}
