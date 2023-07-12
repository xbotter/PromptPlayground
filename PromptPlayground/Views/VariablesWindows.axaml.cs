using Avalonia.Controls;
using Avalonia.Interactivity;
using PromptPlayground.ViewModels;

namespace PromptPlayground.Views;

public partial class VariablesWindows : Window
{
    public bool Canceled { get; set; }
    public VariablesWindows()
    {
        InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!Canceled)
        {
            if (!(this.DataContext as VariablesViewModel)!.Configured())
            {
                e.Cancel = true;
            }
        }
    }

    public void OnCanceledClick(object sender, RoutedEventArgs e)
    {
        Canceled = true;
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