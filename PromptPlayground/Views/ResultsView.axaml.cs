using Avalonia.Controls;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;

namespace PromptPlayground.Views;

public partial class ResultsView : UserControl
{
    private ResultsViewModel model => (this.DataContext as ResultsViewModel)!;
    public ResultsView()
    {
        InitializeComponent();
    }

    public void Clear()
    {
        this.model.Results.Clear();
    }
    public void AddResult(GenerateResult result)
    {
        this.model.Results.Add(result);
    }

    public int GetCount()
    {
        return this.model.Results.Count;
    }

}