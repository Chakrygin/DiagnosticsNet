namespace DiagnosticsNet
{
    public interface IDiagnosticManager
    {
        void Subscribe(IDiagnosticObserver diagnosticObserver);

        void Unsubscribe();
    }
}
