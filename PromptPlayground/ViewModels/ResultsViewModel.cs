using PromptPlayground.Services;
using System.Collections.ObjectModel;

namespace PromptPlayground.ViewModels
{
    public class ResultsViewModel : ViewModelBase
    {

        public ResultsViewModel()
        {
            Results = new ObservableCollection<GenerateResult>();
            Results.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(Results));
            };
        }
        public ObservableCollection<GenerateResult> Results { get; set; } 
    }
}
