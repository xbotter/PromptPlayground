using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PromptPlayground.Services
{
    public class PromptService
    {
        private readonly IKernel _kernel;

        public PromptService(IKernel kernel)
        {
            this._kernel = kernel;
        }

        public async Task<string> RunAsync(string prompt, SKContext context, CancellationToken cancellationToken = default)
        {
            var func = _kernel.CreateSemanticFunction(prompt, maxTokens: 2000);
            var result = await func.InvokeAsync(context);
            return result.Result;
        }
    }
}
