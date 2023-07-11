using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextCompletion;
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

        public Task<string> RunAsync(string prompt, CancellationToken cancellationToken = default)
        {
            var completion = this._kernel.GetService<ITextCompletion>();

            var result = completion.CompleteAsync(prompt, new CompleteRequestSettings()
            {
                 MaxTokens = 2000,
            }, cancellationToken);

            return result;
        }
    }
}
