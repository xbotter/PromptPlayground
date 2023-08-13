using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.SemanticKernel;
using MsBox.Avalonia;
using PromptPlayground.Messages;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;
using PromptPlayground.ViewModels.ConfigViewModels;
using PromptPlayground.ViewModels.ConfigViewModels.LLM;
using PromptPlayground.ViewModels.ConfigViewModels.VectorDB;
using PromptPlayground.Views.Args;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromptPlayground.Views;

public partial class EditorView : UserControl, IRecipient<FunctionSelectedMessage>
{
    private SemanticFunctionViewModel model => (this.DataContext as SemanticFunctionViewModel)!;

    public Func<IConfigAttributesProvider>? GetConfigProvider { get; set; }

    public EditorView()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    private async Task GenerateAsync()
    {
        //try
        //{
        //    OnGenerating?.Invoke(this, new());

        //    cancellationTokenSource?.Dispose();

        //    cancellationTokenSource = new CancellationTokenSource();
        //    var service = new PromptService(GetConfigProvider?.Invoke());

        //    var context = service.CreateContext();
        //    var varBlocks = model.Blocks;
        //    if (varBlocks.Count > 0)
        //    {
        //        var variables = varBlocks.Select(_ => new Variable()
        //        {
        //            Name = _.TrimStart('$')
        //        }).Distinct().ToList();
        //        var variablesVm = new VariablesViewModel(variables);
        //        var variableWindows = new VariablesWindows()
        //        {
        //            DataContext = variablesVm,
        //            ShowInTaskbar = false,
        //            WindowStartupLocation = WindowStartupLocation.CenterOwner
        //        };
        //        await variableWindows.ShowDialog(this.mainWindow);
        //        if (variableWindows.Canceled)
        //        {
        //            throw new Exception("生成已取消");
        //        }
        //        if (!variablesVm.Configured())
        //        {
        //            throw new Exception("变量未配置");
        //        }
        //        foreach (var variable in variablesVm.Variables)
        //        {
        //            context[variable.Name] = variable.Value;
        //        }
        //    }

        //    for (int i = 0; i < MaxCount(); i++)
        //    {
        //        if (cancellationTokenSource?.IsCancellationRequested == true)
        //        {
        //            break;
        //        }

        //        var result = await service.RunAsync(model.Prompt, model.PromptConfig, context.Clone(), cancellationToken: cancellationTokenSource!.Token);
        //        OnGeneratedResult?.Invoke(this, new GenerateResultArgs(result));
        //    }
        //}
        //catch (Exception ex)
        //{
        //    var err = $"发生错误: {ex.Message}";
        //    await MessageBoxManager.GetMessageBoxStandard("Error", err, MsBox.Avalonia.Enums.ButtonEnum.Ok)
        //    .ShowWindowDialogAsync(mainWindow);
        //}
        //finally
        //{
        //    OnGenerated?.Invoke(this, new EventArgs());
        //}
    }

    public void Receive(FunctionSelectedMessage message)
    {
        this.DataContext = message.Function;
    }
}