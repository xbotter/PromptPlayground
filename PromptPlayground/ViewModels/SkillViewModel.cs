using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PromptPlayground.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PromptPlayground.ViewModels
{
    public partial class SkillViewModel : ObservableRecipient, IEquatable<SkillViewModel>, IRecipient<FunctionSelectedMessage>, IRecipient<CloseFunctionMessage>
    {
        private static bool IsFunctionDir(string folder) => File.Exists(Path.Combine(folder, Constants.SkPrompt));

        public bool Equals(SkillViewModel? other)
        {
            return other?.Folder == this.Folder;
        }

        public SkillViewModel(string folder)
        {
            if (Directory.Exists(folder))
            {
                this.Folder = folder;
                this.Title = Path.GetFileName(folder);
                var functions = Directory.GetDirectories(Folder)
                             .Where(IsFunctionDir)
                             .Select(SemanticFunctionViewModel.Create)
                             .ToList();

                Functions = new ObservableCollection<SemanticFunctionViewModel>(functions);
            }
            else
            {
                this.Title = folder;
                Functions = new ObservableCollection<SemanticFunctionViewModel>();
            }
            IsActive = true;
        }
        public string? Folder { get; set; }
        public string Title { get; set; }

        [ObservableProperty]
        private ObservableCollection<SemanticFunctionViewModel> functions;

        [ObservableProperty]
        private SemanticFunctionViewModel? selected;


        partial void OnSelectedChanged(SemanticFunctionViewModel? oldValue, SemanticFunctionViewModel? newValue)
        {
            if (newValue != null && oldValue != newValue)
            {
                WeakReferenceMessenger.Default.Send(new FunctionSelectedMessage(newValue));
            }
        }

        [RelayCommand(CanExecute = nameof(CanAddNewFunction))]
        public void AddNewFunction()
        {
            AddNewFunction(new SemanticFunctionViewModel(""));
        }

        private bool CanAddNewFunction()
        {
            return !Directory.Exists(this.Folder);
        }

        public void AddNewFunction(SemanticFunctionViewModel function)
        {
            if (string.IsNullOrWhiteSpace(function.Name))
            {
                function.Name = $"[{DefaultName} {FindLastFunctionIndex() + 1}]";
            }
            this.Functions.Add(function);
        }
        const string DefaultName = "New Function";
        readonly Regex DefaultNamePattern = new(@"\[New Function (?<index>\d+)\]", RegexOptions.Compiled);
        private int FindLastFunctionIndex()
        {
            return this.Functions.Where(_ => DefaultNamePattern.IsMatch(_.Name))
                 .Select(_ => DefaultNamePattern.Match(_.Name).Groups["index"])
                 .Select(_ => int.Parse(_.Value))
                 .Order()
                 .LastOrDefault();
        }

        public void Receive(FunctionSelectedMessage message)
        {
            if (message.Function != null)
            {
                if (message.Function != this.Selected)
                {
                    if (Functions.Contains(message.Function))
                    {
                        this.Selected = message.Function;
                    }
                    else
                    {
                        this.Selected = null;
                    }
                }
            }
        }

        public void Receive(CloseFunctionMessage message)
        {
            if (this.Functions.Contains(message.Function) && message.Function != this.Selected)
            {
                this.Functions.Remove(message.Function);
            }
        }
    }
}
