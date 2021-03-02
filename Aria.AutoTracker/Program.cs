using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Aria.AutoTracker
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseWindowsService(x => { x.ServiceName = "aria_auto_tracker"; })
                .UseAutofac()
                .ConfigureServices((context, service) => { service.AddApplication<AutoModule>(); });
    }
}