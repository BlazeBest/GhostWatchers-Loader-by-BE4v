using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using GWLoader.Attributes;
using GWLoader.Utils;

namespace GWLoader.Modules
{
    public sealed class ModuleManager
	{
		private List<GWModule> _modules { get; } = new List<GWModule>();

		public ReadOnlyCollection<GWModule> Modules
		{
			get
			{
				return _modules.AsReadOnly();
			}
		}

		internal ReadOnlyCollection<GWModule> FindModules(string path = "Modules\\")
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
				Logs.Log("Modules folder does not exist.");
				return Modules;
			}
			foreach (string path2 in Directory.GetFiles(path))
			{
				if (Path.GetExtension(path2) == ".dll")
				{
					Assembly assembly;
                    try
					{
						assembly = AppDomain.CurrentDomain.Load(File.ReadAllBytes(path2));
					}
					catch (Exception ex)
					{
						Logs.Log("Error loading \"{0}\". Are you sure this is a valid assembly?", Path.GetFileName(path2));
						goto IL_10A;
					}
					foreach (Type type in from t in assembly.GetTypes()
										  where t.IsSubclassOf(typeof(GWModule))
										  select t)
					{
						ModuleInfoAttribute moduleInfo;
						if ((moduleInfo = (type.GetCustomAttributes(typeof(ModuleInfoAttribute), true).FirstOrDefault<object>() as ModuleInfoAttribute)) != null)
						{
							GWModule module = new GWModule(type);
							_modules.Add(module);
							module.Initialize(moduleInfo, this);
							Logs.Log("{0} loaded.", module);
						}
					}
				}
			IL_10A:;
			}
			return Modules;
		}

		public void UnloadModule(GWModule module)
		{
			if (_modules.Contains(module))
			{
				_modules.Remove(module);
				Logs.Log("{0} unloaded.", module);
			}
		}
	}
}
