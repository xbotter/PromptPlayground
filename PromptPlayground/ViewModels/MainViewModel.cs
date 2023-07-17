using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.SemanticKernel.TemplateEngine.Blocks;
using Microsoft.SemanticKernel.TemplateEngine;
using System.Reflection;
using System.Linq;
using AvaloniaEdit.Document;
using System.IO;
using Microsoft.SemanticKernel.SemanticFunctions;
using PromptPlayground.Services;

namespace PromptPlayground.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        Results = new ObservableCollection<GenerateResult>();
    }

    public string StatusBar
    {
        get
        {
            if (Loading)
            {
                return $"({Results.Count}/{Config.MaxCount})生成中 ......";
            }
            return string.Empty;

        }
    }
    public bool Loading
    {
        get => loading; set
        {
            if (loading != value)
            {
                loading = value;
                OnPropertyChanged(nameof(Loading));
                OnPropertyChanged(nameof(StatusBar));
            }
        }
    }

    public ObservableCollection<GenerateResult> Results { get; set; }
    public ConfigViewModel Config = new(true);


    private bool loading = false;

}


