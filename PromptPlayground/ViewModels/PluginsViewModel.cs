using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft;
using PromptPlayground.Messages;
using PromptPlayground.Services;
using PromptPlayground.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels
{
    public partial class PluginsViewModel : ObservableRecipient,
        IRecipient<PluginOpenMessage>,
        IRecipient<PluginCloseMessage>,
        IRecipient<FunctionOpenMessage>,
        IRecipient<FunctionCreateMessage>,
        IRecipient<FunctionSavedMessage>,
        IRecipient<RequestMessage<List<PluginViewModel>>>
    {
        private readonly ProfileService<List<string>> profile;
        const string DefaultPlugin = "·______·";

        public PluginsViewModel()
        {
            this.profile = new ProfileService<List<string>>("openedPlugins.json");
            Plugins = new ObservableCollection<PluginViewModel>();
            OpenedPlugin = new PluginViewModel(DefaultPlugin);
            Plugins.Add(OpenedPlugin);

            var plugins = profile.Get();
            if (plugins != null)
            {
                foreach (var plugin in plugins)
                {
                    if (Directory.Exists(plugin))
                    {
                        Plugins.Add(new PluginViewModel(plugin));
                    }
                }
            }
            IsActive = true;
        }

        [RelayCommand]
        public void FunctionSelected(SemanticFunctionViewModel viewModel)
        {
            WeakReferenceMessenger.Default.Send(new FunctionSelectedMessage(viewModel));
        }


        public void Receive(PluginOpenMessage message)
        {
            if (Directory.Exists(message.Path))
            {
                var plugin = new PluginViewModel(message.Path);
                if (!Plugins.Contains(plugin))
                {
                    Plugins.Add(plugin);
                    this.profile.Save(Plugins
                        .Where(_ => _.Folder != DefaultPlugin)
                        .Select(_ => _.Folder!)
                        .ToList());
                }
            }
        }

        public void Receive(PluginCloseMessage message)
        {
            if (Directory.Exists(message.Path))
            {
                var plugin = Plugins.FirstOrDefault(_ => _.Folder == message.Path);
                if (plugin != null)
                {
                    Plugins.Remove(plugin);
                    this.profile.Save(Plugins
                        .Where(_ => _.Folder != DefaultPlugin)
                        .Select(_ => _.Folder!)
                        .ToList());
                }
            }
        }

        public void Receive(FunctionOpenMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Path) || (!Directory.Exists(message.Path) && !File.Exists(message.Path)))
            {
                return;
            }

            var function = new SemanticFunctionViewModel(message.Path);
            if (!OpenedPlugin.Functions.Contains(function))
            {
                OpenedPlugin.Functions.Add(function);
                FunctionSelected(function);
            }
        }

        public void Receive(FunctionCreateMessage message)
        {
            var function = message.Function ?? new SemanticFunctionViewModel("");
            this.OpenedPlugin.AddNewFunction(function);

            FunctionSelected(function);
        }

        public void Receive(FunctionSavedMessage message)
        {
            var defaultPlugin = Plugins.FirstOrDefault(_ => _.Folder is null);
            if (defaultPlugin != null)
            {
                var function = defaultPlugin.Functions.FirstOrDefault(_ => _.Folder == message.Path);

                var parent = Path.GetDirectoryName(message.Path);

                var plugin = Plugins.FirstOrDefault(_ => _.Folder == parent);

                if (plugin != null && function != null)
                {
                    defaultPlugin.Functions.Remove(function);
                    plugin.Functions.Add(function);
                    WeakReferenceMessenger.Default.Send(new FunctionSelectedMessage(function));
                }
            }
        }

		public void Receive(RequestMessage<List<PluginViewModel>> message)
		{
            message.Reply(Plugins.ToList());
		}

		public PluginViewModel OpenedPlugin { get; set; }

        public ObservableCollection<PluginViewModel> Plugins { get; set; }

    }
}
