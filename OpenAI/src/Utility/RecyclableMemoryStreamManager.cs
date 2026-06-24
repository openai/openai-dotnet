using Microsoft.IO;

namespace OpenAI;

internal class MemoryStreamManager
{
    public static readonly RecyclableMemoryStreamManager Manager = new ();
}