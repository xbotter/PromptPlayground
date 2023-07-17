using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PromptPlayground.ViewModels;

namespace PromptPlayground.Views;

public partial class SkillView : UserControl
{
    public SkillViewModel Model => (this.DataContext as SkillViewModel)!;
    public SkillView()
    {
        InitializeComponent();
        this.DataContext = new SkillViewModel();
    }
    public void OpenFolder(string folder)
    {
        this.Model.Folder = folder;
    }
}