using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using PromptPlayground.ViewModels;
using PromptPlayground.Views.Args;
using System;

namespace PromptPlayground.Views;

public partial class SkillView : UserControl
{
    public SkillViewModel Model => (this.DataContext as SkillViewModel)!;

    public event EventHandler<FunctionSelectedArgs> FunctionSelected;

    public SkillView()
    {
        InitializeComponent();
        this.DataContext = new SkillViewModel();
        FunctionSelected += (sender, e) => { };
    }

    public void OpenFolder(string folder)
    {
        this.Model.Folder = folder;
    }

    public void OnSelectedFunctionChangedAsync(object e, SelectionChangedEventArgs args)
    {
        if (args.AddedItems.Count > 0)
        {
            Model.SelectedFunction = (args.AddedItems[0] as SemanticFunctionViewModel)!;
            FunctionSelected?.Invoke(this, new FunctionSelectedArgs(Model.SelectedFunction));
        }
    }
}