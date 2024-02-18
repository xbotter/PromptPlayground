using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PromptPlayground.ViewModels;
using System;
using System.Collections.Generic;

namespace PromptPlayground.Services
{
    public partial class AverageResult : ViewModelBase
    {
        [ObservableProperty]
        private bool hasResults = false;

        [ObservableProperty]
        private TimeSpan elapsed;

        [ObservableProperty]
        private ResultTokenUsage? tokenUsage;

    }

    public partial class GenerateResult : ViewModelBase
    {
        [ObservableProperty]
        private string text = string.Empty;

        [ObservableProperty]
        private TimeSpan? elapsed;

        [ObservableProperty]
        private string? error;

        [ObservableProperty]
        private string? promptRendered;

        [ObservableProperty]
        private ResultTokenUsage? tokenUsage;

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
