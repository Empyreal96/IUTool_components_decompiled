using System;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200000E RID: 14
	internal class DefaultUniverse : SimpleUniverse
	{
		// Token: 0x06000058 RID: 88 RVA: 0x00002BE9 File Offset: 0x00000DE9
		public DefaultUniverse()
		{
			this.Loader = new Loader(this);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002C00 File Offset: 0x00000E00
		public override Module ResolveModule(Assembly containingAssembly, string moduleName)
		{
			return this.Loader.ResolveModule(containingAssembly, moduleName);
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00002C1F File Offset: 0x00000E1F
		public Loader Loader { get; }

		// Token: 0x0600005B RID: 91 RVA: 0x00002C28 File Offset: 0x00000E28
		internal Assembly LoadAssemblyFromFile(string manifestFileName, string[] netModuleFileNames)
		{
			return this.Loader.LoadAssemblyFromFile(manifestFileName, netModuleFileNames);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002C48 File Offset: 0x00000E48
		internal Assembly LoadAssemblyFromFile(string manifestFileName)
		{
			return this.Loader.LoadAssemblyFromFile(manifestFileName);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002C68 File Offset: 0x00000E68
		internal MetadataOnlyModule LoadModuleFromFile(string netModulePath)
		{
			return this.Loader.LoadModuleFromFile(netModulePath);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00002C88 File Offset: 0x00000E88
		internal Assembly LoadAssemblyFromByteArray(byte[] data)
		{
			return this.Loader.LoadAssemblyFromByteArray(data);
		}
	}
}
