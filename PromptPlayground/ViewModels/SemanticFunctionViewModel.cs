using Avalonia;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.SemanticKernel;
using PromptPlayground.Messages;
using PromptPlayground.Services;

using PromptPlayground.ViewModels.ConfigViewModels;
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
        static string DefaultConfig()
        {
            return JsonSerializer.Serialize(new
            {
                schema = 1,
                description = "",
                template_format = "semantic-kernel",
                input_variables = new List<InputVariable>()
                {
                    new InputVariable()
                    {
                        Name = "input",
                        Default = "",
                        Description = "",
                        IsRequired = false
                    }
                },
                execution_settings = new
                {
                    @default = new { }
                }
            }, new JsonSerializerOptions()
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
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
            }
            else
            {
                this.Folder = "";
                this.Name = folderOrName;
                this.Prompt = "";
                this.Config = DefaultConfig();
            }
            this.IsChanged = false;
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

        public PromptTemplateConfig PromptConfig => PromptTemplateConfig.FromJson(Config);

        public List<InputVariable> InputVariables
        {
            get => PromptConfig.InputVariables;
            set => PromptConfig.InputVariables = value;
        }

        public IList<string> Blocks => PromptConfig.InputVariables.Select(_ => _.Name).ToList();

        [RelayCommand]
        public async Task CloseAsync()
        {
            if (this.IsChanged)
            {
                var confirm = await WeakReferenceMessenger.Default.Send(new ConfirmRequestMessage("File unsaved", $"the {Name} is unsaved, do you want to close it?"));
                if (!confirm)
                {
                    return;
                }
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new CloseFunctionMessage(this));
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(IsChanged))]
        public async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(this.Folder) || !Directory.Exists(this.Folder))
            {
                var response = await WeakReferenceMessenger.Default.Send(new RequestFolderOpen());
                if (string.IsNullOrWhiteSpace(response))
                {
                    return;
                }

                var _promptPath = Path.Combine(response, Constants.SkPrompt);

                if (File.Exists(_promptPath))
                {
                    var confirm = await WeakReferenceMessenger.Default.Send(new ConfirmRequestMessage("file overwrite", "The prompt file already exists, do you want to overwrite it?"));
                    if (!confirm)
                    {
                        return;
                    }
                }

                this.Folder = response;
                this.Name = Path.GetFileName(response);
            }

            var promptPath = Path.Combine(this.Folder, Constants.SkPrompt);
            var configPath = Path.Combine(this.Folder, Constants.SkConfig);
            await File.WriteAllTextAsync(promptPath, this.Prompt);
            await File.WriteAllTextAsync(configPath, this.Config);

            this.IsChanged = false;

            WeakReferenceMessenger.Default.Send(new NotificationMessage("Saved!", this.Folder, NotificationMessage.NotificationType.Success));
            WeakReferenceMessenger.Default.Send(new FunctionSavedMessage(this.Folder));
        }


        [RelayCommand(AllowConcurrentExecutions = false, IncludeCancelCommand = true)]
        public async Task GenerateResultAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.IsGenerating = true;
                Results.Clear();
                var configProvider = WeakReferenceMessenger.Default.Send(new RequestMessage<IConfigAttributesProvider>());
                var service = new PromptService(configProvider.Response);

                var arguments = service.CreateArguments();
                var varBlocks = this.InputVariables;
                if (varBlocks.Count > 0)
                {
                    var variables = varBlocks.Select(_ => new Variable()
                    {
                        Name = _.Name.TrimStart('$'),
                        DefaultValue = _.Default?.ToString(),
                        IsRequired = _.IsRequired
                    }).Distinct().ToList();

                    var result = await WeakReferenceMessenger.Default.Send(new RequestVariablesMessage(variables));

                    if (result.IsCanceled)
                    {
                        throw new Exception("生成已取消");
                    }
                    if (!result.Configured())
                    {
                        throw new Exception("变量未配置");
                    }

                    foreach (var variable in result.Variables)
                    {
                        arguments[variable.Name] = variable.Value;
                    }
                }

                var maxCount = GetMaxCount();
                var results = Enumerable.Range(0, maxCount)
                    .Select(_ => new GenerateResult()
                    {
                        Text = "🤖"
                    }).ToList();

                var tasks = results
                     .Select(async r =>
                     {
                         Results.Add(r);
                         if (GetRunStream())
                         {
                             var results = service.RunStreamAsync(Prompt, PromptConfig.TemplateFormat, PromptConfig.DefaultExecutionSettings, new KernelArguments(arguments), cancellationToken)
                                                 .ConfigureAwait(false);
                             await foreach (var result in results)
                             {
                                 r.PromptRendered = result.PromptRendered;
                                 r.Text = result.Text;
                                 r.Elapsed = result.Elapsed;
                                 r.Error = result.Error;
                                 r.TokenUsage = result.TokenUsage;
                             }
                         }
                         else
                         {
                             var result = await service.RunAsync(Prompt, PromptConfig.TemplateFormat, PromptConfig.DefaultExecutionSettings, new KernelArguments(arguments), cancellationToken);
                             r.PromptRendered = result.PromptRendered;
                             r.Text = result.Text;
                             r.Elapsed = result.Elapsed;
                             r.Error = result.Error;
                             r.TokenUsage = result.TokenUsage;
                         }
                     })
                     .ToList();

                await Task.WhenAll(tasks).ConfigureAwait(false);

                //var db = DbStore.NewScoped;

                //foreach (var result in results)
                //{
                //    if (!result.HasError)
                //    {
                //        db.GenerationResultStores
                //            .Add(new Services.Models.GenerationResultStore()
                //            {
                //                FunctionPath = this.Folder,
                //                Text = result.Text,
                //                RenderedPrompt = result.PromptRendered!,
                //                Usage = result.TokenUsage,
                //                CreatedAt = DateTime.Now,
                //                Elapsed = result.Elapsed.Value
                //            });
                //    }
                //}
                //await db.SaveChangesAsync();

                //Average.HasResults = true;
                //Average.Elapsed = TimeSpan.FromMilliseconds(Results.Where(_ => !_.HasError).Where(_ => _.Elapsed.HasValue).Average(_ => _.Elapsed!.Value.TotalMilliseconds));
                //Average.TokenUsage = new ResultTokenUsage(
                //        (int)Results.Where(_ => !_.HasError).Average(_ => _.TokenUsage?.Total ?? 0),
                //        (int)Results.Where(_ => !_.HasError).Average(_ => _.TokenUsage?.Prompt ?? 0),
                //        (int)Results.Where(_ => !_.HasError).Average(_ => _.TokenUsage?.Completion ?? 0)
                //        );
            }
            catch (OperationCanceledException ex)
            {
                WeakReferenceMessenger.Default.Send(new NotificationMessage("Canceled", ex.Message, NotificationMessage.NotificationType.Warning));
            }
            catch (Exception ex)
            {
                WeakReferenceMessenger.Default.Send(new NotificationMessage("Error", ex.Message, NotificationMessage.NotificationType.Warning));
            }
            finally
            {
                this.IsGenerating = false;
            }
        }



        private int GetMaxCount()
        {
            var result = WeakReferenceMessenger.Default.Send(new ConfigurationRequestMessage("MaxCount"));

            return int.Parse(result.Response);
        }

        private bool GetRunStream()
        {
            var result = WeakReferenceMessenger.Default.Send(new ConfigurationRequestMessage("RunStream"));

            return bool.Parse(result.Response);
        }

        public ObservableCollection<GenerateResult> Results { get; set; } = new();

        [ObservableProperty]
        private AverageResult average = new AverageResult();

    }
}
