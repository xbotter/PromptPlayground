using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PromptPlayground.ViewModels;

namespace PromptPlayground.Views;

public partial class ConfigWindow : Window
{
    public ConfigWindow()
    {
        InitializeComponent();
        this.Closing += ConfigWindow_Closing;
    }

    private void ConfigWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (this.DataContext is ConfigViewModel config)
        {
            config.SaveConfig();
        }
    }
}