using System;
using System.ClientModel;
using System.Collections.Generic;

namespace OpenAI.Assistants;

internal class GetAssistantsCollection : CollectionResult<Assistant>
{
    private readonly Func<GetAssistantsPage> _getFirst;

    public GetAssistantsCollection(Func<GetAssistantsPage> getFirstPage)
    {
        _getFirst = getFirstPage;
    }

    public override IEnumerator<Assistant> GetEnumerator()
    {
        GetAssistantsPage page = _getFirst();

        while (page.HasNext)
        {
            foreach (Assistant value in page.Values)
            {
                yield return value;

                page = (GetAssistantsPage)page.GetNext();
            }
        }
    }
}
