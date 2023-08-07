using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using PromptPlayground.Services;
using PromptPlayground.ViewModels.ConfigViewModels.Embedding;
using PromptPlayground.ViewModels.ConfigViewModels.LLM;
using PromptPlayground.ViewModels.ConfigViewModels.VectorDB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace PromptPlayground.ViewModels.ConfigViewModels
{
    public interface IConfigViewModel
    {
        string Name { get; }
        public ObservableCollection<ConfigAttribute> SelectAttributes(List<ConfigAttribute> allAttributes);
    }

    public abstract class ConfigViewModelBase : ViewModelBase, IConfigViewModel
    {
        protected readonly IConfigAttributesProvider provider;
        private List<string> _requiredAttributes = new();

        public ConfigViewModelBase(IConfigAttributesProvider provider)
        {
            this.provider = provider;
        }
        public abstract string Name { get; }

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


    public interface IConfigAttributesProvider
    {
        IList<ConfigAttribute> AllAttributes { get; }
        ILLMConfigViewModel GetLLM();
        IVectorDbConfigViewModel GetVectorDb();
        IEmbeddingConfigViewModel GetEmbedding();
    }

    public class ConfigAttribute : ObservableObject
    {
        public ConfigAttribute(string name)
        {
            Name = name;
        }
        private string _value = string.Empty;
        [JsonIgnore]
        public string HumanizeName => Name.Humanize();
        public string Type { get; set; } = "string";
        public string Name { get; set; } = string.Empty;
        public string Value { get => _value; set => SetProperty(ref _value, value, nameof(Value)); }

        #region Constants
        public const string AzureDeployment = nameof(AzureDeployment);
        public const string AzureEndpoint = nameof(AzureEndpoint);
        public const string AzureSecret = nameof(AzureSecret);
        public const string AzureEmbeddingDeployment = nameof(AzureEmbeddingDeployment);

        public const string BaiduClientId = nameof(BaiduClientId);
        public const string BaiduSecret = nameof(BaiduSecret);

        public const string QdrantEndpoint = nameof(QdrantEndpoint);
        public const string QdrantApiKey = nameof(QdrantApiKey);

        public const string VectorSize = nameof(VectorSize);
        #endregion
    }


}
