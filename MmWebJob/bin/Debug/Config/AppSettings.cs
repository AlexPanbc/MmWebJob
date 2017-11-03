using System.IO;
using Newtonsoft.Json;

namespace DmtMax.Common.Config
{
	/// <summary>
	/// 系统配置
	/// </summary>
	public class AppSettings
	{
		private static AppSettingsModel _instance;
		private static readonly object Lock = new object();
		public static AppSettingsModel Instance => Get();

		private AppSettings()
		{
		}

		/// <summary>
		/// 获取配置项
		/// </summary>
		/// <returns></returns>
		protected static AppSettingsModel Get()
		{
			if (_instance != null) return _instance;
			lock (Lock)
			{
				if (_instance == null)
				{
					_instance = LoadConfigs();
				}
			}
			return _instance;
		}

		/// <summary>
		/// 重置配置项
		/// </summary>
		public static void Rest()
		{
			_instance = null;
		}

		private static AppSettingsModel LoadConfigs()
		{
			var configpath = Path.Combine(Directory.GetCurrentDirectory(), BuilderConfig.ConfigFilePath);
			if (!File.Exists(configpath))
				throw new FileNotFoundException($"未找到配置文件:{BuilderConfig.ConfigFilePath}");
			return JsonConvert.DeserializeObject<AppSettingsModel>(File.ReadAllText(configpath));
		}
	}
}