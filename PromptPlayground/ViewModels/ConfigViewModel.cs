using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels;

public partial class ConfigViewModel : ViewModelBase
{
    private int modelSelectedIndex = 0;

    public string AzureDeployment { get; set; } = string.Empty;
    public string AzureEndpoint { get; set; } = string.Empty;
    public string AzureSecret { get; set; } = string.Empty;
    public string BaiduClientId { get; set; } = string.Empty;
    public string BaiduSecret { get; set; } = string.Empty;
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
                OnPropertyChanged(nameof(ModelSelectAzure));
                OnPropertyChanged(nameof(ModelSelectBaidu));
            }
        }
    }
    public string SelectedModel => Models[ModelSelectedIndex];

    public bool ModelSelectAzure => SelectedModel == "Azure";
    public bool ModelSelectBaidu => SelectedModel == "Baidu";



    public List<string> Models => new()
    {
         "Azure",
         "Baidu"
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
                this.AzureDeployment = vm.AzureDeployment;
                this.AzureEndpoint = vm.AzureEndpoint;
                this.AzureSecret = vm.AzureSecret;
                this.MaxCount = vm.MaxCount;
                this.BaiduClientId = vm.BaiduClientId;
                this.BaiduSecret = vm.BaiduSecret;
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
