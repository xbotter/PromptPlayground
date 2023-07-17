using AvaloniaEdit.Document;
using Microsoft.SemanticKernel.TemplateEngine.Blocks;
using Microsoft.SemanticKernel.TemplateEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.SemanticKernel.SemanticFunctions;
using System.Collections.ObjectModel;
using PromptPlayground.Services;
using System.Text.Json;

namespace PromptPlayground.ViewModels
{
    public class SemanticFunctionViewModel : ViewModelBase
    {
        public SemanticFunctionViewModel(string folder)
        {
            if (Directory.Exists(folder))
            {
                this.Folder = folder;
                Name = Path.GetFileName(folder);
                this.Prompt = new TextDocument(new StringTextSource(File.ReadAllText(Path.Combine(folder, Constants.SkPrompt))));
                this.Config = new TextDocument(new StringTextSource(File.ReadAllText(Path.Combine(Folder, Constants.SkConfig))));
                this.IsChanged = false;
            }
            else
            {
                this.Folder = "";
                this.Prompt = new TextDocument(new StringTextSource(""));
                this.Config = new TextDocument(new StringTextSource(DefaultConfig()));
            }
        }
        private string DefaultConfig()
        {
            return JsonSerializer.Serialize(new PromptTemplateConfig(), new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        }
        public bool IsChanged { get; set; }
        public TextDocument Prompt { get; set; }
        public TextDocument Config { get; set; }
        public static SemanticFunctionViewModel Create(string folder) => new(folder);
        public string Folder { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public PromptTemplateConfig PromptConfig => PromptTemplateConfig.FromJson(Config.Text);

        public IList<string> Blocks => new PromptTemplateEngine().ExtractBlocks(this.Prompt.Text)
            .Where(IsVariableBlock)
            .Select(GetBlockContent)
            .ToList();

        private static bool IsVariableBlock(Block block)
        {
            return block.GetType().Name == "VarBlock";
        }

        private static string GetBlockContent(Block block)
        {
            return block.GetType().GetRuntimeProperties().First(_ => _.Name == "Content").GetValue(block) as string ?? string.Empty;
        }

        public bool IsGenerating { get; set; }
    }
}
