using Hangfire;
using Microsoft.Owin;
using MmWebJob;
using Owin;

[assembly: OwinStartup(typeof(OwinStart))]
namespace MmWebJob
{
    class OwinStart
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseHangfireDashboard("/console");
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                //全部小写
                Queues = new[] { "default", "redis", "online", "offline", "rabbitmq", "distribution_cs" },
                ServerName = "哈哈定时服务"
            });
        }
    }
}
