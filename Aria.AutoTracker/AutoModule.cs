using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Aria.AutoTracker
{
    [DependsOn(typeof(AbpAutofacModule))]
    public class AutoModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var config = context.Services.GetConfiguration();
            var interval = Convert.ToDouble(config["Interval"]);
            context.Services.AddQuartz(x =>
            {
                x.SchedulerId = "auto-scheduler";
                x.SchedulerName = "auto update aria2 bt-tracker scheduler";
                x.UseMicrosoftDependencyInjectionScopedJobFactory();
                x.UseSimpleTypeLoader();
                x.UseInMemoryStore();
                x.UseDefaultThreadPool(z => { z.MaxConcurrency = 10; });

                var key = new JobKey(Guid.NewGuid().ToString("D"));
                x.AddJob<AutoJob>(z => { z.StoreDurably().WithIdentity(key); });
                x.AddTrigger(z => z
                    .ForJob(key)
                    .StartNow()
                    .WithSimpleSchedule(w => w.WithInterval(TimeSpan.FromHours(interval)).RepeatForever())
                );
            });

            context.Services.AddQuartzHostedService(x => { x.WaitForJobsToComplete = true; });
        }
    }
}