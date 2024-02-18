using LLama.Common;
using LLama;
using LLamaSharp.SemanticKernel.TextCompletion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextGeneration;
using PromptPlayground.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels.ConfigViewModels.LLM
{
    internal class LlamaSharpConfigViewModel : ConfigViewModelBase, ILLMConfigViewModel
    {
        private string? _modelPath;
        private LLamaWeights? _model;
        public LlamaSharpConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
            RequireAttribute(ConfigAttribute.LlamaModelPath);
        }
        public override string Name => "LlamaSharp";

        public IKernelBuilder CreateKernelBuilder()
        {
            var modelPath = this.GetAttribute(ConfigAttribute.LlamaModelPath);
            var parameters = new ModelParams(modelPath);

            if (_model is null || _modelPath != modelPath)
            {
                _model?.Dispose();
                _modelPath = modelPath;
                _model = LLamaWeights.LoadFromFile(parameters);
            }

            var ex = new StatelessExecutor(_model, parameters);

            var builder = Kernel.CreateBuilder();
            builder.Services.AddKeyedSingleton<ITextGenerationService>("local-llama", new LLamaSharpTextCompletion(ex));

            return builder;
        }

        public ResultTokenUsage? GetUsage(FunctionResult resultModel)
        {
            return null;
        }

    }
}
