using Avalonia.Controls;
using Avalonia.Interactivity;
using PromptPlayground.ViewModels;

namespace PromptPlayground.Views;

public partial class VariablesWindows : Window
{
    private VariablesViewModel Model => (this.DataContext as VariablesViewModel)!;
    public VariablesWindows()
    {
        InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!this.Model.IsCanceled)
        {
            if (!e.IsProgrammatic)
            {
                this.Model.IsCanceled = true;
            }
        }
    }

    public void OnCanceledClick(object sender, RoutedEventArgs e)
    {
        this.Model.IsCanceled = true;
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