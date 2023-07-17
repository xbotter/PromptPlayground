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
        public ResultsViewModel(ObservableCollection<GenerateResult> results)
        {
            Results = results;
            Results.CollectionChanged += Results_CollectionChanged;
        }

        private void Results_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Results));
        }

        public ObservableCollection<GenerateResult> Results { get; set; }
    }
}
