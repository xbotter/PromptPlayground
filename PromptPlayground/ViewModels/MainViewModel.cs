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


