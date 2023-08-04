using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.ViewModels.ConfigViewModels.VectorDB
{
    public class QdrantConfigViewModel : VectorDbConfigViewModelBase
    {
        public override string Name => "Qdrant";

        public QdrantConfigViewModel(IConfigAttributesProvider provider) : base(provider)
        {
            RequireAttribute(ConfigAttribute.QdrantEndpoint);
            RequireAttribute(ConfigAttribute.QdrantApiKey);
        }
    }
}
