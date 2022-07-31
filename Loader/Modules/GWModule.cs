using System;
using System.Security.Policy;
using GWLoader.Attributes;

namespace GWLoader.Modules
{
	public class GWModule
	{
		public GWModule()
        {

        }

		internal GWModule(Type type)
        {
			this.type = type;
        }

		public Type type { get; private set; }

		public string Name { get; private set; }

		public string Version { get; private set; }

		public string Author { get; private set; }

		public ModuleManager ModuleManager { get; private set; }

		public void Unload()
		{
			ModuleManager.UnloadModule(this);
		}

		internal void Initialize(ModuleInfoAttribute moduleInfo, ModuleManager moduleManager)
		{
			Name = moduleInfo.Name;
			Version = moduleInfo.Version;
			Author = moduleInfo.Author;
			ModuleManager = moduleManager;
		}

		public override string ToString()
		{
			return string.Format("{0} ({1}) by {2}", Name, Version, Author);
		}
	}
}
