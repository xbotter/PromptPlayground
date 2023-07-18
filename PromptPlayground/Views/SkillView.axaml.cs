using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using PromptPlayground.ViewModels;
using PromptPlayground.Views.Args;
using System;
using System.IO;
using System.Threading.Tasks;

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

    public async Task OpenFolderAsync()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }
        var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            AllowMultiple = false
        });
        if (folder.Count > 0)
        {
            var localPath = folder[0].TryGetLocalPath();
            if (Path.Exists(localPath))
            {
                this.Model.Folder = localPath;
                
            }
        }
       
    }

    public void OnSelectedFunctionChangedAsync(object e, SelectionChangedEventArgs args)
    {
        if (args.AddedItems.Count > 0)
        {
            Model.SelectedFunction = (args.AddedItems[0] as SemanticFunctionViewModel)!;
            FunctionSelected?.Invoke(this, new FunctionSelectedArgs(Model.SelectedFunction));
        }
    }

    public async void OnOpenFolder(object sender, RoutedEventArgs e)
    {
        await OpenFolderAsync();
    }

}