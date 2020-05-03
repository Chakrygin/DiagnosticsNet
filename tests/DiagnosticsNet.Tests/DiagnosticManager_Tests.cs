using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiagnosticsNet.Tests
{
    [TestClass]
    public sealed class DiagnosticManager_Tests : TestClassBase
    {
        [TestMethod]
        public void After_subscribe_listener_should_be_enabled()
        {
            var listener = CreateDiagnosticListener();
            var observer = CreateDiagnosticObserver(listener);

            using (var manager = CreateDiagnosticManager())
            {
                manager.Subscribe(observer);

                listener.IsEnabled()
                    .Should().BeTrue();
            }
        }

        [TestMethod]
        public void After_unsubscribe_listener_should_not_be_enabled()
        {
            var listener = CreateDiagnosticListener();
            var observer = CreateDiagnosticObserver(listener);

            using (var manager = CreateDiagnosticManager())
            {
                manager.Subscribe(observer);
                manager.Unsubscribe();

                listener.IsEnabled()
                    .Should().BeFalse();
            }
        }

        [TestMethod]
        public void After_dispose_listener_should_not_be_enabled()
        {
            var listener = CreateDiagnosticListener();
            var observer = CreateDiagnosticObserver(listener);

            using (var manager = CreateDiagnosticManager())
            {
                manager.Subscribe(observer);
            }

            listener.IsEnabled()
                .Should().BeFalse();
        }
    }
}
