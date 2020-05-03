namespace DiagnosticsNet
{
    public interface IDiagnosticHandler
    {
        bool IsEnabled(string name);

        void Write(string name, object value);
    }
}
