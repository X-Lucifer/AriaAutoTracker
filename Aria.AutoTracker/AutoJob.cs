using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using TG.INI;
using Volo.Abp.DependencyInjection;

namespace Aria.AutoTracker
{
    public class AutoJob : IJob, ITransientDependency
    {
        private readonly ILogger<AutoJob> _log;
        private readonly IConfiguration _config;

        public AutoJob(ILogger<AutoJob> log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                AriaConfig config = new AriaConfig();
                _config.Bind("AriaConfig", config);
                if (string.IsNullOrEmpty(config.Path) || config.TrackerList == null || config.TrackerList.Count <= 0)
                {
                    return;
                }

                IniDocument doc = new IniDocument();
                if (!File.Exists(config.Path))
                {
                    var path = AppContext.BaseDirectory + "aria2.conf";
                    doc.Read(path);
                }
                else
                {
                    doc.Read(config.Path);
                }

                var key = "bt-tracker";
                var tracker = doc.GlobalSection.Find(key);
                if (tracker != null || doc.GlobalSection.ContainsKey(key))
                {
                    doc.GlobalSection.RemoveEntry(tracker);
                }

                var txtlist = new List<string>();
                foreach (var item in config.TrackerList.OrderBy(x => x.Sort))
                {
                    using var client = new WebClient {Encoding = Encoding.UTF8};
                    var txt = await client.DownloadStringTaskAsync(new Uri(item.Url));
                    if (!string.IsNullOrEmpty(txt))
                    {
                        _log.LogInformation($"BT Tracker: 【{item.Name}】 download has been completed , {DateTime.Now:G}");
                        txt = Regex.Replace(txt, "\\s+", "|");
                        txtlist.Add(txt);
                    }
                }

                var trackers = new List<string>();
                foreach (var item in txtlist)
                {
                    foreach (var line in item.Split('|', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (string.IsNullOrEmpty(line) || trackers.Contains(line))
                        {
                            continue;
                        }

                        trackers.Add(line);
                        _log.LogInformation($"Add tracker: {line}");
                    }
                }

                var xtracker = string.Join(",", trackers);
                doc.GlobalSection.AddKeyValue(key, xtracker);
                File.Delete(config.Path);
                await Task.Delay(1000);
                await using var xfile = new FileStream(config.Path, FileMode.Create, FileAccess.ReadWrite);
                doc.Write(xfile);
                _log.LogInformation($"Aria2 config update finished , {DateTime.Now:G}");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var service = _config["ServiceName"];
                    if (!string.IsNullOrEmpty(service))
                    {
                        var list = ServiceController.GetServices();
                        foreach (var item in list)
                        {
                            if (!item.ServiceName.Equals(service, StringComparison.InvariantCultureIgnoreCase))
                            {
                                continue;
                            }

                            if (item.Status != ServiceControllerStatus.Stopped &&
                                item.Status != ServiceControllerStatus.StopPending)
                            {
                                item.Stop();
                                await Task.Delay(1000);
                                _log.LogInformation($"Service: {service} has stopped running , {DateTime.Now:G}");
                            }

                            item.Start();
                            await Task.Delay(1000);
                            _log.LogInformation($"Service: {service} has started , {DateTime.Now:G}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.LogInformation(e.ToString());
            }
        }
    }
}