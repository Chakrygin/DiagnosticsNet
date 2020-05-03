using System;
using System.Diagnostics;

using Microsoft.Extensions.Logging;

using Moq;

namespace DiagnosticsNet.Tests
{
    public abstract class TestClassBase
    {
        public static DiagnosticListener CreateDiagnosticListener()
        {
            var guid = Guid.NewGuid();
            var name = guid.ToString("N");

            var listener = new DiagnosticListener(name);

            return listener;
        }

        public static Mock<IDiagnosticHandler> CreateDiagnosticHandler()
        {
            return CreateDiagnosticHandler<IDiagnosticHandler>();
        }

        public static Mock<TDiagnosticHandler> CreateDiagnosticHandler<TDiagnosticHandler>()
            where TDiagnosticHandler : class, IDiagnosticHandler
        {
            var mock = new Mock<TDiagnosticHandler> {CallBase = true};

            mock.Setup(x => x.IsEnabled(It.IsAny<string>()))
                .Returns(true);

            return mock;
        }

        public static IDiagnosticObserver CreateDiagnosticObserver(DiagnosticListener listener)
        {
            var handler = CreateDiagnosticHandler();

            return CreateDiagnosticObserver(handler.Object, listener);
        }

        public static IDiagnosticObserver CreateDiagnosticObserver(IDiagnosticHandler handler, DiagnosticListener listener)
        {
            var observer = CreateDiagnosticObserver(handler, options =>
            {
                options.DiagnosticListenerName = listener.Name;
            });

            return observer;
        }

        public static IDiagnosticObserver CreateDiagnosticObserver(IDiagnosticHandler handler,
            Action<DiagnosticObserverOptions<IDiagnosticHandler>> configure)
        {
            var logger = CreateLogger<DiagnosticObserver<IDiagnosticHandler>>();
            var options = new DiagnosticObserverOptions<IDiagnosticHandler>();
            configure(options);

            return new DiagnosticObserver<IDiagnosticHandler>(options, handler);
        }

        public static DiagnosticManager CreateDiagnosticManager()
        {
            return new DiagnosticManager();
        }

        private static ILogger<T> CreateLogger<T>()
        {
            return Mock.Of<ILogger<T>>();
        }
    }
}
