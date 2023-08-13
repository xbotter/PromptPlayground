using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.Messages
{
    public class FileOpenMessage
    {
        public string? FilePath { get; set; }
        public FileOpenMessage(string? filePath)
        {
            FilePath = filePath;
        }
    }
}
