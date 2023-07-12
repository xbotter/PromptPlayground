
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
using AvaloniaEdit.Document;
using System.IO;

namespace PromptPlayground.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        Results = new ObservableCollection<string>(new List<string>() { });
        Results.CollectionChanged += Results_CollectionChanged;
    }

    private void Results_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(StatusBar));
    }

    public TextDocument Document
    {
        get => document;
        set
        {
            if (document != value)
            {
                document = value;
                textChanged = false;
                OnPropertyChanged(nameof(Document));
                OnPropertyChanged(nameof(Prompt));
                OnPropertyChanged(nameof(LinkDocument));
                OnPropertyChanged(nameof(StatusBar));
                value.TextChanged += (s, e) =>
                {
                    OnPropertyChanged(nameof(Prompt));
                    TextChanged = true;
                };
            }
        }
    }

    public bool LinkDocument => File.Exists(this.document.FileName);
    public bool TextChanged
    {
        get => textChanged;
        set
        {
            if (textChanged != value)
            {
                textChanged = value;
                OnPropertyChanged(nameof(TextChanged));
            }
        }
    }
    public string StatusBar
    {
        get
        {
            if (Loading)
            {
                return $"({Results.Count}/{Config.MaxCount})生成中 ......";
            }
            else if (LinkDocument)
            {
                return "open file: " + this.document.FileName;
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
    public string Prompt => document.Text;
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


    private TextDocument document = new();
    private bool textChanged;

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


