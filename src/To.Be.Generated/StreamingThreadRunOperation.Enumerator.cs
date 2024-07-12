using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

public partial class StreamingThreadRunOperation : ThreadRunOperation
{
    internal class AsyncUpdateEnumerator : ContinuableAsyncEnumerator<StreamingUpdate>
    {

    }
}
