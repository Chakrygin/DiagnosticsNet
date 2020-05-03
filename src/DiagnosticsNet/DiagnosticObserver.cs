using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DiagnosticsNet
{
    public sealed class DiagnosticObserver<TDiagnosticHandler> : IDiagnosticObserver, IObserver<KeyValuePair<string, object>>
        where TDiagnosticHandler : class, IDiagnosticHandler
    {
        private readonly DiagnosticObserverOptions<TDiagnosticHandler> _options;
        private readonly TDiagnosticHandler _handler;

        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public DiagnosticObserver(
            DiagnosticObserverOptions<TDiagnosticHandler> options,
            TDiagnosticHandler handler)
        {
            _options = options;
            _handler = handler;
        }

        #region IObserver<DiagnosticListener>

        void IObserver<DiagnosticListener>.OnNext(DiagnosticListener diagnosticListener)
        {
            if (diagnosticListener.Name == _options.DiagnosticListenerName)
            {
                var subscription = diagnosticListener.Subscribe(this, _handler.IsEnabled);
                _subscriptions.Add(subscription);
            }
        }

        void IObserver<DiagnosticListener>.OnError(Exception error)
        { }

        void IObserver<DiagnosticListener>.OnCompleted()
        {
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions.Clear();
        }

        #endregion

        #region IObserver<KeyValuePair<string, object>>

        void IObserver<KeyValuePair<string, object>>.OnNext(KeyValuePair<string, object> pair)
        {
            _handler.Write(pair.Key, pair.Value);
        }

        void IObserver<KeyValuePair<string, object>>.OnError(Exception error)
        { }

        void IObserver<KeyValuePair<string, object>>.OnCompleted()
        { }

        #endregion
    }
}
