using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Donteco.Security;

namespace Donteco
{
	public static class DataStorage
	{
		private static string LocalPath
		{
			get
			{
				return string.Format("{0}/{1}", Environment.CurrentDirectory, "data.json");
			}
		}

		private static void Load()
		{
			if (SteamClient.IsValid && SteamRemoteStorage.IsCloudEnabled)
			{
				if (!SteamRemoteStorage.FileExists("data.json"))
				{
					SteamRemoteStorage.FileWrite("data.json", new byte[0]);
				}
				byte[] array = SteamRemoteStorage.FileRead("data.json");
				if (array != null)
				{
					DataStorage.memoryStorage = DataStorage.Decrypt(Encoding.UTF8.GetString(array)).FromJson<Dictionary<string, string>>();
				}
			}
			else
			{
				Debug.Log("[Load] Cloud saves are disabled!");
				if (!File.Exists(DataStorage.LocalPath))
				{
					File.WriteAllText(DataStorage.LocalPath, string.Empty);
				}
				DataStorage.memoryStorage = DataStorage.Decrypt(File.ReadAllText(DataStorage.LocalPath)).FromJson<Dictionary<string, string>>();
			}
			if (DataStorage.memoryStorage == null)
			{
				DataStorage.memoryStorage = new Dictionary<string, string>();
			}
			DataStorage.loaded = true;
		}

		public static void Save()
		{
			string text = DataStorage.memoryStorage.ToJson(false);
			text = DataStorage.Encrypt(text);
			if (SteamClient.IsValid && SteamRemoteStorage.IsCloudEnabled)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				if (!SteamRemoteStorage.FileWrite("data.json", bytes))
				{
					Debug.LogError("Can't Save in Steam Cloud");
				}
			}
			File.WriteAllText("data.json", text);
		}

		public static void Set<T>(string key, T value, bool autoSave = true)
		{
			if (!DataStorage.loaded)
			{
				DataStorage.Load();
			}
			if (!DataStorage.memoryStorage.ContainsKey(key))
			{
				DataStorage.memoryStorage.Add(key, string.Empty);
			}
			DataStorage.memoryStorage[key] = value.ToJson(false);
			if (autoSave)
			{
				DataStorage.Save();
			}
		}

		public static T Get<T>(string key, T defaultValue = default(T))
		{
			if (!DataStorage.loaded)
			{
				DataStorage.Load();
			}
			if (!DataStorage.memoryStorage.ContainsKey(key))
			{
				return defaultValue;
			}
			return DataStorage.memoryStorage[key].FromJson<T>();
		}

		public static void Update<T>(string key, Func<T, T> update, bool autoSave = true)
		{
			T arg = DataStorage.Get<T>(key, default(T));
			T value = update(arg);
			DataStorage.Set<T>(key, value, autoSave);
		}

		public static T[] Find<T>(string keySearch)
		{
			return (from kv in DataStorage.memoryStorage
					where kv.Key.Contains(keySearch)
					select kv.Value.FromJson<T>()).ToArray<T>();
		}

		private static string Decrypt(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			return StringCipher.Decrypt(text, "thisIsSparta!");
		}

		private static string Encrypt(string text)
		{
			return StringCipher.Encrypt(text, "thisIsSparta!");
		}

		private const string FILE_NAME = "data.json";

		private static Dictionary<string, string> memoryStorage;

		private static bool loaded;
	}
}
