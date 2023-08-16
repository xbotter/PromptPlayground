namespace PromptPlayground.Services
{
    public class CopyTextMessage
    {
        public CopyTextMessage(string text)
        {
            this.Text = text;
        }

        public string Text { get; set; }
    }
}