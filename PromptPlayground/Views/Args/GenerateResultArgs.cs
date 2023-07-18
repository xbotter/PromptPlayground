using PromptPlayground.Services;

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
