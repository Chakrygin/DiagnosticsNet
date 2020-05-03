namespace DiagnosticsNet
{
    public sealed class DiagnosticObserverOptions<TDiagnosticHandler>
        where TDiagnosticHandler : class, IDiagnosticHandler
    {
        public string DiagnosticListenerName { get; set; } = "";
    }
}
