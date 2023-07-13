using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
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

        public async Task<string> RunAsync(string prompt, PromptTemplateConfig config, SKContext context, CancellationToken cancellationToken = default)
        {
            var func = _kernel.CreateSemanticFunction(prompt, config);
            var result = await func.InvokeAsync(context);
            return result.Result;
        }
    }
}
