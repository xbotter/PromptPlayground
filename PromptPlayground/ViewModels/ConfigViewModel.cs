using PromptPlayground.ViewModels.LLMConfigViewModels;
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
    public List<ConfigAttribute> AllAttributes { get; set; } = new();

    private int modelSelectedIndex = 0;

    public int MaxCount { get; set; } = 3;
    public int ModelSelectedIndex
    {
        get => modelSelectedIndex; set
        {
            if (modelSelectedIndex != value)
            {
                modelSelectedIndex = value;
                OnPropertyChanged(nameof(ModelSelectedIndex));
                OnPropertyChanged(nameof(SelectedModel));
                OnPropertyChanged(nameof(Attributes));
            }
        }
    }

    [JsonIgnore]
    public List<string> ModelLists => LLMs.Select(_ => _.Name).ToList();
    [JsonIgnore]
    public ObservableCollection<ConfigAttribute> Attributes => SelectedModel.SelectAttributes(this.AllAttributes);

    [JsonIgnore]
    public ILLMConfigViewModel SelectedModel => LLMs[ModelSelectedIndex];
    [JsonIgnore]
    public List<ILLMConfigViewModel> LLMs => new()
    {
        new AzureOpenAIConfigViewModel(this),
        new BaiduTurboConfigViewModel(this),
        new BaiduConfigViewModel(this)
    };

    IList<ConfigAttribute> IConfigAttributesProvider.AllAttributes => this.AllAttributes;

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
                this.ModelSelectedIndex = vm.ModelSelectedIndex;
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
    private string[] RequiredAttributes = new string[]
    {
        ConfigAttribute.AzureDeployment,
        ConfigAttribute.AzureEndpoint,
        ConfigAttribute.AzureSecret,
        ConfigAttribute.BaiduClientId,
        ConfigAttribute.BaiduSecret,
    };

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
}
