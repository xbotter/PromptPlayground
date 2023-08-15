using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PromptPlayground.ViewModels
{
    public class VariablesViewModel : ViewModelBase
    {
        static Dictionary<string, string> VariablesCache = new Dictionary<string, string>();
        public VariablesViewModel(List<Variable> variables)
        {
            Variables = new ObservableCollection<Variable>(variables);

            foreach (var var in Variables)
            {
                if (VariablesCache.ContainsKey(var.Name))
                {
                    var.Value = VariablesCache[var.Name];
                }
            }
        }
        public bool IsCanceled { get; set; }
        public ObservableCollection<Variable> Variables { get; set; }

        public bool Configured()
        {
            foreach (var var in Variables)
            {
                if (string.IsNullOrWhiteSpace(var.Value))
                {
                    return false;
                }
                VariablesCache[var.Name] = var.Value;
            }
            return true;
        }
    }
    public class Variable : ObservableObject
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
