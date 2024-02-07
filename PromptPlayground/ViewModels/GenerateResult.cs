using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PromptPlayground.ViewModels;
using System;

namespace PromptPlayground.Services
{
    public partial class GenerateResult : ViewModelBase
    {
        public string Text { get; set; } = string.Empty;
        public TimeSpan Elapsed { get; set; }
        public string? Error { get; set; } = string.Empty;
        public string? PromptRendered { get; set; }
        public ResultTokenUsage? TokenUsage { get; set; }

        public bool HasError => !string.IsNullOrWhiteSpace(Error);

        [RelayCommand]
        public void CopyPrompt()
        {
            if (!string.IsNullOrWhiteSpace(PromptRendered))
            {
                WeakReferenceMessenger.Default.Send(new CopyTextMessage(PromptRendered));
            }
        }

        [RelayCommand]
        public void CopyText()
        {
            if (!string.IsNullOrWhiteSpace(Text) && !HasError)
            {
                WeakReferenceMessenger.Default.Send(new CopyTextMessage(Text));
            }
        }

    }

    public record ResultTokenUsage(int Total, int Prompt, int Completion);
}
