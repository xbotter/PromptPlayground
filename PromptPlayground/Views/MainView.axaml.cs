using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ERNIE_Bot.SDK.Models;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using PromptPlayground.Messages;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PromptPlayground.Views;

public partial class MainView : UserControl, IRecipient<RequestFolderOpen>,
                                             IRecipient<RequestFileOpen>,
                                             IRecipient<ConfirmRequestMessage>,
                                             IRecipient<NotificationMessage>,
                                             IRecipient<CopyTextMessage>,
                                             IRecipient<RequestVariablesMessage>
{
    private WindowNotificationManager _manager;

    private MainViewModel model => (this.DataContext as MainViewModel)!;
    private Window mainWindow => (this.Parent as Window)!;

    public MainView()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.RegisterAll(this);
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var topLevel = TopLevel.GetTopLevel(this);
        _manager = new WindowNotificationManager(topLevel) { MaxItems = 3 };

        var defaultFunction = new SemanticPluginViewModel("[New Function]");
        this.EditorView.DataContext = defaultFunction;
        this.ResultsView.DataContext = new ResultsViewModel(defaultFunction);

        WeakReferenceMessenger.Default.Send(new FunctionCreateMessage(defaultFunction));
    }

    private void AboutClick(object sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutView()
        {
            DataContext = model.About,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        aboutWindow.Show(mainWindow);
    }

    private void OnConfigClick(object sender, RoutedEventArgs e)
    {
        var configWindow = new ConfigWindow()
        {
            DataContext = model.Config,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        configWindow.ShowDialog(mainWindow);
    }

    private async Task<string?> FolderOpenAsync()
    {
        var folders = await TopLevel.GetTopLevel(this)!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            AllowMultiple = false
        });
        var folder = folders.FirstOrDefault()?.TryGetLocalPath();
        return folder;
    }
    private async Task<string?> FileOpenAsync()
    {
        var file = await TopLevel.GetTopLevel(this)!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = false,
            FileTypeFilter = new FilePickerFileType[]
            {
                new FilePickerFileType("sk prompt")
                {
                    Patterns = new string[] { Constants.SkPrompt }
                }
            }
        });
        var filePath = file.FirstOrDefault()?.TryGetLocalPath();
        return filePath;
    }

    private async Task<bool> ConfirmRequestMessageAsync(ConfirmRequestMessage message)
    {
        var result = await MessageBoxManager.GetMessageBoxStandard(message.Title, message.Message, MsBox.Avalonia.Enums.ButtonEnum.OkCancel)
            .ShowAsPopupAsync(this);

        return result == MsBox.Avalonia.Enums.ButtonResult.Ok;
    }

    public void Receive(RequestFolderOpen message)
    {
        message.Reply(FolderOpenAsync());
    }

    public void Receive(RequestFileOpen message)
    {
        message.Reply(FileOpenAsync());
    }

    public void Receive(ConfirmRequestMessage message)
    {
        message.Reply(ConfirmRequestMessageAsync(message));
    }

    public void Receive(NotificationMessage message)
    {
        _manager?.Show(new Notification(message.Title, message.Message, (NotificationType)message.Level, TimeSpan.FromSeconds(1.5)));
    }

    public async void Receive(CopyTextMessage message)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null || topLevel.Clipboard is null)
            return;

        await topLevel.Clipboard.SetTextAsync(message.Text);

        _manager?.Show(new Notification("Copied", "", NotificationType.Success, TimeSpan.FromSeconds(1)));
    }

    public void Receive(RequestVariablesMessage message)
    {
        message.Reply(ShowVariablesWindows(message.Variables));
    }
    private async Task<VariablesViewModel> ShowVariablesWindows(List<Variable> variables)
    {
        var variablesVm = new VariablesViewModel(variables);
        var variableWindows = new VariablesWindows()
        {
            DataContext = variablesVm,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        await variableWindows.ShowDialog(mainWindow);
        return variablesVm;
    }
}
