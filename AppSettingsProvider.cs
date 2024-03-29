﻿using Microsoft.Extensions.Configuration;

namespace db_ref_finder.Common {
	public class AppSettingsProvider {
		private readonly IConfigurationRoot configuration;

		public AppSettingsProvider(string file = "appSettings.json") {
			var filePath = Path.Combine(Environment.CurrentDirectory, file);

			configuration = new ConfigurationBuilder()
				.AddJsonFile(filePath)
				.Build();
		}

		public string GetValue(string key) {
			return configuration[key];
		}

		public int GetInt(string key) {
			return int.Parse(GetValue(key));
		}

		public double GetDouble(string key) {
			return double.Parse(GetValue(key));
		}

		public string GetConnectionString(string key = "Default") {
			return configuration.GetConnectionString(key);
		}
	}
}
