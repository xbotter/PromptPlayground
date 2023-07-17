using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.SemanticKernel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PromptPlayground.ViewModels.LLMConfigViewModels
{
    public interface ILLMConfigViewModel
    {
        string Name { get; }
        ObservableCollection<ConfigAttribute> Attributes { get; }
        public IKernel CreateKernel();
    }

    public abstract class LLMConfigViewModelBase : ViewModelBase, ILLMConfigViewModel
    {
        protected void RequireAttribute(string attribute)
        {
            _attributes ??= new ObservableCollection<ConfigAttribute>();

            if (!_attributes.Any(_ => _.Name == attribute))
            {
                _attributes.Add(new ConfigAttribute(attribute));
            }
        }
        protected ObservableCollection<ConfigAttribute> _attributes;
        protected string GetAttribute(string name)
        {
            return _attributes.FirstOrDefault(_ => _.Name == name)?.Value ?? string.Empty;
        }
        public virtual string Name => throw new System.NotImplementedException();

        public ObservableCollection<ConfigAttribute> Attributes
        {
            get => _attributes;
            set => _attributes = value;
        }

        public virtual IKernel CreateKernel()
        {
            throw new System.NotImplementedException();
        }
        public LLMConfigViewModelBase()
        {
            _attributes ??= new ObservableCollection<ConfigAttribute>();
        }
    }

    public class ConfigAttribute : ObservableObject
    {
        public ConfigAttribute(string name)
        {
            Name = name;
        }
        private string _value = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get => _value; set => SetProperty(ref _value, value, nameof(Value)); }
    }

}
