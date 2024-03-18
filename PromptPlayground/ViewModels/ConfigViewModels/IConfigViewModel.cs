using Microsoft.SemanticKernel;
using PromptPlayground.Services;
using PromptPlayground.ViewModels.ConfigViewModels.LLM;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace PromptPlayground.ViewModels.ConfigViewModels
{
    public interface IConfigViewModel
    {
        string Name { get; }
        public IList<ConfigAttribute> SelectAttributes(IList<ConfigAttribute> allAttributes);
    }

    public abstract class ConfigViewModelBase : ViewModelBase, IConfigViewModel
    {
        protected readonly IConfigAttributesProvider provider;
        private List<string> _requiredAttributes = [];

        public ConfigViewModelBase(IConfigAttributesProvider provider)
        {
            this.provider = provider;
        }
        public abstract string Name { get; }

        public IList<ConfigAttribute> SelectAttributes(IList<ConfigAttribute> allAttributes)
        {
            return allAttributes.Where(_ => _requiredAttributes.Contains(_.Name)).ToList();
        }

        protected void RequireAttribute(string attribute)
        {
            if (!_requiredAttributes.Contains(attribute))
            {
                _requiredAttributes.Add(attribute);
            }
        }
        protected string GetAttribute(string name)
        {
            return provider.AllAttributes.FirstOrDefault(_ => _.Name == name)?.Value ?? string.Empty;
        }
    }


    public interface IConfigAttributesProvider
    {
        IList<ConfigAttribute> AllAttributes { get; }
        ILLMConfigViewModel GetLLM();
    }
}
