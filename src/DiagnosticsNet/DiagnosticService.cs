using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiagnosticsNet
{
    public sealed class DiagnosticService : IHostedService
    {
        private readonly ILogger<DiagnosticService> _logger;
        private readonly IDiagnosticManager _diagnosticManager;
        private readonly IEnumerable<IDiagnosticObserver> _diagnosticObservers;

        public DiagnosticService(
            ILogger<DiagnosticService> logger,
            IDiagnosticManager diagnosticManager,
            IEnumerable<IDiagnosticObserver> diagnosticObservers)
        {
            _logger = logger;
            _diagnosticManager = diagnosticManager;
            _diagnosticObservers = diagnosticObservers;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Diagnostic service starting...");

            foreach (var diagnosticObserver in _diagnosticObservers)
            {
                _diagnosticManager.Subscribe(diagnosticObserver);
            }

            _logger.LogDebug("Diagnostic service started.");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Diagnostic service stopping...");

            _diagnosticManager.Unsubscribe();

            _logger.LogDebug("Diagnostic service stopped.");

            return Task.CompletedTask;
        }
    }
}
