using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;
using Moq;
using PromptPlayground.Messages;
using PromptPlayground.Services;
using PromptPlayground.Services.TemplateEngine;
using PromptPlayground.Services.TemplateEngine.Abstractions.Blocks;
using PromptPlayground.Services.TemplateEngine.Blocks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels
{
    public partial class SemanticFunctionViewModel : ViewModelBase, IEquatable<SemanticFunctionViewModel>
    {
        #region static
        static PromptTemplateEngine _engine = new();
        static string DefaultConfig()
        {
            return JsonSerializer.Serialize(new PromptTemplateConfig(), new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        }

        private static bool HasVariable(Block block)
        {
            return block is VarBlock || (block is CodeBlock code && code.HasVar());
        }

        private static string GetBlockContent(Block block)
        {
            if (block is VarBlock var)
            {
                return var.Content;
            }
            else if (block is CodeBlock code)
            {
                return code.VarContent();
            }
            return string.Empty;
        }

        public static SemanticFunctionViewModel Create(string folder) => new(folder);

        #endregion


        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private bool isChanged;

        [ObservableProperty]
        private bool isGenerating;

        public SemanticFunctionViewModel(string folderOrName)
        {
            if (Path.GetFileName(folderOrName) == Constants.SkPrompt)
            {
                folderOrName = Path.GetDirectoryName(folderOrName)!;
            }

            if (Directory.Exists(folderOrName))
            {
                this.Folder = folderOrName;
                Name = Path.GetFileName(folderOrName);
                var promptPath = Path.Combine(folderOrName, Constants.SkPrompt);
                var configPath = Path.Combine(folderOrName, Constants.SkConfig);
                this.Prompt = File.ReadAllText(promptPath);
                this.Config = File.ReadAllText(configPath);
                this.IsChanged = false;
            }
            else
            {
                this.Folder = "";
                this.Name = folderOrName;
                this.Prompt = "";
                this.Config = DefaultConfig();
                this.IsChanged = true;
            }

        }

        [ObservableProperty]
        public string prompt;
        [ObservableProperty]
        public string config;
        [ObservableProperty]
        private string folder = string.Empty;
        [ObservableProperty]
        private string name = string.Empty;

        partial void OnPromptChanged(string value)
        {
            IsChanged = true;
        }

        partial void OnConfigChanged(string value)
        {
            IsChanged = true;
        }
        partial void OnIsGeneratingChanged(bool value)
        {
            WeakReferenceMessenger.Default.Send(new LoadingStatus(value));
        }

        public bool Equals(SemanticFunctionViewModel? other)
        {
            if (ReferenceEquals(this, other)) return true;
            return other?.Folder == this.Folder && Directory.Exists(this.Folder);
        }

        private PromptTemplateConfig PromptConfig => PromptTemplateConfig.FromJson(Config);

        public IList<string> Blocks => _engine.ExtractBlocks(this.Prompt)
                                        .Where(HasVariable)
                                        .Select(GetBlockContent)
                                        .ToList();

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(IsChanged))]
        public async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(this.Folder) || !Directory.Exists(this.Folder))
            {
                var response = await WeakReferenceMessenger.Default.Send(new AsyncRequestMessage<FolderOpenMessage>());
                if (response == null || string.IsNullOrWhiteSpace(response.Folder))
                {
                    return;
                }

                var _promptPath = Path.Combine(response.Folder, Constants.SkPrompt);

                if (File.Exists(_promptPath))
                {
                    var confirm = await WeakReferenceMessenger.Default.Send(new ConfirmRequestMessage("file overwrite", "The prompt file already exists, do you want to overwrite it?"));
                    if (!confirm)
                    {
                        return;
                    }
                }

                this.Folder = response.Folder;
                this.Name = Path.GetFileName(response.Folder);
            }

            var promptPath = Path.Combine(this.Folder, Constants.SkPrompt);
            var configPath = Path.Combine(this.Folder, Constants.SkConfig);
            await File.WriteAllTextAsync(promptPath, this.Prompt);
            await File.WriteAllTextAsync(configPath, this.Config);

            this.IsChanged = false;

            WeakReferenceMessenger.Default.Send(new NotificationMessage("Saved!", this.Folder, NotificationMessage.NotificationType.Success));
        }


        [RelayCommand(AllowConcurrentExecutions = false, IncludeCancelCommand = true)]
        public async Task GenerateResultAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.IsGenerating = true;
                Results.Clear();
                await Task.Delay(2, cancellationToken);
                for (int i = 0; i < GetMaxCount(); i++)
                {
                    Results.Add(new GenerateResult()
                    {
                        Text = "Text",
                        Elapsed = new TimeSpan(0, 0, 1, 10, 99),
                        TokenUsage = new ResultTokenUsage(10, 1, 1)
                    });
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                this.IsGenerating = false;
            }
        }

        private int GetMaxCount()
        {
            var result = WeakReferenceMessenger.Default.Send(new ResultCountRequestMessage());
            return result.Response;
        }

        public ObservableCollection<GenerateResult> Results { get; set; } = new();

    }
}
