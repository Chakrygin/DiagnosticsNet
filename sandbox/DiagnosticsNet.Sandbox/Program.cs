using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DiagnosticsNet.Sandbox
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>();
                })
                .Build();

            host.Run();
        }
    }
}
