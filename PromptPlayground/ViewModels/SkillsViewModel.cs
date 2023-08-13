using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft;
using PromptPlayground.Messages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels
{
    public partial class SkillsViewModel : ObservableRecipient, IRecipient<SkillOpenMessage>,
        IRecipient<FunctionOpenMessage>,
        IRecipient<FunctionCreateMessage>,
        IRecipient<FunctionChangedMessage>
    {
        public SkillsViewModel()
        {

            UnsavedFunctions = new SkillViewModel("opened");
            Skills = new ObservableCollection<SkillViewModel>
            {
                UnsavedFunctions
            };
            IsActive = true;
        }

        [RelayCommand]
        public void CloseSkillFolder(SkillViewModel model)
        {
            if (this.Skills.Contains(model))
            {
                this.Skills.Remove(model);
            }
        }

        [RelayCommand]
        public void FunctionSelected(SemanticFunctionViewModel viewModel)
        {
            WeakReferenceMessenger.Default.Send(new FunctionSelectedMessage(viewModel));
        }

        public void Receive(SkillOpenMessage message)
        {
            if (Directory.Exists(message.Folder))
            {
                var skill = new SkillViewModel(message.Folder);
                if (!Skills.Contains(skill))
                {
                    Skills.Add(skill);
                }
            }
        }

        public void Receive(FunctionOpenMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.FilePath) || !Directory.Exists(message.FilePath))
            {
                return;
            }

            var function = new SemanticFunctionViewModel(message.FilePath);
            if (!UnsavedFunctions.Functions.Contains(function))
            {
                UnsavedFunctions.Functions.Add(function);
                FunctionSelected(function);
            }
        }

        public void Receive(FunctionCreateMessage message)
        {
            var function = new SemanticFunctionViewModel("[New Function]");
            this.UnsavedFunctions.Functions.Add(function);

            FunctionSelected(function);
        }

        public void Receive(FunctionChangedMessage message)
        {
            if (!UnsavedFunctions.Functions.Contains(message.Function))
            {
                UnsavedFunctions.Functions.Add(message.Function);
            }
        }

        public SkillViewModel UnsavedFunctions { get; set; }

        public ObservableCollection<SkillViewModel> Skills { get; set; }

    }
}
