using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PromptPlayground.ViewModels;
using PromptPlayground.Views.Args;
using System.IO;

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
        this.ResultsView.DataContext ??= new ResultsViewModel();
        this.EditorView.OnGenerating += (sender, e) =>
        {
            this.model.Loading = true;
            this.ResultsView.Clear();
            this.model.StatusBar = $"(0/{model.Config.MaxCount}) 生成中...";

        };
        this.EditorView.OnGenerated += (sender, e) =>
        {
            this.model.Loading = false;
            this.model.StatusBar = string.Empty;
        };
        this.EditorView.OnGeneratedResult += (sender, result) =>
        {
            this.ResultsView.AddResult(result.Result);
            this.model.StatusBar = $"({this.ResultsView.GetCount()}/{model.Config.MaxCount}) 生成中...";
        };
        this.EditorView.GetConfigProvider = () => this.model.Config;
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
        if (this.SkillView.TrySelectFunction(filePath, out var func))
        {
            this.EditorView.DataContext = func;
        }
        else if (File.Exists(filePath) && Path.GetFileName(filePath) == Constants.SkPrompt)
        {
            this.EditorView.DataContext = new SemanticFunctionViewModel(Path.GetDirectoryName(filePath)!);
        }
    }


    private async void OnSkillDirOpen(object sender, RoutedEventArgs e)
    {
        await this.SkillView.OpenFolderAsync();
    }

    private void OnNewFile(object sender, RoutedEventArgs e)
    {
        this.SkillView.NoFunctionSelected();
        this.EditorView.DataContext = new SemanticFunctionViewModel("");
    }
}
