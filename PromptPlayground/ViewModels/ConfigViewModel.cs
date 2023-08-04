using Azure.AI.OpenAI;
using PromptPlayground.ViewModels.ConfigViewModels;
using PromptPlayground.ViewModels.ConfigViewModels.Embedding;
using PromptPlayground.ViewModels.ConfigViewModels.LLM;
using PromptPlayground.ViewModels.ConfigViewModels.VectorDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PromptPlayground.ViewModels;

public partial class ConfigViewModel : ViewModelBase, IConfigAttributesProvider
{
    private string[] RequiredAttributes = new string[]
   {
#region LLM Config
       ConfigAttribute.AzureDeployment,
        ConfigAttribute.AzureEndpoint,
        ConfigAttribute.AzureSecret,
        ConfigAttribute.BaiduClientId,
        ConfigAttribute.BaiduSecret,
#endregion
#region Vector DB
        ConfigAttribute.VectorSize,
        ConfigAttribute.QdrantEndpoint,
        ConfigAttribute.QdrantApiKey,
#endregion
#region Embedding
        ConfigAttribute.AzureEmbeddingDeployment,
#endregion
   };

    public List<ConfigAttribute> AllAttributes { get; set; } = new();


    public int MaxCount { get; set; } = 3;
    #region Model
    private int modelSelectedIndex = 0;

    public int ModelSelectedIndex
    {
        get => modelSelectedIndex; set
        {
            if (modelSelectedIndex != value)
            {
                modelSelectedIndex = value;
                OnPropertyChanged(nameof(ModelSelectedIndex));
                OnPropertyChanged(nameof(SelectedModel));
                OnPropertyChanged(nameof(ModelAttributes));
            }
        }
    }

    [JsonIgnore]
    public List<string> ModelLists => LLMs.Select(_ => _.Name).ToList();
    [JsonIgnore]
    public ObservableCollection<ConfigAttribute> ModelAttributes => SelectedModel.SelectAttributes(this.AllAttributes);
    [JsonIgnore]
    public ILLMConfigViewModel SelectedModel => LLMs[ModelSelectedIndex];
    [JsonIgnore]
    public List<ILLMConfigViewModel> LLMs => new()
    {
        new AzureOpenAIConfigViewModel(this),
        new BaiduTurboConfigViewModel(this),
        new BaiduConfigViewModel(this)
    };
    #endregion

    #region vector DB
    private bool enableVectorDB;
    public bool EnableVectorDB { get => enableVectorDB; set => SetProperty(ref enableVectorDB, value); }

    private int vectorDbSelectedIndex = 0;
    public int VectorDbSelectedIndex
    {
        get => vectorDbSelectedIndex; set
        {
            if (vectorDbSelectedIndex != value)
            {
                vectorDbSelectedIndex = value;
                OnPropertyChanged(nameof(VectorDbSelectedIndex));
                OnPropertyChanged(nameof(SelectedVectorDb));
                OnPropertyChanged(nameof(VectorDbAttributes));
            }
        }
    }

    [JsonIgnore]
    public List<string> VectorDbLists => VectorDbs.Select(_ => _.Name).ToList();

    [JsonIgnore]
    public IVectorDbConfigViewModel SelectedVectorDb => VectorDbs[VectorDbSelectedIndex];

    [JsonIgnore]
    public List<IVectorDbConfigViewModel> VectorDbs => new()
    {
        new QdrantConfigViewModel(this)
    };

    [JsonIgnore]
    public ObservableCollection<ConfigAttribute> VectorDbAttributes => SelectedVectorDb.SelectAttributes(this.AllAttributes);

    #endregion

    #region Embedding 
    private int embeddingSelectedIndex = 0;
    public int EmbeddingSelectedIndex
    {
        get => embeddingSelectedIndex; set
        {
            if (embeddingSelectedIndex != value)
            {
                embeddingSelectedIndex = value;
                OnPropertyChanged(nameof(EmbeddingSelectedIndex));
                OnPropertyChanged(nameof(SelectedEmbedding));
                OnPropertyChanged(nameof(EmbeddingAttributes));
            }
        }
    }

    [JsonIgnore]
    public List<string> EmbeddingLists => Embeddings.Select(_ => _.Name).ToList();

    [JsonIgnore]
    public IEmbeddingConfigViewModel SelectedEmbedding => Embeddings[VectorDbSelectedIndex];

    [JsonIgnore]
    public List<IEmbeddingConfigViewModel> Embeddings => new()
    {
        new AzureOpenAIEmbeddingConfigViewModel(this)
    };

    [JsonIgnore]
    public ObservableCollection<ConfigAttribute> EmbeddingAttributes => SelectedEmbedding.SelectAttributes(this.AllAttributes);


    #endregion
    IList<ConfigAttribute> IConfigAttributesProvider.AllAttributes => this.AllAttributes;
    public ILLMConfigViewModel GetLLM()
    {
        return this.SelectedModel;
    }

    public IVectorDbConfigViewModel GetVectorDb()
    {
        return this.SelectedVectorDb;
    }

    public IEmbeddingConfigViewModel GetEmbedding()
    {
        return this.SelectedEmbedding;
    }

    public ConfigViewModel(bool requireLoadConfig = false) : this()
    {
        if (requireLoadConfig)
        {
            LoadConfigFromUserProfile();
        }
    }

    public ConfigViewModel()
    {
        this.AllAttributes = CheckAttributes(this.AllAttributes);
    }

    private void LoadConfigFromUserProfile()
    {
        var profile = GetConfigFilePath();
        if (File.Exists(profile))
        {
            var vm = JsonSerializer.Deserialize<ConfigViewModel>(File.ReadAllText(profile));
            if (vm != null)
            {
                this.AllAttributes = CheckAttributes(vm.AllAttributes);
                this.MaxCount = vm.MaxCount;
                this.EnableVectorDB = vm.EnableVectorDB;
                this.ModelSelectedIndex = vm.ModelSelectedIndex;
                this.VectorDbSelectedIndex = vm.VectorDbSelectedIndex;
                this.EmbeddingSelectedIndex = vm.EmbeddingSelectedIndex;
            }
        }
    }
    private List<ConfigAttribute> CheckAttributes(List<ConfigAttribute> list)
    {
        foreach (var item in RequiredAttributes)
        {
            if (!list.Any(_ => _.Name == item))
            {
                list.Add(new ConfigAttribute(item));
            }
        }
        return list;
    }


    private void SaveConfigToUserProfile()
    {
        var profile = GetConfigFilePath();
        File.WriteAllText(profile, JsonSerializer.Serialize(this));
    }

    private string GetConfigFilePath()
    {
        var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(profile, "PromptPlayground.config");
    }

    public void SaveConfig()
    {
        SaveConfigToUserProfile();
    }

    public void ReloadConfig()
    {
        this.LoadConfigFromUserProfile();
    }
}
