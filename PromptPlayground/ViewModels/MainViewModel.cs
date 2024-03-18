using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using PromptPlayground.Messages;
using System;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    static IMessenger Messenger => WeakReferenceMessenger.Default;

    public ConfigViewModel Config { get; set; } = new(true);

    public StatusViewModel Status { get; set; } = new();

    public AboutViewModel About { get; set; } = new();


    public MainViewModel()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    [RelayCommand]
    public static void NewFile()
    {
        WeakReferenceMessenger.Default.Send(new FunctionCreateMessage());
    }

    [RelayCommand]
    public async Task OpenFileAsync()
    {
        var response = await Messenger.Send<RequestFileOpen>();

        if (!string.IsNullOrWhiteSpace(response))
        {
            WeakReferenceMessenger.Default.Send(new FunctionOpenMessage(response));
        }
    }

    [RelayCommand]
    public async Task OpenFolderAsync()
    {
        var response = await Messenger.Send<RequestFolderOpen>();

        if (!string.IsNullOrWhiteSpace(response))
        {
            WeakReferenceMessenger.Default.Send(new PluginOpenMessage(response!));
        }
    }
}
