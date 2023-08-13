using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;
using System;

namespace PromptPlayground.Views;

public partial class ResultsView : UserControl, IRecipient<FunctionChangedMessage>
{
    private ResultsViewModel model => (this.DataContext as ResultsViewModel)!;
    public ResultsView()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public int GetCount()
    {
        return this.model.Results.Count;
    }

    public void Receive(FunctionChangedMessage message)
    {
        this.model.Results = message.Function.Results;
    }
}