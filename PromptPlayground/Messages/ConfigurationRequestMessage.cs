using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.Messages
{
    public class ConfigurationRequestMessage : RequestMessage<string>
    {
        public ConfigurationRequestMessage(string config)
        {
            Config = config;
        }

        public string Config { get; }
    }
}
