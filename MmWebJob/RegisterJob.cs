using Hangfire.RecurringJobExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmWebJob
{
    public class RegisterJob
    {

        public static void Register()
        {
            //需要定时处理的类
            CronJob.AddOrUpdate(typeof(WriteInJob));
        }

    }
}
