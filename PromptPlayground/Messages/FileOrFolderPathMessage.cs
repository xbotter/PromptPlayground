﻿using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.Messages
{
    public abstract class FileOrFolderPathMessage
    {
        public string? Path { get; set; }
        public FileOrFolderPathMessage(string? path)
        {
            Path = path;
        }
    }

    public class RequestFileOpen : AsyncRequestMessage<string?>
    {

    }
    public class RequestFolderOpen : AsyncRequestMessage<string?>
    {

    }
}
