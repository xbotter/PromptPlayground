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
        IRecipient<FunctionCreateMessage>
    {
        public SkillsViewModel()
        {

            OpenedFunctions = new SkillViewModel("opened");
            Skills = new ObservableCollection<SkillViewModel>
            {
                OpenedFunctions
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
            if (Directory.Exists(message.Path))
            {
                var skill = new SkillViewModel(message.Path);
                if (!Skills.Contains(skill))
                {
                    Skills.Add(skill);
                }
            }
        }

        public void Receive(FunctionOpenMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Path) || !Directory.Exists(message.Path))
            {
                return;
            }

            var function = new SemanticFunctionViewModel(message.Path);
            if (!OpenedFunctions.Functions.Contains(function))
            {
                OpenedFunctions.Functions.Add(function);
                FunctionSelected(function);
            }
        }

        public void Receive(FunctionCreateMessage message)
        {
            var function = message.Function ?? new SemanticFunctionViewModel("");
            this.OpenedFunctions.AddNewFunction(function);

            FunctionSelected(function);
        }

        public SkillViewModel OpenedFunctions { get; set; }

        public ObservableCollection<SkillViewModel> Skills { get; set; }

    }
}
