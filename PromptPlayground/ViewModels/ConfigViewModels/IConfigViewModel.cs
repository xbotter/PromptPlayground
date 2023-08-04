using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using PromptPlayground.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PromptPlayground.ViewModels.ConfigViewModels
{
    public interface IConfigViewModel
    {
        string Name { get; }
        public ObservableCollection<ConfigAttribute> SelectAttributes(List<ConfigAttribute> allAttributes);
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
        [JsonIgnore]
        public string HumanizeName => Name.Humanize();
        public string Name { get; set; } = string.Empty;
        public string Value { get => _value; set => SetProperty(ref _value, value, nameof(Value)); }

        #region Constants
        public const string AzureDeployment = nameof(AzureDeployment);
        public const string AzureEndpoint = nameof(AzureEndpoint);
        public const string AzureSecret = nameof(AzureSecret);

        public const string BaiduClientId = nameof(BaiduClientId);
        public const string BaiduSecret = nameof(BaiduSecret);

        public const string QdrantEndpoint = nameof(QdrantEndpoint);
        public const string QdrantApiKey = nameof(QdrantApiKey);
        #endregion
    }


}
