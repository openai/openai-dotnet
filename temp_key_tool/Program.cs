using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run <path-to-snk>");
            return;
        }
        string snkPath = args[0];
        byte[] bytes = File.ReadAllBytes(snkPath);
        
        // Header for public key blob
        byte[] header = new byte[] { 
            0x00, 0x24, 0x00, 0x00, 0x04, 0x80, 0x00, 0x00, 
            0x94, 0x00, 0x00, 0x00, 0x06, 0x02, 0x00, 0x00, 
            0x00, 0x24, 0x00, 0x00, 0x52, 0x53, 0x41, 0x31 
        };

        int index = Search(bytes, header);
        if (index != -1)
        {
            // Found the header.
            // The public key blob is usually 160 bytes for 1024-bit key.
            // But let's just print 160 bytes and see.
            // Or maybe more.
            // The Azure key in csproj is 320 hex chars = 160 bytes.
            // So let's try to extract 160 bytes.
            
            byte[] publicKey = new byte[160];
            Array.Copy(bytes, index, publicKey, 0, 160);
            Console.WriteLine(BitConverter.ToString(publicKey).Replace("-", "").ToLower());
        }
        else
        {
            Console.WriteLine("Header not found.");
            // Print hex dump of first 100 bytes
            Console.WriteLine(BitConverter.ToString(bytes.Take(100).ToArray()));
        }
    }

    static int Search(byte[] src, byte[] pattern)
    {
        for (int i = 0; i <= src.Length - pattern.Length; i++)
        {
            if (src.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
            {
                return i;
            }
        }
        return -1;
    }
}
