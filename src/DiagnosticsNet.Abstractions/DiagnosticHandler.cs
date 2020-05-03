using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DiagnosticsNet
{
    public abstract class DiagnosticHandler : IDiagnosticHandler
    {
        private readonly Dictionary<string, DiagnosticWriter> _writers;

        protected DiagnosticHandler()
        {
            var methods = GetDiagnosticMethods();

            _writers = methods.ToDictionary(
                x => x.Key,
                x => new DiagnosticWriter(x.Value, this));
        }

        private Dictionary<string, MethodInfo> GetDiagnosticMethods()
        {
            var result = new Dictionary<string, MethodInfo>();

            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes<DiagnosticNameAttribute>();
                foreach (var attribute in attributes)
                {
                    var diagnosticName = attribute.Name;

                    if (result.TryGetValue(diagnosticName, out var otherMethod))
                    {
                        throw new InvalidOperationException();
                    }

                    result.Add(diagnosticName, method);
                }
            }

            return result;
        }

        public virtual bool IsEnabled(string name)
        {
            return _writers.ContainsKey(name);
        }

        public virtual void Write(string name, object value)
        {
            if (_writers.TryGetValue(name, out var writer))
            {
                writer?.Write(value);
            }
        }
    }
}
