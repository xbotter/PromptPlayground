using Avalonia.Controls;
using Avalonia.Interactivity;
using PromptPlayground.ViewModels;

namespace PromptPlayground.Views;

public partial class VariablesWindows : Window
{
    private VariablesViewModel model => (this.DataContext as VariablesViewModel)!;
    public VariablesWindows()
    {
        InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!this.model.IsCanceled)
        {
            if (!e.IsProgrammatic)
            {
                this.model.IsCanceled = true;
            }
        }
    }

    public void OnCanceledClick(object sender, RoutedEventArgs e)
    {
        this.model.IsCanceled = true;
        this.Close();
    }
    public void OnContinuedClick(object sender, RoutedEventArgs e)
    {
        if ((this.DataContext as VariablesViewModel)!.Configured())
        {
            this.Close();
        }
    }
}