using PromptPlayground.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
