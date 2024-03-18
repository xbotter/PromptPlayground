using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PromptPlayground.ViewModels
{
    public class VariablesViewModel : ViewModelBase
    {
        static readonly Dictionary<string, string> _variablesCache = [];

        /// <summary>
        /// only for design time
        /// </summary>
        public VariablesViewModel()
        {
            Variables = new ObservableCollection<Variable>([.. new Faker<Variable>()
                .RuleFor(x => x.Name, f => f.Lorem.Word())
                .Generate(5)]);
        }

        public VariablesViewModel(List<Variable> variables)
        {
            Variables = new ObservableCollection<Variable>(variables);

            foreach (var var in Variables)
            {
                if (_variablesCache.TryGetValue(var.Name, out string? value))
                {
                    var.Value = value;
                }
                else if (string.IsNullOrWhiteSpace(var.Value) && !string.IsNullOrWhiteSpace(var.DefaultValue))
                {
                    var.Value = var.DefaultValue;
                }
            }
        }
        public bool IsCanceled { get; set; }
        public ObservableCollection<Variable> Variables { get; set; }

        public bool Configured()
        {
            foreach (var var in Variables)
            {
                if (var.IsRequired && string.IsNullOrWhiteSpace(var.Value))
                {
                    return false;
                }
                if (var.Value != var.DefaultValue)
                {
                    _variablesCache[var.Name] = var.Value;
                }
            }
            return true;
        }
    }
    public class Variable : ObservableObject
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? DefaultValue { get; set; }
        public bool IsRequired { get; set; } = false;
    }
}
