﻿using Azure.AI.OpenAI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using PromptPlayground.Messages;
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

public partial class ConfigViewModel : ViewModelBase, IConfigAttributesProvider,
                                        IRecipient<ResultCountRequestMessage>,
                                        IRecipient<RequestMessage<IConfigAttributesProvider>>
{
    private string[] RequiredAttributes = new string[]
   {
#region LLM Config
        ConfigAttribute.AzureDeployment,
        ConfigAttribute.AzureEndpoint,
        ConfigAttribute.AzureSecret,
        ConfigAttribute.BaiduClientId,
        ConfigAttribute.BaiduSecret,
        ConfigAttribute.BaiduModel,
        ConfigAttribute.OpenAIApiKey,
        ConfigAttribute.OpenAIModel,
        ConfigAttribute.DashScopeApiKey,
        ConfigAttribute.DashScopeModel,
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

    public int MaxCount { get => maxCount; set => SetProperty(ref maxCount, value); }
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
                OnPropertyChanged(nameof(SelectedModel.Name));
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
    private readonly List<ILLMConfigViewModel> LLMs = new();
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

    private List<IVectorDbConfigViewModel> VectorDbs = new();

    [JsonIgnore]
    public ObservableCollection<ConfigAttribute> VectorDbAttributes => SelectedVectorDb.SelectAttributes(this.AllAttributes);

    #endregion

    #region Embedding 
    private int embeddingSelectedIndex = 0;
    private int maxCount = 3;

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

    private readonly List<IEmbeddingConfigViewModel> Embeddings = new();

    [JsonIgnore]
    public ObservableCollection<ConfigAttribute> EmbeddingAttributes => SelectedEmbedding.SelectAttributes(this.AllAttributes);


    #endregion
    IList<ConfigAttribute> IConfigAttributesProvider.AllAttributes => this.AllAttributes;
    public ILLMConfigViewModel GetLLM()
    {
        return this.SelectedModel;
    }

    public IVectorDbConfigViewModel? GetVectorDb()
    {
        return this.EnableVectorDB ? this.SelectedVectorDb : null;
    }

    public IEmbeddingConfigViewModel? GetEmbedding()
    {
        return this.EnableVectorDB ? this.SelectedEmbedding : null;
    }

    public ConfigViewModel(bool requireLoadConfig = false) : this()
    {
        if (requireLoadConfig)
        {
            WeakReferenceMessenger.Default.RegisterAll(this);
            LoadConfigFromUserProfile();
        }
    }

    public ConfigViewModel()
    {
        this.AllAttributes = CheckAttributes(this.AllAttributes);

        LLMs.Add(new AzureOpenAIConfigViewModel(this));
        LLMs.Add(new BaiduConfigViewModel(this));
        LLMs.Add(new OpenAIConfigViewModel(this));
        LLMs.Add(new DashScopeConfigViewModel(this));

        Embeddings.Add(new AzureOpenAIEmbeddingConfigViewModel(this));

        VectorDbs.Add(new QdrantConfigViewModel(this));


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

    public void Receive(ResultCountRequestMessage message)
    {
        message.Reply(this.MaxCount);
    }

    public void Receive(RequestMessage<IConfigAttributesProvider> message)
    {
        message.Reply(this);
    }
}
