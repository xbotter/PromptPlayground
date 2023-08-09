using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace PromptPlayground.ViewModels
{
    public class SkillViewModel : ViewModelBase
    {
        public SkillViewModel()
        {
            Plugins.Add(new PluginViewModel()
            {
                Title = "unsaved"
            });
            Plugins.Add(new PluginViewModel()
            {
                Title = "test"
            });
        }
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
                    OnPropertyChanged(nameof(SelectedIndex));
                }
            }
        }

        public int SelectedIndex
        {
            get => SelectedFunction != null ? Functions.IndexOf(SelectedFunction) : -1;
        }
        public ObservableCollection<PluginViewModel> Plugins { get; set; } = new ObservableCollection<PluginViewModel>();
        public string FolderName => Directory.Exists(Folder) ? Path.GetFileName(Folder) : string.Empty;
        public string Folder
        {
            get => folder; set
            {
                if (value != folder)
                {
                    folder = value;
                    if (Path.Exists(Folder))
                    {
                        Functions = Directory.GetDirectories(Folder)
                                     .Where(IsFunctionDir)
                                     .Select(SemanticFunctionViewModel.Create)
                                     .ToList();
                    }
                    OnPropertyChanged(nameof(Folder));
                    OnPropertyChanged(nameof(FolderName));
                    OnPropertyChanged(nameof(Functions));
                    OnPropertyChanged(nameof(OpenSkillFolder));
                }
            }
        }

        public List<SemanticFunctionViewModel> Functions { get; set; } = new List<SemanticFunctionViewModel>();
        private bool IsFunctionDir(string folder) => File.Exists(Path.Combine(folder, Constants.SkPrompt));

        public bool OpenSkillFolder => Directory.Exists(folder);
    }
    public class PluginViewModel : ViewModelBase
    {
        public string? Title { get; set; }
        public List<SemanticFunctionViewModel> Functions { get; set; } = new List<SemanticFunctionViewModel>()
        {
            new SemanticFunctionViewModel("Test")
        };
    }
}
