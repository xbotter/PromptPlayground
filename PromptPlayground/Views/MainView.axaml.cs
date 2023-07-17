using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;
using Microsoft;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;
using MsBox.Avalonia;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;
using PromptPlayground.Views.Args;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PromptPlayground.Views;

public partial class MainView : UserControl
{
    private MainViewModel model => (this.DataContext as MainViewModel)!;
    private Window mainWindow => (this.Parent as Window)!;


    public MainView()
    {
        InitializeComponent();
        this.SkillView.FunctionSelected += SkillView_FunctionSelected;
        this.EditorView.DataContext = new SemanticFunctionViewModel("");
        this.EditorView.OnGenerating += (sender, e) =>
        {
            this.model.Loading = true;
            this.model.Results.Clear();
            this.ResultsView.DataContext = new ResultsViewModel(model.Results);
        };
        this.EditorView.OnGenerated += (sender, e) => this.model.Loading = false;
        this.EditorView.OnGeneratedResult += (sender, result) =>
        {
            this.model.Results.Add(result.Result);
        };
        this.EditorView.CreateKernel = () => this.model.Config.SelectedModel.CreateKernel();
        this.EditorView.GetMaxCount = () => this.model.Config.MaxCount;

    }

    private void SkillView_FunctionSelected(object? sender, FunctionSelectedArgs e)
    {
        var folder = e.SelectedFunction.Folder;
        var filePath = Path.Combine(folder, Constants.SkPrompt);
        OpenFile(filePath);
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
    private async void OnImportFile(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }
        var file = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
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
        if (file.Count > 0)
        {
            var filePath = file[0].TryGetLocalPath()!;
            OpenFile(filePath);
        }
    }
    private void OpenFile(string filePath)
    {
        if (File.Exists(filePath) && Path.GetFileName(filePath) == Constants.SkPrompt)
        {
            this.EditorView.DataContext = new SemanticFunctionViewModel(Path.GetDirectoryName(filePath)!);
        }
    }

    private async void OnSkillDirOpen(object sender, RoutedEventArgs e)
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
                this.SkillView.OpenFolder(localPath);
            }
        }
    }
}
