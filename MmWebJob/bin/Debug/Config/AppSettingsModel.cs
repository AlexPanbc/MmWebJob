using System.Collections.Generic;

namespace DmtMax.Common.Config
{
	public class AppSettingsModel
	{
		/// <summary>
		/// 启用帮助文档
		/// </summary>
		public bool EnableHelpDocument { get; set; }

		/// <summary>
		/// 数据库连接配置
		/// </summary>
		public List<DbConnection> DbConnections { get; set; }


		/// <summary>
		/// Redis连接字符串
		/// </summary>
		public string RedisConnectionString { get; set; }

		/// <summary>
		/// 微信配置
		/// </summary>
		public WxConfigs WxConfig { get; set; }

		/// <summary>
		/// JWT签名秘钥
		/// </summary>
		public string JWTSecurityKey { get; set; }

		/// <summary>
		/// token有效期时间（单位天）
		/// </summary>
		public double AuthTokenExpireLength { get; set; }

		/// <summary>
		/// 图片服务器域名
		/// </summary>
		public string ImgServerDomain { get; set; }

		/// <summary>
		/// H5的域名
		/// </summary>
		public string H5Url { get; set; }

		/// <summary>
		/// H5页面地址
		/// </summary>
		public string H5CommonUrl { get; set; }

		/// <summary>
		/// 极光推送，回话配置
		/// </summary>
		public JPushConfig JPushConfig { get; set; }

		/// <summary>
		/// 极光短信配置
		/// </summary>
		public JPushConfig JSmsConfig { get; set; }

		/// <summary>
		/// 问卷url  /vote/?h5id={0}&amp;tmpl=vote-template
		/// </summary>
		public string QuestionnaireUrl { get; set; }

		/// <summary>
		/// 购车计算器地址
		/// </summary>
		public string CarPriceCalculatorUrl { get; set; }

		/// <summary>
		/// 商城商品Url
		/// </summary>
		public string ProductUrl { get; set; }

		/// <summary>
		/// 短信配置
		/// </summary>
		public SMSConfigs SMSConfigs { get; set; }

		/// <summary>
		/// 阿里云oss配置
		/// </summary>
		public OSS OSS { get; set; }


		/// <summary>
		/// 素材存放的服务器位置
		/// </summary>
		public string CsMaterialFoler { get; set; }

		/// <summary>
		/// 定时任务配置
		/// </summary>
		public TaskConfig TaskConfig { get; set; }

		/// <summary>
		/// RabbitMQ配置
		/// </summary>
		public RabbitMQ RabbitMQConfig { get; set; }

		/// <summary>
		/// RabbitMQ相关的队列名字
		/// </summary>
		public Dictionary<string, string> RabbitQueues { get; set; }

		/// <summary>
		/// 一个手机号一天最多发几条验证码
		/// </summary>
		public int SmsValidCodeDailyCount { get; set; } = 3;

		/// <summary>
		/// 雪花算法配置
		/// </summary>
		public SnowflakeConfigs Snowflake { get; set; } = new SnowflakeConfigs();

		/// <summary>
		/// 当前站点域名
		/// </summary>
		public string CurrentDomain { get; set; }

		/// <summary>
		/// 销售顾问名片jssdk使用的appid
		/// </summary>
		public string ProfileWeixinAppId { get; set; }
	}

	public class DbConnection
	{
		/// <summary>
		/// 连接名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 连接字符串
		/// </summary>
		public string ConnectionString { get; set; }
	}

	public class WxConfigs
	{
		public string AppId { get; set; }

		public string AppSecret { get; set; }

		public string Token { get; set; }

		public string WeixinAESKey { get; set; }
	}

	public class JPushConfig
	{
		public string AppKey { get; set; }

		public string AppSecret { get; set; }

		/// <summary>
		/// 生成环境
		/// </summary>
		public bool Production { get; set; }
	}

	public class SMSConfigs
	{
		public string UserName { get; set; }

		public string Password { get; set; }

		public string Sign { get; set; }
	}

	public class OSS
	{
		public string AccessId { get; set; }

		public string AccessKey { get; set; }

		public string Endpoint { get; set; }

		public string BucketName { get; set; }

		/// <summary>
		/// 上传的跟目录（可以为空）
		/// </summary>
		public string UploadRootDir { get; set; }


	}

	public class HBaseConfig
	{
		public string HbaseHost { get; set; }
		public int HbasePort { get; set; }
	}


	public class TaskConfig
	{
		/// <summary>
		/// 仪表盘端口
		/// </summary>
		public int DashboardPort { get; set; } = 1234;

		/// <summary>
		/// Redis持久化到第几个库
		/// </summary>
		public int RedisStorageDb { get; set; } = 15;
	}

	public class RabbitMQ
	{
		public string HostName { get; set; }

		public int Port { get; set; } = 5672;

		public string UserName { get; set; }

		public string Password { get; set; }

		public string VirtualHost { get; set; }
	}

	public class SnowflakeConfigs
	{
		/// <summary>
		/// 工作id
		/// </summary>
		public long WorkerId { get; set; } = 1;

		/// <summary>
		/// 数据中心id
		/// </summary>
		public long DataCenterId { get; set; } = 1;
	}
}
