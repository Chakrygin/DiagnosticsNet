using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DiagnosticsNet
{
    public sealed class DiagnosticManager : IDiagnosticManager, IDisposable
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public void Subscribe(IDiagnosticObserver diagnosticObserver)
        {
            var subscription = DiagnosticListener.AllListeners.Subscribe(diagnosticObserver);
            _subscriptions.Add(subscription);
        }

        public void Unsubscribe()
        {
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions.Clear();
        }

        public void Dispose()
        {
            Unsubscribe();
        }
    }
}
