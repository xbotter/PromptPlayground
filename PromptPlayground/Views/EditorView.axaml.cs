using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.SemanticKernel;
using MsBox.Avalonia;
using PromptPlayground.Messages;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;
using PromptPlayground.ViewModels.ConfigViewModels;
using PromptPlayground.ViewModels.ConfigViewModels.LLM;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromptPlayground.Views;

public partial class EditorView : UserControl, IRecipient<FunctionSelectedMessage>
{
    public EditorView()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void Receive(FunctionSelectedMessage message)
    {
        this.DataContext = message.Function;
    }


    private void ShowHistoryWindow(object? sender, RoutedEventArgs e)
    {
        var window = new HistoryWindow()
        {
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        window.Show();
    }

}