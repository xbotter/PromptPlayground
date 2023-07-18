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
using System;

namespace PromptPlayground.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {

    }

    public string StatusBar { get => statusBar; set => SetProperty(ref statusBar, value); }
    public bool Loading
    {
        get => loading; set => SetProperty(ref loading, value);
    }


    public ConfigViewModel Config = new(true);


    private bool loading = false;
    private string statusBar = string.Empty;
}


