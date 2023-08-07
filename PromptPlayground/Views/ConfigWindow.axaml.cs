using Avalonia.Controls;
using Avalonia.Interactivity;
using PromptPlayground.ViewModels;
using System;

namespace PromptPlayground.Views;

public partial class ConfigWindow : Window
{
    public ConfigWindow()
    {
        InitializeComponent();
        this.Closed += ConfigWindow_Closed;
    }

    private void ConfigWindow_Closed(object? sender, EventArgs e)
    {
        if (this.DataContext is ConfigViewModel config)
        {
            config.ReloadConfig();
        }
    }

    public void SaveConfig(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ConfigViewModel config)
        {
            config.SaveConfig();
        }
        this.Close();
    }
    public void ResetConfig(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}