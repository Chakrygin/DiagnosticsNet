using System;
using System.Diagnostics;

namespace DiagnosticsNet
{
    public interface IDiagnosticObserver : IObserver<DiagnosticListener>
    { }
}
