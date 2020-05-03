using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace DiagnosticsNet.Tests
{
    [TestClass]
    public sealed class DiagnosticObserver_Tests : TestClassBase
    {
        [TestMethod]
        public void TestMethod()
        {
            var listener = CreateDiagnosticListener();
            var handler = CreateDiagnosticHandler();
            var observer = CreateDiagnosticObserver(handler.Object, listener);

            using (var manager = CreateDiagnosticManager())
            {
                manager.Subscribe(observer);

                listener.IsEnabled("Test");
            }

            handler.Verify(x => x.IsEnabled("Test"), Times.Once);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var listener = CreateDiagnosticListener();
            var handler = CreateDiagnosticHandler();
            var observer = CreateDiagnosticObserver(handler.Object, listener);

            var value = new { };
            using (var manager = CreateDiagnosticManager())
            {
                manager.Subscribe(observer);

                listener.Write("Test", value);
            }

            handler.Verify(x => x.Write("Test", value), Times.Once);
        }
    }
}
