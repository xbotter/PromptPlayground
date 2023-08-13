﻿using Avalonia.Controls;
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
    IMessenger Messenger => WeakReferenceMessenger.Default;

    public ConfigViewModel Config { get; set; } = new(true);

    public StatusViewModel Status { get; set; } = new();


    public MainViewModel()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    [RelayCommand]
    public void NewFile()
    {
        WeakReferenceMessenger.Default.Send(new FunctionCreateMessage());
    }

    [RelayCommand]
    public async Task OpenFileAsync()
    {
        var response = await this.Messenger.Send<AsyncRequestMessage<FileOpenMessage>>();

        if (response.FilePath != null)
        {
            WeakReferenceMessenger.Default.Send(new FunctionOpenMessage(response.FilePath));
        }
    }

    [RelayCommand]
    public async Task OpenFolderAsync()
    {
        var response = await this.Messenger.Send<AsyncRequestMessage<FolderOpenMessage>>();

        if (response.Folder != null)
        {
            WeakReferenceMessenger.Default.Send(new SkillOpenMessage(response.Folder));
        }
    }
}
