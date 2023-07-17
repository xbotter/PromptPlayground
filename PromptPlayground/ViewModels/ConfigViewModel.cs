using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using PromptPlayground.ViewModels.LLMConfigViewModels;

namespace PromptPlayground.ViewModels;

public partial class ConfigViewModel : ViewModelBase
{
    public AzureOpenAIConfigViewModel AzureConfig { get; set; } = new AzureOpenAIConfigViewModel();
    public BaiduTurboConfigViewModel BaiduTurboConfig { get; set; } = new BaiduTurboConfigViewModel();
    public BaiduConfigViewModel BaiduConfig { get; set; } = new BaiduConfigViewModel();

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
    public ObservableCollection<ConfigAttribute> Attributes => SelectedModel.Attributes;
    [JsonIgnore]
    public ILLMConfigViewModel SelectedModel => LLMs[ModelSelectedIndex];
    [JsonIgnore]
    public List<ILLMConfigViewModel> LLMs => new()
    {
        this.AzureConfig,
        this.BaiduTurboConfig,
        this.BaiduConfig
    };

    public ConfigViewModel(bool requireLoadConfig = false)
    {
        if (requireLoadConfig)
        {
            LoadConfigFromUserProfile();
        }
    }

    public ConfigViewModel()
    {

    }

    private void LoadConfigFromUserProfile()
    {
        var profile = GetConfigFilePath();
        if (File.Exists(profile))
        {
            var vm = JsonSerializer.Deserialize<ConfigViewModel>(File.ReadAllText(profile));
            if (vm != null)
            {
                this.AzureConfig = vm.AzureConfig;
                this.BaiduTurboConfig = vm.BaiduTurboConfig;
                this.BaiduConfig = vm.BaiduConfig;

                this.MaxCount = vm.MaxCount;
                this.ModelSelectedIndex = vm.ModelSelectedIndex;
            }
        }
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
}
