using PromptPlayground.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.Views.Args
{
    public class GenerateResultArgs
    {
        public GenerateResultArgs(GenerateResult result)
        {
            Result = result;
        }

        public GenerateResult Result { get; }
    }
}
