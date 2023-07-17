using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels
{
    public class SkillViewModel : ViewModelBase
    {
        private string folder = string.Empty;

        public string Folder
        {
            get => folder; set
            {
                if (value != folder)
                {
                    folder = value;
                    OnPropertyChanged(nameof(Folder));
                }
            }
        }
    

        // todo: render folder tree view
        
    }
}
