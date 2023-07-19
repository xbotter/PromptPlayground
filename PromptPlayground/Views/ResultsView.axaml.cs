using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;
using System;

namespace PromptPlayground.Views;

public partial class ResultsView : UserControl
{
    private WindowNotificationManager? _manager;

    private ResultsViewModel model => (this.DataContext as ResultsViewModel)!;
    public ResultsView()
    {
        InitializeComponent();
    }

    public void Clear()
    {
        this.model.Results.Clear();
    }
    public void AddResult(GenerateResult result)
    {
        this.model.Results.Add(result);
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var topLevel = TopLevel.GetTopLevel(this);
        _manager = new WindowNotificationManager(topLevel) { MaxItems = 3 };
    }

    public int GetCount()
    {
        return this.model.Results.Count;
    }

    public async void OnCopyClick(object sender, RoutedEventArgs e)
    {
        var result = (sender as Button)!.DataContext as GenerateResult;

        if (result is null)
            return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null || topLevel.Clipboard is null)
            return;

        await topLevel.Clipboard.SetTextAsync(result.Text);

        _manager?.Show(new Notification("Copied", "", NotificationType.Success, TimeSpan.FromSeconds(1)));

    }

}