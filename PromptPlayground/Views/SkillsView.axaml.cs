using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PromptPlayground.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PromptPlayground.Views;

public partial class SkillsView : UserControl
{
    public SkillsViewModel Model => (this.DataContext as SkillsViewModel)!;

    public SkillsView()
    {
        InitializeComponent();
        this.DataContext = new SkillsViewModel();
    }
}