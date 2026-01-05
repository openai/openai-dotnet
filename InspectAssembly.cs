using System;
using System.Reflection;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        var assemblyPath = args[0];
        var assembly = Assembly.LoadFrom(assemblyPath);
        var types = assembly.GetTypes().Where(t => t.Name.Contains("InternalResponseFormatType"));
        foreach (var type in types)
        {
            Console.WriteLine($"Type: {type.FullName}, Assembly: {type.Assembly.FullName}");
        }
    }
}