using Microsoft;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.Qdrant;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Skills.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels.ConfigViewModels.VectorDB
{
    public class QdrantConfigViewModel : ConfigViewModelBase, IVectorDbConfigViewModel
    {
        public override string Name => "Qdrant";

        public QdrantConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
            RequireAttribute(ConfigAttribute.QdrantEndpoint);
            RequireAttribute(ConfigAttribute.QdrantApiKey);

            RequireAttribute(ConfigAttribute.VectorSize);
        }

        public void RegisterMemory(KernelBuilder kernel)
        {
            var endpoint = GetAttribute(ConfigAttribute.QdrantEndpoint);
            var vectorSize = GetAttribute(ConfigAttribute.VectorSize);

            Requires.NotNullOrWhiteSpace(endpoint);

            var store = new QdrantMemoryStore(endpoint!, int.Parse(vectorSize));

            kernel.WithMemoryStorage(store);

        }
    }
}
