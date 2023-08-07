using Avalonia.Interactivity;
using Microsoft;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.Qdrant;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Memory.Collections;
using Microsoft.SemanticKernel.Skills.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels.ConfigViewModels.VectorDB
{
    public class QdrantConfigViewModel : ConfigViewModelBase, IVectorDbConfigViewModel
    {
        private QdrantMemoryStore? _store;
        public override string Name => "Qdrant";

        public QdrantConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
            RequireAttribute(ConfigAttribute.QdrantEndpoint);
            RequireAttribute(ConfigAttribute.QdrantApiKey);
            RequireAttribute(ConfigAttribute.VectorSize);
        }

        public void RegisterMemory(KernelBuilder kernel)
        {
            var store = GetStore();
            kernel.WithMemoryStorage(store);
        }
        private QdrantMemoryStore GetStore()
        {
            if (_store != null)
            {
                return _store;
            }

            var endpoint = GetAttribute(ConfigAttribute.QdrantEndpoint);
            var vectorSize = GetAttribute(ConfigAttribute.VectorSize);
            var apiKey = GetAttribute(ConfigAttribute.QdrantApiKey);

            Requires.NotNullOrWhiteSpace(endpoint);

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(endpoint)
            };

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
            }

            _store = new QdrantMemoryStore(httpClient, int.Parse(vectorSize));
            return _store;
        }

        public int Limit { get; set; } = 3;
        public string Collection { get; set; }

    }
}
