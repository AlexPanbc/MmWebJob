
using Hangfire.RecurringJobExtensions;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MmWebJob
{
    /// <summary>
    /// 定时任务 具体要执行业务处理的类
    /// 多种业务可以分开多个类  来处理
    /// 需要在RegisterJob 调用
    /// 在AutofacModule注入即可使用
    /// </summary>
    public class WriteInJob
    {
        private RedisHelper RedisHelper { get; }
        public WriteInJob(RedisHelper redisHelper)
        {
            RedisHelper = redisHelper;
        }

        /// <summary>
        /// 每隔1分钟处理一次
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [RecurringJob("*/1 * * * *", TimeZone = "China Standard Time"), DisplayName("每隔1分钟处理一次")]
        public async Task Write()
        {
            int count = Read() + 1;
            FileStream fs = new FileStream("D:\\wushan.txt", FileMode.Create);
            byte[] data = Encoding.Default.GetBytes(count.ToString());
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
            RedisHelper.Set("jobkey", count, RedisDb.Default);

        }
        public int Read()
        {
            StreamReader fileStream = null;
            try
            {
                string a = null;
                fileStream = new StreamReader("D:\\wushan.txt", Encoding.Default);
                a = fileStream.ReadToEnd();
                return int.Parse(a != null ? a : "0");
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                fileStream?.Close();
            }
        }
    }
}
