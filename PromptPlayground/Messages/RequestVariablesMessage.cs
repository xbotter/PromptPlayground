using CommunityToolkit.Mvvm.Messaging.Messages;
using PromptPlayground.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.Messages
{
    public class RequestVariablesMessage : AsyncRequestMessage<VariablesViewModel>
    {
        public RequestVariablesMessage(List<Variable> variables)
        {
            Variables = variables;
        }
        public List<Variable> Variables { get; set; }
    }
}
