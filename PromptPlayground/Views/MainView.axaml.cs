using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft;
using Microsoft.SemanticKernel;
using MsBox.Avalonia;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;
using System;
using System.Linq;
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

            var service = new PromptService(kernel);
            model.Results.Clear();
            for (int i = 0; i < model.Config.MaxCount; i++)
            {
                var result = await service.RunAsync(model.Prompt);
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
}
