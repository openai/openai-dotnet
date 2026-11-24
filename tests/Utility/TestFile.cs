using System.IO;
using System.Threading.Tasks;

namespace OpenAI.Tests.Utility;

internal static class TestFile
{
    public static string JoinPath(string path1, string path2)
    {
#if NETFRAMEWORK
        return Path.Combine(path1, path2);
#else
        return Path.Join(path1, path2);
#endif
    }

    public static Task<byte[]> ReadAllBytesAsync(string path)
    {
#if NETFRAMEWORK
        return Task.Run(() => File.ReadAllBytes(path));
#else
        return File.ReadAllBytesAsync(path);
#endif
    }
}
