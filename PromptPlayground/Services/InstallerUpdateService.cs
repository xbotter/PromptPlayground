#define WINDOWS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.Services
{
    internal sealed class InstallerUpdateService
    {

        const string ReleaseLink = "https://github.com/xbotter/PromptPlayground/releases/latest";
#if WINDOWS
        const string InstallerLink = ReleaseLink + "/win-x64-setup.exe";
#endif
        private readonly string installerPath = Path.Combine(Environment.CurrentDirectory, "setup.exe");

        readonly HttpClient _httpClient = new();

        internal async Task<Version?> GetLatestVersionAsync()
        {
            var response = await _httpClient.GetAsync(ReleaseLink);
            if (response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.Redirect)
            {
                var content = response.Headers.Location!.ToString();
                var version = content.Split("tag")[1].Split("/")[0];
                return new Version(version);
            }
            return null;
        }

        internal async Task<Version?> HasNewVersion()
        {
            var latestVersion = await GetLatestVersionAsync();
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            if (latestVersion != null && latestVersion > currentVersion)
            {
                return latestVersion;
            }
            return null;
        }

        internal async Task DownloadInstallerAsync()
        {
            using var stream = await _httpClient.GetStreamAsync(InstallerLink);
            using var fileStream = File.OpenWrite(installerPath);
            await stream.CopyToAsync(fileStream);
            await stream.FlushAsync();
            await fileStream.FlushAsync();
        }

        internal bool IsInstallerExists()
        {
            return File.Exists(installerPath);
        }


        internal void RunInstaller()
        {
            var process = new Process();
            process.StartInfo.FileName = installerPath;
            process.Start();
        }
    }
}
