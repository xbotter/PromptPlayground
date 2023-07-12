using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels
{
    public class VariablesViewModel : ViewModelBase
    {
        public VariablesViewModel(List<Variable> variables)
        {
            Variables = new ObservableCollection<Variable>(variables);
        }
        public ObservableCollection<Variable> Variables { get; set; }

        public bool Configured()
        {
            return Variables.All(_ => !string.IsNullOrWhiteSpace(_.Value));
        }
    }
    public class Variable
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
