using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.Messages
{
    public class ConfirmRequestMessage : AsyncRequestMessage<bool>
    {
        public string Title { get; set; }
        public string? Message { get; set; }
        public ConfirmRequestMessage(string title, string? message)
        {
            Title = title;
            Message = message;
        }

    }
}
