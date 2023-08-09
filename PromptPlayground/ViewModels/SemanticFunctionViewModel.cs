using AvaloniaEdit.Document;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;
using Moq;
using PromptPlayground.Services.TemplateEngine;
using PromptPlayground.Services.TemplateEngine.Abstractions.Blocks;
using PromptPlayground.Services.TemplateEngine.Blocks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels
{
    public class SemanticFunctionViewModel : ViewModelBase
    {
        static PromptTemplateEngine _engine = new();

        private bool isChanged;
        private bool isGenerating;

        public SemanticFunctionViewModel(string folder)
        {
            if (Directory.Exists(folder))
            {
                this.Folder = folder;
                Name = Path.GetFileName(folder);
                var promptPath = Path.Combine(folder, Constants.SkPrompt);
                var configPath = Path.Combine(folder, Constants.SkConfig);
                this.Prompt = new TextDocument(new StringTextSource(File.ReadAllText(promptPath)))
                {
                    FileName = promptPath
                };
                this.Config = new TextDocument(new StringTextSource(File.ReadAllText(configPath)))
                {
                    FileName = configPath
                };
                this.IsChanged = false;
            }
            else
            {
                this.Folder = "";
                this.Name = folder;
                this.Prompt = new TextDocument(new StringTextSource(""));
                this.Config = new TextDocument(new StringTextSource(DefaultConfig()));
            }
            this.Prompt.TextChanged += (sender, e) =>
            {
                IsChanged = true;
            };
            this.Config.TextChanged += (sender, e) =>
            {
                IsChanged = true;
            };
        }
        private string DefaultConfig()
        {
            return JsonSerializer.Serialize(new PromptTemplateConfig(), new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        }
        public bool IsChanged { get => isChanged; set => SetProperty(ref isChanged, value); }
        public TextDocument Prompt { get; set; }
        public TextDocument Config { get; set; }
        public static SemanticFunctionViewModel Create(string folder) => new(folder);
        public string Folder { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public PromptTemplateConfig PromptConfig => PromptTemplateConfig.FromJson(Config.Text);

        public IList<string> Blocks => _engine.ExtractBlocks(this.Prompt.Text)
                                        .Where(HasVariable)
                                        .Select(GetBlockContent)
                                        .ToList();

        private static bool HasVariable(Block block)
        {
            return block is VarBlock || (block is CodeBlock code && code.HasVar());
        }

        private static string GetBlockContent(Block block)
        {
            if (block is VarBlock var)
            {
                return var.Content;
            }
            else if (block is CodeBlock code)
            {
                return code.VarContent();
            }
            return string.Empty;
        }

        public bool IsGenerating { get => isGenerating; set => SetProperty(ref isGenerating, value); }

    }
}
