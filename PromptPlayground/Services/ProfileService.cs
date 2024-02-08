using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.Services
{
    internal class ProfileService<T>
    {
        private readonly string _profile;

        public ProfileService(string profile)
        {
            this._profile = profile;
        }

        public string ProfilePath()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var profileFolder = Path.Combine(userProfile, ".prompt_playground");
            if (!Directory.Exists(profileFolder))
            {
                Directory.CreateDirectory(profileFolder);
            }
            return Path.Combine(profileFolder, $"{_profile}");
        }



        public T? Get()
        {
            var path = ProfilePath();
            if (!File.Exists(path))
            {
                return default;
            }
            var json = File.ReadAllText(path);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public void Save(T profile)
        {
            var path = ProfilePath();
            var json = System.Text.Json.JsonSerializer.Serialize(profile);
            File.WriteAllText(path, json, Encoding.UTF8);
        }
    }
}
