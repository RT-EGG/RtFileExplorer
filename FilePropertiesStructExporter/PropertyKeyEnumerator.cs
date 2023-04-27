using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System.Reflection;

namespace FilePropertiesStructExporter
{
    public class PropertyKeyEnumerator
    {
        public static IEnumerable<KeyInfo> EnumSystems()
        {
            var ancestors = new List<Type>();

            return EnumIn(ancestors, typeof(SystemProperties.System));
        }

        private static IEnumerable<KeyInfo> EnumIn(List<Type> inAncestors, Type inType)
        {
            inAncestors.Add(inType);
            try
            {
                var ancestors = inAncestors.ToArray();

                foreach (var property in inType.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty))
                {
                    if (!(property.GetValue(inType) is PropertyKey propertyKey))
                        throw new Exception();

                    yield return new KeyInfo(ancestors, property.Name, propertyKey);
                }

                foreach (var inner in inType.GetNestedTypes())
                {
                    foreach (var info in EnumIn(inAncestors, inner))
                    {
                        yield return info;
                    }
                }
            }
            finally
            {
                inAncestors.RemoveAt(inAncestors.Count - 1);
            }

            yield break;
        }

        public class KeyInfo
        {
            public KeyInfo(Type[] inAncestors, string inName, PropertyKey inKey)
            {
                Ancestors = inAncestors;
                Name = inName;

                var property = FileInstance.Properties.GetProperty(inKey);
                DisplayName = property.Description.DisplayName;
                ValueType = property.Description.ValueType;

            }

            public readonly Type[] Ancestors;
            public readonly string Name;
            public readonly string DisplayName;
            public readonly Type ValueType;

            public override string ToString()
                => $"{string.Join('.', Ancestors.Select(t => t.Name))}.{Name}({ValueType}): \"{DisplayName}\"";

            private ShellFile FileInstance => _fileInstance.Value;
            private Lazy<ShellFile> _fileInstance = new Lazy<ShellFile>(
                () =>
                {
                    var assembly = Assembly.GetEntryAssembly();
                    if (assembly is null)
                        throw new InvalidProgramException();

                    return ShellFile.FromFilePath(assembly.Location);
                }
            );
        }
    }
}
