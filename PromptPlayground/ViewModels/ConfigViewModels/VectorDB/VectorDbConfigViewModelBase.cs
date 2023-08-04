using PromptPlayground.ViewModels.ConfigViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels.ConfigViewModels.VectorDB
{
    public interface IVectorDbConfigViewModel : IConfigViewModel
    {
    }
    public class VectorDbConfigViewModelBase : ViewModelBase, IVectorDbConfigViewModel
    {
        private readonly IConfigAttributesProvider provider;
        private List<string> _requiredAttributes = new();

        public VectorDbConfigViewModelBase(IConfigAttributesProvider provider)
        {
            this.provider = provider;
        }
        public virtual string Name => throw new NotImplementedException();

        public ObservableCollection<ConfigAttribute> SelectAttributes(List<ConfigAttribute> allAttributes)
        {
            return new ObservableCollection<ConfigAttribute>(allAttributes.Where(_ => _requiredAttributes.Contains(_.Name)));
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
}
