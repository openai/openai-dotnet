using System;
using System.ClientModel;
using System.Collections.Generic;

namespace OpenAI.Assistants;

internal class AssistantCollection : CollectionResult<Assistant>
{
    private readonly Func<AssistantCollectionPage> _getFirst;

    public AssistantCollection(Func<AssistantCollectionPage> getFirstPage)
    {
        _getFirst = getFirstPage;
    }

    public override IEnumerator<Assistant> GetEnumerator()
    {
        AssistantCollectionPage page = _getFirst();

        while (page.HasNext)
        {
            foreach (Assistant value in page.Values)
            {
                yield return value;

                page = (AssistantCollectionPage)page.GetNext();
            }
        }
    }
}
