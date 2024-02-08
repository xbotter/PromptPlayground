using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using PromptPlayground.Messages;
using PromptPlayground.Services;
using System.Collections.ObjectModel;

namespace PromptPlayground.ViewModels
{
    public class ResultsViewModel : ObservableRecipient, IRecipient<FunctionSelectedMessage>
    {
        private SemanticFunctionViewModel function;
        public ObservableCollection<GenerateResult> Results => function.Results;
        public AverageResult AverageResult => function.Average;

        public ResultsViewModel(SemanticFunctionViewModel function)
        {
            this.function = function;
            IsActive = true;
        }

        public void Receive(FunctionSelectedMessage message)
        {
            this.function = message.Function;
            OnPropertyChanged(nameof(Results));
        }
    }
}
