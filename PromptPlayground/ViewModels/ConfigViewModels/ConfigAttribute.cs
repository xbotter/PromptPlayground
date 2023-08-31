using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PromptPlayground.ViewModels.ConfigViewModels
{
    public class ConfigAttribute : ObservableObject
    {
        static ConfigAttribute()
        {
            var consts = typeof(ConfigAttribute).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
            var ct = typeof(ConfigTypeAttribute);
            _fields = consts.Select(c =>
            {
                var value = c.GetValue(null) as string;

                if (c.IsDefined(ct))
                {
                    return (value, c.GetCustomAttribute(ct) as ConfigTypeAttribute);
                }
                else
                {
                    return (value, new ConfigTypeAttribute());
                }
            }).ToDictionary(_ => _.Item1!, _ => _.Item2!);
        }
        static readonly Dictionary<string, ConfigTypeAttribute> _fields;
        public ConfigAttribute(string name)
        {
            Name = name;
            this.Type = _fields[name].Type;
            this.SelectValues = _fields[name].SelectValues?.ToList() ?? new List<string>();
        }
        private string _value = string.Empty;
        [JsonIgnore]
        public string HumanizeName => Name.Humanize();
        public string Type { get; set; } = "string";
        public List<string> SelectValues { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get => _value; set => SetProperty(ref _value, value, nameof(Value)); }

        #region Constants
        public const string AzureDeployment = nameof(AzureDeployment);
        public const string AzureEndpoint = nameof(AzureEndpoint);
        public const string AzureSecret = nameof(AzureSecret);
        public const string AzureEmbeddingDeployment = nameof(AzureEmbeddingDeployment);

        public const string BaiduClientId = nameof(BaiduClientId);
        public const string BaiduSecret = nameof(BaiduSecret);
        [ConfigType("select", "Ernie-Bot", "Ernie-Bot-turbo", "BLOOMZ_7B")]
        public const string BaiduModel = nameof(BaiduModel);

        public const string QdrantEndpoint = nameof(QdrantEndpoint);
        public const string QdrantApiKey = nameof(QdrantApiKey);

        public const string VectorSize = nameof(VectorSize);
        #endregion
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ConfigTypeAttribute : Attribute
    {
        /// <summary>
        /// `string` or `select`, use select must set SelectValues
        /// </summary>
        /// <param name="type"></param>
        public ConfigTypeAttribute(string type = "string", params string[] selectValues)
        {
            Type = type;
            if (type == "select")
            {
                if (selectValues == null || selectValues.Length == 0)
                {
                    throw new ArgumentException("selectValues must not be null or empty when type is select");
                }

                SelectValues = selectValues;
            }
        }

        public string Type { get; }
        public string[]? SelectValues { get; }
    }


}
