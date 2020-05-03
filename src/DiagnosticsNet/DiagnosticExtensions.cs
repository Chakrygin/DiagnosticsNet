using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace DiagnosticsNet
{
    public static class DiagnosticExtensions
    {
        public static IServiceCollection AddDiagnostics(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddTransient<IDiagnosticManager, DiagnosticManager>();
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IHostedService, DiagnosticService>());

            return services;
        }

        public static IServiceCollection AddDiagnosticObserver<TDiagnosticHandler>(this IServiceCollection services,
            Action<DiagnosticObserverOptions<TDiagnosticHandler>> configure)
            where TDiagnosticHandler : class, IDiagnosticHandler
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.TryAddTransient<DiagnosticObserverOptions<TDiagnosticHandler>>(serviceProvider =>
            {
                var options = new DiagnosticObserverOptions<TDiagnosticHandler>();
                configure(options);

                return options;
            });

            services.TryAddTransient<TDiagnosticHandler>();

            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IDiagnosticObserver, DiagnosticObserver<TDiagnosticHandler>>());

            return services;
        }

        public static IServiceCollection AddAspNetCoreDiagnosticObserver<TDiagnosticHandler>(this IServiceCollection services)
            where TDiagnosticHandler : class, IDiagnosticHandler
        {
            return services.AddDiagnosticObserver<TDiagnosticHandler>(options =>
            {
                options.DiagnosticListenerName = "Microsoft.AspNetCore";
            });
        }

        public static IServiceCollection AddEntityFrameworkCoreDiagnosticObserver<TDiagnosticHandler>(this IServiceCollection services)
            where TDiagnosticHandler : class, IDiagnosticHandler
        {
            return services.AddDiagnosticObserver<TDiagnosticHandler>(options =>
            {
                options.DiagnosticListenerName = "Microsoft.EntityFrameworkCore";
            });
        }

        public static IServiceCollection AddSqlClientDiagnosticObserver<TDiagnosticHandler>(this IServiceCollection services)
            where TDiagnosticHandler : class, IDiagnosticHandler
        {
            return services.AddDiagnosticObserver<TDiagnosticHandler>(options =>
            {
                options.DiagnosticListenerName = "SqlClientDiagnosticListener";
            });
        }

        public static IServiceCollection AddHttpClientDiagnosticObserver<TDiagnosticHandler>(this IServiceCollection services)
            where TDiagnosticHandler : class, IDiagnosticHandler
        {
            return services.AddDiagnosticObserver<TDiagnosticHandler>(options =>
            {
                options.DiagnosticListenerName = "HttpHandlerDiagnosticListener";
            });
        }
    }
}
