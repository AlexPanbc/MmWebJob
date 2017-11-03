using System;
using Autofac;
using Module = Autofac.Module;

namespace MmWebJob
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WriteInJob>().PropertiesAutowired().InstancePerDependency();
            builder.RegisterType<RedisHelper>().PropertiesAutowired().InstancePerDependency();
        }
    }
}
