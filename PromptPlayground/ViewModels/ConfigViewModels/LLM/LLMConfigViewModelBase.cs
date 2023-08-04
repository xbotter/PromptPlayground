using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using PromptPlayground.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PromptPlayground.ViewModels.ConfigViewModels.LLM
{
    public interface ILLMConfigViewModel : IConfigViewModel
    {
        public IKernel CreateKernel();
        public ResultTokenUsage? GetUsage(ModelResult resultModel);
    }
    public abstract class LLMConfigViewModelBase : ViewModelBase, ILLMConfigViewModel
    {
        private readonly IConfigAttributesProvider attributesProvider;
        private List<string> _requiredAttributes = new();

        public LLMConfigViewModelBase(IConfigAttributesProvider attributesProvider)
        {
            this.attributesProvider = attributesProvider;
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
            return attributesProvider.AllAttributes.FirstOrDefault(_ => _.Name == name)?.Value ?? string.Empty;
        }
        public virtual string Name => throw new System.NotImplementedException();

        public virtual IKernel CreateKernel()
        {
            throw new System.NotImplementedException();
        }

        public ObservableCollection<ConfigAttribute> SelectAttributes(List<ConfigAttribute> allAttributes)
        {
            return new ObservableCollection<ConfigAttribute>(allAttributes.Where(_ => _requiredAttributes.Contains(_.Name)));
        }

        public virtual ResultTokenUsage? GetUsage(ModelResult result)
        {
            return null;
        }
    }
}
