using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels
{
    public partial class StatusViewModel : ObservableRecipient, IRecipient<ValueChangedMessage<string>>, IRecipient<LoadingStatus>
    {
        [ObservableProperty]
        public string text = string.Empty;

        [ObservableProperty]
        private bool loading = false;

        public StatusViewModel()
        {
            this.IsActive = true;

            WeakReferenceMessenger.Default.Register<ValueChangedMessage<string>, string>(this, "Status");
        }

        public void Receive(ValueChangedMessage<string> message)
        {

        }

        public void Receive(LoadingStatus message)
        {
            this.Loading = message.IsRunning;
        }
    }
}
public class LoadingStatus
{
    public LoadingStatus(bool isRunning)
    {
        IsRunning = isRunning;
    }

    public bool IsRunning { get; set; }
}

