// See https://aka.ms/new-console-template for more information


using FilePropertiesStructExporter;
using System.Reflection;

var assembly = Assembly.GetEntryAssembly();
if (assembly is null)
    throw new InvalidProgramException();

var filepath = $"{Path.GetDirectoryName(assembly.Location)}/properties.txt";

using (var writer = new StreamWriter(new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write)))
{
    foreach (var info in PropertyKeyEnumerator.EnumSystems())
    {
        writer.WriteLine(info);
        Console.WriteLine(info);
    }
}

Console.WriteLine("\"{filepath}\" exported.");
