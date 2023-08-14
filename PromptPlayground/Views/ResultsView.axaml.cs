using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;
using System;

namespace PromptPlayground.Views;

public partial class ResultsView : UserControl
{
    public ResultsView()
    {
        InitializeComponent();
    }
}