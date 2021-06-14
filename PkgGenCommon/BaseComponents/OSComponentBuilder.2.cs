using System;
using System.Collections.Generic;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000041 RID: 65
	public abstract class OSComponentBuilder<T, V> : PkgObjectBuilder<T, V> where T : OSComponentPkgObject, new() where V : OSComponentBuilder<T, V>
	{
		// Token: 0x060000FD RID: 253 RVA: 0x000057C8 File Offset: 0x000039C8
		internal OSComponentBuilder()
		{
			this.fileGroups = new List<FileGroupBuilder>();
			this.registryGroups = new List<RegistryKeyGroupBuilder>();
			this.registryImports = new List<string>();
		}

		// Token: 0x060000FE RID: 254 RVA: 0x000057F4 File Offset: 0x000039F4
		public FileGroupBuilder AddFileGroup()
		{
			FileGroupBuilder fileGroupBuilder = new FileGroupBuilder();
			this.fileGroups.Add(fileGroupBuilder);
			return fileGroupBuilder;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00005814 File Offset: 0x00003A14
		public V AddRegistryImport(string source)
		{
			this.registryImports.Add(source);
			return (V)((object)this);
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00005828 File Offset: 0x00003A28
		public RegistryKeyGroupBuilder AddRegistryGroup()
		{
			RegistryKeyGroupBuilder registryKeyGroupBuilder = new RegistryKeyGroupBuilder();
			this.registryGroups.Add(registryKeyGroupBuilder);
			return registryKeyGroupBuilder;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00005848 File Offset: 0x00003A48
		public override T ToPkgObject()
		{
			base.RegisterMacro("runtime.default", "$(runtime.system32)");
			base.RegisterMacro("env.default", "$(env.system32)");
			this.pkgObject.FileGroups.Clear();
			this.pkgObject.KeyGroups.Clear();
			this.pkgObject.RegImports.Clear();
			this.fileGroups.ForEach(delegate(FileGroupBuilder file)
			{
				this.pkgObject.FileGroups.Add(file.ToPkgObject());
			});
			this.registryGroups.ForEach(delegate(RegistryKeyGroupBuilder regKey)
			{
				this.pkgObject.KeyGroups.Add(regKey.ToPkgObject());
			});
			this.registryImports.ForEach(delegate(string import)
			{
				this.pkgObject.RegImports.Add(new RegImport(import));
			});
			return base.ToPkgObject();
		}

		// Token: 0x040000EC RID: 236
		private List<FileGroupBuilder> fileGroups;

		// Token: 0x040000ED RID: 237
		private List<RegistryKeyGroupBuilder> registryGroups;

		// Token: 0x040000EE RID: 238
		private List<string> registryImports;
	}
}
