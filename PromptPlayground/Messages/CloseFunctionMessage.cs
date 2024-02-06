using PromptPlayground.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.Messages
{
    public class CloseFunctionMessage
    {
        public CloseFunctionMessage(SemanticPluginViewModel function)
        {
            Function = function;
        }

        public SemanticPluginViewModel Function { get; }
    }
}
