using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels
{
    public partial class AboutViewModel : ViewModelBase
    {

        const string GitHubLink = "https://github.com/xbotter/PromptPlayground";
        const string ReleaseLink = GitHubLink + "/releases/latest";

        public Version? CurrentVersion { get; set; }

        public AboutViewModel()
        {
            CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        }


        [RelayCommand]
        public void OpenGitHub() => OpenLink(GitHubLink);

        [RelayCommand]
        public void CheckVersion() => OpenLink(ReleaseLink);

        public void OpenLink(string link)
        {
            var psi = new ProcessStartInfo()
            {
                FileName = link,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}
