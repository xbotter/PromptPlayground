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
        private int limit = 2;
        private string collection = string.Empty;
        private float relevance = 0.7f;

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

        public int Limit
        {
            get => limit;
            set => SetProperty(ref limit, value);
        }
        public string Collection
        {
            get => collection;
            set => SetProperty(ref collection, value);
        }

        public float Relevance
        {
            get => relevance;
            set => SetProperty(ref relevance, value);
        }
    }
}
