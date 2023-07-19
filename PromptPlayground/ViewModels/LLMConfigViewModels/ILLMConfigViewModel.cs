using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.SemanticKernel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PromptPlayground.ViewModels.LLMConfigViewModels
{
    public interface ILLMConfigViewModel
    {
        string Name { get; }
        public IKernel CreateKernel();
        public ObservableCollection<ConfigAttribute> SelectAttributes(List<ConfigAttribute> allAttributes);
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
    }

    public interface IConfigAttributesProvider
    {
        IList<ConfigAttribute> AllAttributes { get; }
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

        #region Constants
        public const string AzureDeployment = nameof(AzureDeployment);
        public const string AzureEndpoint = nameof(AzureEndpoint);
        public const string AzureSecret = nameof(AzureSecret);

        public const string BaiduClientId = nameof(BaiduClientId);
        public const string BaiduSecret = nameof(BaiduSecret);
        #endregion
    }


}
