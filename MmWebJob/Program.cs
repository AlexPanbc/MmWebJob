
using Topshelf;
namespace MmWebJob
{
    public class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<StartUp>(s =>
                {
                    s.ConstructUsing(name => new StartUp());
                    s.WhenStarted(t => t.Start());
                    s.WhenStopped(t => t.Stop());
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetServiceName("MmWebJob");
                x.SetDisplayName("哈哈定时服务");
            });
        }
    }
}
