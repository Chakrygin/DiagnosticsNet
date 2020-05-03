using System;
using System.Reflection;

namespace DiagnosticsNet.Sandbox.DiagnosticHandlers
{
    public sealed class GenericDiagnosticHandler : IDiagnosticHandler
    {
        public bool IsEnabled(string name)
        {
            Console.WriteLine($"IsEnabled: {name}");
            Console.WriteLine();

            return true;
        }

        public void Write(string name, object value)
        {
            Console.WriteLine($"{name}:");

            var type = value.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                try
                {
                    var propertyValue = property.GetValue(value);
                    Console.WriteLine($"\t{property.Name}: {propertyValue}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\t{property.Name}: {ex.Message}");
                }
            }

            Console.WriteLine();
        }
    }
}
