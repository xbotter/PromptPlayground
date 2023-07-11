
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
using Microsoft.SemanticKernel.TemplateEngine.Blocks;
using Microsoft.SemanticKernel.TemplateEngine;
using System.Reflection;
using System.Linq;
using System.Reflection.Emit;

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
    public string Prompt
    {
        get => prompt; set
        {
            if (prompt != value)
            {
                prompt = value;
                OnPropertyChanged(nameof(Prompt));
                OnPropertyChanged(nameof(Blocks));
            }
        }
    }
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
    private string prompt = string.Empty;


    public IList<string> Blocks => new PromptTemplateEngine().ExtractBlocks(this.Prompt)
            .Where(IsVariableBlock)
            .Select(GetBlockContent)
            .ToList();

    private static bool IsVariableBlock(Block block)
    {
        return block.GetType().Name == "VarBlock";
       
    }

    private static string GetBlockContent(Block block)
    {
        return block.GetType().GetRuntimeProperties().First(_ => _.Name == "Content").GetValue(block) as string ?? string.Empty;
    }
}


