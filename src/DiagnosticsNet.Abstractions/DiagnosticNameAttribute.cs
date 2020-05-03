using System;

namespace DiagnosticsNet
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class DiagnosticNameAttribute : Attribute
    {
        public DiagnosticNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
