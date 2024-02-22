using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PromptPlayground;

public partial class HistoryWindow : Window
{
    public SemanticFunctionViewModel Function { get; set; }
    public HistoryWindow(SemanticFunctionViewModel function)
    {
        this.Function = function;
        InitializeComponent();
        LoadHistory();
    }

    private async void LoadHistory()
    {
        if (string.IsNullOrWhiteSpace(Function.Folder))
        {
            return;
        }
        var db = DbStore.NewScoped;
        this.DataContext = await db.GenerationResultStores.Where(_ => _.FunctionPath.Equals(Function.Folder))
             .OrderByDescending(_ => _.CreatedAt)
             .ToListAsync();
    }

}