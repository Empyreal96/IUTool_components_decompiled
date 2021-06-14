using System;
using System.Reflection.Adds;
using System.Reflection.Mock;
using Microsoft.Tools.IO;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200001D RID: 29
	internal class Loader
	{
		// Token: 0x0600012E RID: 302 RVA: 0x00003251 File Offset: 0x00001451
		public Loader(IMutableTypeUniverse universe)
		{
			this._universe = universe;
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600012F RID: 303 RVA: 0x00003270 File Offset: 0x00001470
		// (set) Token: 0x06000130 RID: 304 RVA: 0x000032A2 File Offset: 0x000014A2
		public IReflectionFactory Factory
		{
			get
			{
				bool flag = this._factory == null;
				if (flag)
				{
					this._factory = new DefaultFactory();
				}
				return this._factory;
			}
			set
			{
				this._factory = value;
			}
		}

		// Token: 0x06000131 RID: 305 RVA: 0x000032AC File Offset: 0x000014AC
		private MetadataFile OpenMetadataFile(string filename)
		{
			return this._dispenser.OpenFileAsFileMapping(filename);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x000032CC File Offset: 0x000014CC
		public Assembly LoadAssemblyFromFile(string file)
		{
			MetadataFile metadataFile = this.OpenMetadataFile(file);
			Assembly assembly = AssemblyFactory.CreateAssembly(this._universe, metadataFile, this.Factory, metadataFile.FilePath);
			this._universe.AddAssembly(assembly);
			return assembly;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00003310 File Offset: 0x00001510
		public Assembly LoadAssemblyFromFile(string manifestFile, string[] netModuleFiles)
		{
			MetadataFile metadataFile = this.OpenMetadataFile(manifestFile);
			MetadataFile[] array = null;
			bool flag = netModuleFiles != null && netModuleFiles.Length != 0;
			if (flag)
			{
				array = new MetadataFile[netModuleFiles.Length];
				for (int i = 0; i < netModuleFiles.Length; i++)
				{
					array[i] = this.OpenMetadataFile(netModuleFiles[i]);
				}
			}
			Assembly assembly = AssemblyFactory.CreateAssembly(this._universe, metadataFile, array, this.Factory, metadataFile.FilePath, netModuleFiles);
			this._universe.AddAssembly(assembly);
			return assembly;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000339C File Offset: 0x0000159C
		public Assembly LoadAssemblyFromByteArray(byte[] data)
		{
			MetadataFile metadataImport = this._dispenser.OpenFromByteArray(data);
			Assembly assembly = AssemblyFactory.CreateAssembly(this._universe, metadataImport, this.Factory, string.Empty);
			this._universe.AddAssembly(assembly);
			return assembly;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x000033E4 File Offset: 0x000015E4
		public MetadataOnlyModule LoadModuleFromFile(string moduleFileName)
		{
			MetadataFile import = this._dispenser.OpenFileAsFileMapping(moduleFileName);
			return new MetadataOnlyModule(this._universe, import, this.Factory, moduleFileName);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00003418 File Offset: 0x00001618
		public Module ResolveModule(Assembly containingAssembly, string moduleName)
		{
			bool flag = containingAssembly == null || string.IsNullOrEmpty(containingAssembly.Location);
			if (flag)
			{
				throw new ArgumentException("manifestModule needs to be associated with an assembly with valid location");
			}
			string directoryName = LongPathPath.GetDirectoryName(containingAssembly.Location);
			string text = LongPathPath.Combine(directoryName, moduleName);
			MetadataFile import = this._dispenser.OpenFileAsFileMapping(text);
			MetadataOnlyModule metadataOnlyModule = new MetadataOnlyModule(this._universe, import, this.Factory, text);
			metadataOnlyModule.SetContainingAssembly(containingAssembly);
			return metadataOnlyModule;
		}

		// Token: 0x0400004F RID: 79
		private readonly IMutableTypeUniverse _universe;

		// Token: 0x04000050 RID: 80
		private readonly MetadataDispenser _dispenser = new MetadataDispenser();

		// Token: 0x04000051 RID: 81
		private IReflectionFactory _factory;
	}
}
