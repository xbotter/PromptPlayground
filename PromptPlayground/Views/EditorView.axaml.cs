using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.SemanticKernel;
using Microsoft;
using MsBox.Avalonia;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PromptPlayground.Views.Args;

namespace PromptPlayground.Views;

public partial class EditorView : UserControl
{
    private CancellationTokenSource? cancellationTokenSource;
    private SemanticFunctionViewModel model => (this.DataContext as SemanticFunctionViewModel)!;
    private Window mainWindow => (TopLevel.GetTopLevel(this) as Window)!;

    public Func<IKernel>? CreateKernel { get; set; }
    public Func<int> GetMaxCount { get; set; }

    public event EventHandler OnGenerating;
    public event EventHandler OnGenerated;
    public event EventHandler<GenerateResultArgs>? OnGeneratedResult;

    private int MaxCount() => GetMaxCount();
    public EditorView()
    {
        InitializeComponent();
        GetMaxCount = () => 3;
        OnGenerating += (sender, args) => this.model.IsGenerating = true;
        OnGenerated += (sender, args) => this.model.IsGenerating = false;
    }
    private async Task GenerateAsync()
    {
        try
        {
            OnGenerating?.Invoke(this, new());

            var kernel = (CreateKernel?.Invoke()) ?? throw new Exception("无法创建Kernel，请检查LLM配置");
            cancellationTokenSource?.Dispose();

            cancellationTokenSource = new CancellationTokenSource();
            var context = kernel.CreateNewContext(cancellationTokenSource.Token);
            if (model.Blocks.Count > 0)
            {
                var variables = this.model.Blocks.Select(_ => new Variable()
                {
                    Name = _.TrimStart('$')
                }).Distinct().ToList();
                var variablesVm = new VariablesViewModel(variables);
                var variableWindows = new VariablesWindows()
                {
                    DataContext = variablesVm,
                    ShowInTaskbar = false,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                await variableWindows.ShowDialog(this.mainWindow);
                if (variableWindows.Canceled)
                {
                    throw new Exception("生成已取消");
                }
                if (!variablesVm.Configured())
                {
                    throw new Exception("变量未配置");
                }
                foreach (var variable in variablesVm.Variables)
                {
                    context[variable.Name] = variable.Value;
                }
            }

            var service = new PromptService(kernel);


            for (int i = 0; i < MaxCount(); i++)
            {
                if (cancellationTokenSource?.IsCancellationRequested == true)
                {
                    break;
                }
                var result = await service.RunAsync(model.Prompt.Text, model.PromptConfig, context, cancellationToken: cancellationTokenSource!.Token);
                OnGeneratedResult?.Invoke(this, new GenerateResultArgs(result));
            }
        }
        catch (Exception ex)
        {
            var err = $"发生错误: {ex.Message}";
            await MessageBoxManager.GetMessageBoxStandard("Error", err, MsBox.Avalonia.Enums.ButtonEnum.Ok)
            .ShowWindowDialogAsync(mainWindow);
        }
        finally
        {
            OnGenerated?.Invoke(this, new EventArgs());
        }
    }


    private async void OnGenerateButtonClick(object sender, RoutedEventArgs e)
    {
        await GenerateAsync().ConfigureAwait(false);
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        cancellationTokenSource?.Cancel();
    }

    private void OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
        if (Directory.Exists(model.Folder))
        {
            File.WriteAllText(model.Prompt.FileName, model.Prompt.Text);
            File.WriteAllText(model.Config.FileName, model.Config.Text);
            model.IsChanged = false;
        }
    }

}