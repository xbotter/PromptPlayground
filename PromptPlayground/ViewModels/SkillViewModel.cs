using Microsoft;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels
{
    public class SkillViewModel : ViewModelBase
    {
        private string folder = string.Empty;
        private SemanticFunctionViewModel? selectedFunction;
        public SemanticFunctionViewModel? SelectedFunction
        {
            get => selectedFunction;
            set
            {
                if (value != selectedFunction)
                {
                    selectedFunction = value;
                    OnPropertyChanged(nameof(SelectedFunction));
                }
            }
        }

        public string FolderName => Directory.Exists(Folder) ? Path.GetFileName(Folder) : string.Empty;
        public string Folder
        {
            get => folder; set
            {
                if (value != folder)
                {
                    folder = value;
                    OnPropertyChanged(nameof(Folder));
                    OnPropertyChanged(nameof(FolderName));
                    OnPropertyChanged(nameof(Functions));
                    OnPropertyChanged(nameof(OpenSkillFolder));
                }
            }
        }

        public List<SemanticFunctionViewModel> Functions
        {
            get => !Path.Exists(Folder) ? new List<SemanticFunctionViewModel>()
                    : Directory.GetDirectories(Folder)
                     .Where(IsFunctionDir)
                     .Select(SemanticFunctionViewModel.Create)
                     .ToList();
        }
        private bool IsFunctionDir(string folder) => File.Exists(Path.Combine(folder, Constants.SkPrompt));

        public bool OpenSkillFolder => Directory.Exists(folder);
    }
}
