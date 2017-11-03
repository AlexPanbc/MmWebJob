using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Linq;

namespace MmWebJob
{
    /// <summary>
    /// Redis帮助类
    /// </summary>
    public class RedisHelper : IDisposable
    {
        private static ConnectionMultiplexer Redis => RedisConnectionMultiplexerManager.Create();

        public async Task<T> GetAsync<T>(string key, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var val = await database.StringGetAsync(key);
            return string.IsNullOrEmpty(val) ? default : JsonConvert.DeserializeObject<T>(val);
        }

        public T Get<T>(string key, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var val = database.StringGet(key);
            return string.IsNullOrEmpty(val) ? default : JsonConvert.DeserializeObject<T>(val);
        }


        public async Task<Dictionary<string, T>> Get<T>(RedisDb db = RedisDb.Default, params string[] keys)
        {
            var database = Redis.GetDatabase((int)db);
            var rediskeys = keys.Select(p => (RedisKey)p).ToArray();
            var val = await database.StringGetAsync(rediskeys);
            var dict = new Dictionary<string, T>();
            for (var index = 0; index < rediskeys.Length; index++)
            {
                dict.Add(rediskeys[index], val[index] == RedisValue.Null ? default(T) : JsonConvert.DeserializeObject<T>(val[index]));
            }
            return dict;
        }

        public async Task HashSetAsync<T>(string key, string field, T value, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            await database.HashSetAsync(key, field, JsonConvert.SerializeObject(value));
        }

        public void HashSet<T>(string key, string field, T value, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            database.HashSet(key, field, JsonConvert.SerializeObject(value));
        }

        public async Task HashSetAsync<T>(string key, IDictionary<string, T> values, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            await
                database.HashSetAsync(key,
                    values.Select(p => new HashEntry(p.Key, JsonConvert.SerializeObject(p.Value))).ToArray());
        }

        public void HashSet<T>(string key, IDictionary<string, T> values, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            database.HashSet(key,
                values.Select(p => new HashEntry(p.Key, JsonConvert.SerializeObject(p.Value))).ToArray());
        }

        public async Task HashSet<T>(string key, string field, T value, DateTime expireAt, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var batch = database.CreateBatch();
            var addTasks = new Task[]
            {
                batch.HashSetAsync(key, field, JsonConvert.SerializeObject(value)),
                batch.KeyExpireAsync(key, expireAt.ToUniversalTime())
            };
            batch.Execute();
            Task.WaitAll(addTasks);
        }

        public async Task HashSet<T>(string key, string field, T value, TimeSpan expireIn, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var batch = database.CreateBatch();
            var addTasks = new Task[]
            {
                batch.HashSetAsync(key, field, JsonConvert.SerializeObject(value)),
                batch.KeyExpireAsync(key, expireIn)
            };
            batch.Execute();
            Task.WaitAll(addTasks);
        }

        public async Task HashSet<T>(string key, IDictionary<string, T> values, DateTime expireAt,
            RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var batch = database.CreateBatch();
            var addTasks = new List<Task>
            {
                batch.HashSetAsync(key,
                    values.Select(i => new HashEntry(i.Key, JsonConvert.SerializeObject(i.Value))).ToArray()),
                batch.KeyExpireAsync(key, expireAt - DateTime.Now)
            };
            batch.Execute();
            Task.WaitAll(addTasks.ToArray());
        }

        public async Task HashSet<T>(string key, IDictionary<string, T> values, TimeSpan expireIn,
            RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var batch = database.CreateBatch();
            var addTasks = new List<Task>
            {
                batch.HashSetAsync(key,
                    values.Select(i => new HashEntry(i.Key, JsonConvert.SerializeObject(i.Value))).ToArray()),
                batch.KeyExpireAsync(key, expireIn)
            };
            batch.Execute();
            Task.WaitAll(addTasks.ToArray());
        }


        public async Task<long> HashIncr(string key, string field, long value, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            return await database.HashIncrementAsync(key, field, value);
        }

        public async Task<long> HashDecr(string key, string field, long value, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            return await database.HashDecrementAsync(key, field, value);
        }

        public long HashDecrl(string key, string field, long value, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            return database.HashDecrement(key, field, value);
        }

        public async Task<Dictionary<string, T>> HashGet<T>(string key, RedisDb db = RedisDb.Default,
            params string[] field)
        {
            var database = Redis.GetDatabase((int)db);
            var values = await database.HashGetAsync(key, field.Select(k => (RedisValue)k).ToArray());
            var dict = new Dictionary<string, T>();

            for (var index = 0; index < field.Length; index++)
                dict.Add(field[index], values[index] == RedisValue.Null ? default(T) : JsonConvert.DeserializeObject<T>(values[index]));
            return dict;
        }

        public async Task<IDictionary<string, dynamic>> HashGet(string key, RedisDb db = RedisDb.Default,
            params string[] field)
        {
            var database = Redis.GetDatabase((int)db);
            var values = await database.HashGetAsync(key, field.Select(k => (RedisValue)k).ToArray());
            var dict = new Dictionary<string, dynamic>();
            for (var index = 0; index <= field.Length; index++)
                dict.Add(field[index], values[index]);
            return dict;
        }


        public async Task<T> HashGetAsync<T>(string key, string field, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var value = await database.HashGetAsync(key, field);
            return string.IsNullOrEmpty(value) ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public T HashGet<T>(string key, string field, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var value = database.HashGet(key, field);
            return string.IsNullOrEmpty(value) ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public async Task<List<T>> HashGet<T>(string key, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var values = await database.HashValuesAsync(key);
            return
                values?.Select(p => string.IsNullOrEmpty(p) ? default(T) : JsonConvert.DeserializeObject<T>(p)).ToList();
        }

        public async Task<long> HashLength(string key, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            return await database.HashLengthAsync(key);
        }

        public async Task<List<T>> HashKeys<T>(string key, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var keys = await database.HashKeysAsync(key);
            return keys.Select(p => JsonConvert.DeserializeObject<T>(p)).ToList();
        }

        public async Task<IDictionary<string, T>> HashGetAllAsync<T>(string key, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var value = await database.HashGetAllAsync(key);
            if (value == null || !value.Any())
                return null;
            var dic = new ConcurrentDictionary<string, T>();
            foreach (var i in value)
                dic.TryAdd(i.Name,
                    string.IsNullOrEmpty(i.Value) ? default(T) : JsonConvert.DeserializeObject<T>(i.Value));
            return dic;
        }

        public IDictionary<string, T> HashGetAll<T>(string key, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var value = database.HashGetAll(key);
            if (value == null || !value.Any())
                return null;
            var dic = new ConcurrentDictionary<string, T>();
            foreach (var i in value)
                dic.TryAdd(i.Name,
                    string.IsNullOrEmpty(i.Value) ? default(T) : JsonConvert.DeserializeObject<T>(i.Value));
            return dic;
        }

        public async Task<Dictionary<string, dynamic>> HashGetAll(string key, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var value = await database.HashGetAllAsync(key);
            if (value == null || !value.Any())
                return null;
            return value.ToDictionary(item => item.Name.ToString(), item => (dynamic)item.Value);
        }


        public async Task<bool> HashRemove(string key, RedisDb db = RedisDb.Default, params string[] fields)
        {
            var database = Redis.GetDatabase((int)db);
            return
                await
                    database.HashDeleteAsync(key, fields.Select(p => (RedisValue)p).ToArray()) > 0;
        }

        public async Task<bool> HasExist(string key, string field, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            return await database.HashExistsAsync(key, field);
        }


        /// <summary>
        /// 加锁操作
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="redisAction"></param>
        /// <param name="lockTime">锁定时长默认100ms</param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task LockOperate(string key, Action<IDatabase> redisAction, RedisDb db = RedisDb.Default,
            int lockTime = 100)
        {
            var database = Redis.GetDatabase((int)db);
            var token = Guid.NewGuid().ToString();
            try
            {
                var locks = await database.LockTakeAsync($"{key}_lock", token, TimeSpan.FromMilliseconds(lockTime));
                if (locks)
                    redisAction(database);
            }
            finally
            {
                await database.LockReleaseAsync($"{key}_lock", token);
            }
        }

        /// <summary>
        /// 加锁操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="redisAction"></param>
        /// <param name="db"></param>
        /// <param name="timeOut">锁定时长默认100ms</param>
        /// <returns></returns>
        public async Task<T> LockOperate<T>(string key, Func<IDatabase, T> redisAction, RedisDb db = RedisDb.Default,
            int timeOut = 100)
        {
            var database = Redis.GetDatabase((int)db);
            var token = Guid.NewGuid().ToString();
            try
            {
                var locks = await database.LockTakeAsync($"{key}_lock", token, TimeSpan.FromMilliseconds(timeOut));
                return locks ? redisAction(database) : default(T);
            }
            finally
            {
                await database.LockReleaseAsync($"{key}_lock", token);
            }
        }


        public async Task<long> HashDecrWithLock<T>(string key, string field, long value, Func<T, bool> func,
            RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            RedisValue token = Guid.NewGuid().ToString(); // Environment.MachineName;
            try
            {
                await database.LockTakeAsync(key, token, TimeSpan.FromMilliseconds(100));
                var hashValue = await database.HashGetAsync(key, field);
                var validateValue = JsonConvert.DeserializeObject<T>(hashValue);
                if (!func(validateValue)) //如果当前库存小于等于0则返回-1 标识未中奖
                {
                    return -1;
                }
                return await database.HashDecrementAsync(key, field, value);
            }
            finally
            {
                await database.LockReleaseAsync(key, token);
            }
        }


        public async Task<bool> SortSet<T>(string key, double score, T value, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            return await database.SortedSetAddAsync(key, JsonConvert.SerializeObject(value), score);
        }

        //public async Task<bool> SortSet<T>(string key, RedisDb db = RedisDb.Default)
        //{
        //    var database = Redis.GetDatabase((int) db);
        //    return await database.SortedSetAddAsync(key, JsonConvert.SerializeObject(value), score);
        //}

        //public async Task<bool> SortSet<T>(string key, RedisDb db = RedisDb.Default)
        //{
        //    var database = Redis.GetDatabase((int) db);
        //    return await database.SortedSetIncrementAsync(key, JsonConvert.SerializeObject(value), score);
        //}

        public async Task<long> Incr(string key, long value, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            return await database.StringIncrementAsync(key, value);
        }

        public async Task<long> Decr(string key, long value, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            return await database.StringDecrementAsync(key, value);
        }


        public async Task<bool> SetAsync<T>(string key, T value, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            return await database.StringSetAsync(key, JsonConvert.SerializeObject(value));
        }

        public bool Set<T>(string key, T value, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            return database.StringSet(key, JsonConvert.SerializeObject(value));
        }

        public async Task<bool> Set<T>(string key, T value, TimeSpan expireIn, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            return await database.StringSetAsync(key, JsonConvert.SerializeObject(value), expireIn);
        }


        public async Task<bool> Set<T>(string key, T value, DateTime expireAt, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            return await database.StringSetAsync(key, JsonConvert.SerializeObject(value), expireAt - DateTime.Now);
        }

        public async Task<bool> Set<T>(IDictionary<string, T> values, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var val =
                values.Select(p => new KeyValuePair<RedisKey, RedisValue>(p.Key, JsonConvert.SerializeObject(p.Value)))
                    .ToArray();
            return await database.StringSetAsync(val);
        }

        public async Task<bool> Set<T>(IDictionary<string, T> values, TimeSpan expireIn, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var batch = database.CreateBatch();
            var addTasks =
                values.Select(i => batch.StringSetAsync(i.Key, JsonConvert.SerializeObject(i.Value), expireIn))
                    .Cast<Task>()
                    .ToArray();
            batch.Execute();
            Task.WaitAll(addTasks);
            return true;
        }

        public async Task<bool> Set<T>(IDictionary<string, T> values, DateTime expireAt, RedisDb db = RedisDb.Default)
        {
            var database = Redis.GetDatabase((int)db);
            var batch = database.CreateBatch();
            var expire = expireAt - DateTime.Now;
            var addTasks =
                values.Select(i => batch.StringSetAsync(i.Key, JsonConvert.SerializeObject(i.Value), expire))
                    .Cast<Task>()
                    .ToArray();
            batch.Execute();
            Task.WaitAll(addTasks);
            return true;
        }


        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireIn"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<bool> SetExpirIn(string key, TimeSpan expireIn, RedisDb db = RedisDb.Default)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("redis key 不能为空");
            var database = Redis.GetDatabase((int)db);
            return await database.KeyExpireAsync(key, expireIn);
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireAt"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<bool> SetExpirAt(string key, DateTime expireAt, RedisDb db = RedisDb.Default)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("redis key 不能为空");
            var database = Redis.GetDatabase((int)db);
            return await database.KeyExpireAsync(key, expireAt);
        }

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="db"></param>
        /// <param name="operates"></param>
        /// <returns></returns>
        public async Task MultiOperate(RedisDb db, Action<IDatabase> operates)
        {
            var database = Redis.GetDatabase((int)db);
            operates(database);
        }

        public async Task<T> MultiOperate<T>(RedisDb db, Func<IDatabase, T> operates)
        {
            var database = Redis.GetDatabase((int)db);
            return operates(database);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keys"></param>
        public async Task Remove(params string[] keys)
        {
            var database = Redis.GetDatabase((int)RedisDb.Default);
            var redisKeys = keys.Select(p => (RedisKey)p).ToArray();
            await database.KeyDeleteAsync(redisKeys);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="db"></param>
        /// <param name="keys"></param>
        public async Task Remove(RedisDb db, params string[] keys)
        {
            var database = Redis.GetDatabase((int)db);
            var redisKeys = keys.Select(p => (RedisKey)p).ToArray();
            await database.KeyDeleteAsync(redisKeys);
        }


        /// <summary>
        /// 异步订阅消息
        /// </summary>
        /// <param name="channelName">通道</param>
        /// <param name="subscriber">订阅回调</param>
        /// <returns></returns>
        public async Task<ISubscriber> SubscribeAsync(string channelName, Action<string, string> subscriber)
        {
            var sub = Redis.GetSubscriber();
            await sub.SubscribeAsync(channelName, (channel, message) => { subscriber(channel, message); });
            return sub;
        }
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="channelName">通道</param>
        /// <param name="subscriber">订阅回调</param>
        /// <returns></returns>
        public ISubscriber Subscribe(string channelName, Action<string, string> subscriber)
        {
            var sub = Redis.GetSubscriber();
            sub.Subscribe(channelName, (channel, message) => { subscriber(channel, message); });
            return sub;
        }


        /// <summary>
        /// Redis库
        /// </summary>


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            //_redis?.Dispose();
        }
    }
    public enum RedisDb
    {
        /// <summary>
        /// 默认库
        /// </summary>
        Default = 0
    }
    /// <summary>
    /// Redis连接管理器
    /// </summary>
    public class RedisConnectionMultiplexerManager
    {
        private static ConnectionMultiplexer _instance;

        private RedisConnectionMultiplexerManager()
        {
        }

        /// <summary>
        /// 创建连接管理器
        /// </summary>
        /// <returns></returns>
        public static ConnectionMultiplexer Create()
        {
            if (_instance != null)
                return _instance;
            _instance = ConnectionMultiplexer.Connect("110.110.110.110:6379,password=110");
            return _instance;
        }
    }
}