using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;
using Microsoft;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TemplateEngine;
using Microsoft.SemanticKernel.TemplateEngine.Blocks;
using MsBox.Avalonia;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    }


    private void OnConfigClick(object sender, RoutedEventArgs e)
    {
        var configWindow = new ConfigWindow()
        {
            DataContext = model.Config,
        };

        configWindow.ShowDialog(mainWindow);
    }

    private async Task GenerateAsync()
    {
        try
        {
            Loading(true);


            var kernel = CreateKernel();

            var context = kernel.CreateNewContext();
            if (model.Blocks.Count > 0)
            {
                var variables = this.model.Blocks.Select(_ => new Variable()
                {
                    Name = _.TrimStart('$')
                }).Distinct().ToList();
                var variablesVm = new VariablesViewModel(variables);
                var variableWindows = new VariablesWindows()
                {
                    DataContext = variablesVm
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
            model.Results.Clear();
            for (int i = 0; i < model.Config.MaxCount; i++)
            {
                var result = await service.RunAsync(model.Prompt, context);
                model.Results.Add(result);
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
            Loading(false);
        }
    }

    private IKernel CreateKernel()
    {
        if (model.Config.ModelSelectAzure)
        {
            Requires.NotNullOrWhiteSpace(model.Config.AzureDeployment, nameof(model.Config.AzureDeployment));
            Requires.NotNullOrWhiteSpace(model.Config.AzureEndpoint, nameof(model.Config.AzureEndpoint));
            Requires.NotNullOrWhiteSpace(model.Config.AzureSecret, nameof(model.Config.AzureSecret));

            return Kernel.Builder
               .WithAzureChatCompletionService(model.Config.AzureDeployment, model.Config.AzureEndpoint, model.Config.AzureSecret)
               .Build();
        }
        else if (model.Config.ModelSelectBaidu)
        {
            Requires.NotNullOrWhiteSpace(model.Config.BaiduClientId, nameof(model.Config.BaiduClientId));
            Requires.NotNullOrWhiteSpace(model.Config.BaiduSecret, nameof(model.Config.BaiduSecret));

            return Kernel.Builder
                .WithERNIEBotTurboChatCompletionService(model.Config.BaiduClientId, model.Config.BaiduSecret)
                .Build();
        }
        else
        {
            throw new Exception("请先配置接口参数");
        }

    }

    private void Loading(bool loading)
    {
        model.Loading = loading;
    }

    private async void OnGenerateButtonClick(object sender, RoutedEventArgs e)
    {
        await GenerateAsync().ConfigureAwait(false);
    }

    private void OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
        if (model.LinkDocument)
        {
            File.WriteAllText(model.Document.FileName, model.Document.Text);
            model.TextChanged = false;
        }
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
                    Patterns = new string[] { "skprompt.txt" }
                }
            }
        });
        if (file.Count > 0)
        {
            using var stream = await file[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            var text = await streamReader.ReadToEndAsync();
            model.Document = new TextDocument(new StringTextSource(text))
            {
                FileName = file[0].TryGetLocalPath()
            };
        }
    }
}
