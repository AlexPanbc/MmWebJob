using Autofac;
using Hangfire;
using Hangfire.Console;
using Microsoft.Owin.Hosting;
using System;

namespace MmWebJob
{
    public class StartUp
    {
        private IDisposable _host;
        public void Start()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<AutofacModule>();
            GlobalConfiguration.Configuration.UseAutofacActivator(builder.Build(), false)
                .UseRedisStorage("110.110.110.110:6379,password=110", new Hangfire.Redis.RedisStorageOptions
                {
                    Db = 15,
                    Prefix = "job:"
                })
                .UseConsole(new ConsoleOptions
                {
                    FollowJobRetentionPolicy = true,
                    PollInterval = 2000,
                    ExpireIn = TimeSpan.FromHours(1)
                });

            RegisterJob.Register();
            _host = WebApp.Start<OwinStart>($"http://localhost:1234");
        }
        public void Stop()
        {
            _host.Dispose();
        }
    }
}
