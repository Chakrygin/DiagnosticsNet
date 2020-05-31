Diagnostics.NET
===============

.NET Core provides the DiagnosticSource mechanism that allows code to be
instrumented for production-time logging of rich data payloads for consumption
within the process that was instrumented. At runtime consumers can dynamically
discover data sources and subscribe to the ones of interest.

This mechanism is widely used by .NET and ASP.NET Core classes and can be used,
for example, to collect metrics and traces.

Diagnostics.NET is a simple library that allows your ASP.NET Core applications
to subscribe to DiagnosticSource events and process them.

You can learn more about the DiagnosticSource mechanism from
[DiagnosticSource User's Guide](https://github.com/dotnet/runtime/blob/master/src/libraries/System.Diagnostics.DiagnosticSource/src/DiagnosticSourceUsersGuide.md).

Packages
--------

The Diagnostics.NET library consists of two NuGet packages:

* DiagnosticsNet
* DiagnosticsNet.Abstractions

Quick Start
-----------

Suppose that we need to log the execution time of HTTP requests in our
ASP.NET Core application. Let's consider how this problem can be solved with
the DiagnosticSource mechanism and the Diagnostics.NET library.

1. Create new ASP.NET Core application.

2. Install DiagnosticsNet package.

3. Create a handler class by implementing interface `IDiagnosticHandler` or by
   inheriting from abstract class `DiagnosticHandler`.

4. If you are implementing interface `IDiagnosticHandler`, you need to implement
   methods `IsEnabled` and `Write`:

    ```c#
    using DiagnosticsNet;

    public sealed class ExampleDiagnosticHandler : IDiagnosticHandler
    {
        private readonly ILogger<ExampleDiagnosticHandler> _logger;

        public ExampleDiagnosticHandler(ILogger<ExampleDiagnosticHandler> logger)
        {
            _logger = logger;
        }

        public bool IsEnabled(string name)
        {
            // This method should return true for each event that we want to process.

            var isEnabled =
                name == "Microsoft.AspNetCore.Hosting.HttpRequestIn" ||
                name == "Microsoft.AspNetCore.Hosting.HttpRequestIn.Start" ||
                name == "Microsoft.AspNetCore.Hosting.HttpRequestIn.Stop";

            return isEnabled;
        }

        public void Write(string name, dynamic value)
        {
            // This method should get the data from the value and
            // use it to handle the event.

            switch (name)
            {
                case "Microsoft.AspNetCore.Hosting.HttpRequestIn.Start":
                {
                    var httpContext = (HttpContext) value.HttpContext;
                    httpContext.Items["Stopwatch"] = Stopwatch.StartNew();

                    break;
                }

                case "Microsoft.AspNetCore.Hosting.HttpRequestIn.Stop":
                {
                    var httpContext = (HttpContext) value.HttpContext;
                    var stopwatch = (Stopwatch) httpContext.Items["Stopwatch"];

                    _logger.LogInformation(
                       $"Request finished in {stopwatch.Elapsed.TotalMilliseconds} ms.");

                    break;
                }
            }
        }
    }
    ```

5. If you inherit from class `DiagnosticHandler`, you need to define a methods
   for each event that you need to process and mark them with `DiagnosticName` attribute.

    ```c#
    using DiagnosticsNet;

    public sealed class ExampleDiagnosticHandler : DiagnosticHandler
    {
        private readonly ILogger<ExampleDiagnosticHandler> _logger;

        public ExampleDiagnosticHandler(ILogger<ExampleDiagnosticHandler> logger)
        {
            _logger = logger;
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.HttpRequestIn")]
        public void OnHttpRequestIn()
        {
            // This method is required to receive Start and Stop events.
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.HttpRequestIn.Start")]
        public void OnHttpRequestInStart(HttpContext httpContext)
        {
            httpContext.Items["Stopwatch"] = Stopwatch.StartNew();
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.HttpRequestIn.Stop")]
        public void OnHttpRequestInStop(HttpContext httpContext)
        {
            var stopwatch = (Stopwatch) httpContext.Items["Stopwatch"];

            _logger.LogInformation(
                $"Request finished in {stopwatch.Elapsed.TotalMilliseconds} ms.");
        }
    }
    ```

6. Add the handler class to the DI container in the Startup.cs file:

    ```c#
    using DiagnosticsNet;

    public sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDiagnostics()
                .AddDiagnosticObserver<ExampleDiagnosticHandler>(options =>
                {
                    options.DiagnosticListenerName = "Microsoft.AspNetCore";
                });

            // ...
        }

        // ...
    }
    ```

7. Launch the application. In the log you will see a message similar to:

    ```log
    info: ExampleDiagnosticHandler[0]
          Request finished in 0.48 ms.
    ```
