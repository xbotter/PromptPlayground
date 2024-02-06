using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PromptPlayground.ViewModels
{
	public class VariablesViewModel : ViewModelBase
	{
		static Dictionary<string, string> VariablesCache = new Dictionary<string, string>();

		/// <summary>
		/// only for design time
		/// </summary>
		public VariablesViewModel()
		{
			Variables = new ObservableCollection<Variable>(new Faker<Variable>()
				.RuleFor(x => x.Name, f => f.Lorem.Word())
				.Generate(5).ToList());
		}

		public VariablesViewModel(List<Variable> variables)
		{
			Variables = new ObservableCollection<Variable>(variables);

			foreach (var var in Variables)
			{
				if (VariablesCache.ContainsKey(var.Name))
				{
					var.Value = VariablesCache[var.Name];
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
		public string? DefaultValue { get; set; }
	}
}
