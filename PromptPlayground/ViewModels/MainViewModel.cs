
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Avalonia.Controls;
using PromptPlayground.Views;
using Avalonia;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

namespace PromptPlayground.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
     

        Results = new ObservableCollection<string>(new List<string>() { });

    }
    public bool Loading
    {
        get => loading; set
        {
            if (loading != value)
            {
                loading = value;
                OnPropertyChanged(nameof(Loading));
            }
        }
    }
    public string Prompt { get; set; } = string.Empty;
    
    public ObservableCollection<string> Results { get; set; }
  

    public string Status
    {
        get
        {
            if (loading)
            {
                return "loading";
            }
            else
            {
                return "ready";
            }
        }
    }
    public ConfigViewModel Config = new(true);
    private bool loading = false;
}


