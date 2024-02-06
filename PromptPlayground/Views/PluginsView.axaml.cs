using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PromptPlayground.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PromptPlayground.Views;

public partial class PluginsView : UserControl
{
    public PluginsViewModel Model => (this.DataContext as PluginsViewModel)!;

    public PluginsView()
    {
        InitializeComponent();
        this.DataContext = new PluginsViewModel();
    }
}