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
        public CloseFunctionMessage(SemanticFunctionViewModel function)
        {
            Function = function;
        }

        public SemanticFunctionViewModel Function { get; }
    }
}
