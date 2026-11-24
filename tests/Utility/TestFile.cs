using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests.Utility;

internal static class TestFile
{
    public static Task<byte[]> ReadAllBytesAsync(string path)
    {
#if NETFRAMEWORK
        return Task.Run(() => File.ReadAllBytes(path));
#else
        return File.ReadAllBytesAsync(path);
#endif
    }
}
