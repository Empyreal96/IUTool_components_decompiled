using System;
using System.Collections.Generic;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000047 RID: 71
	public sealed class RegistryKeyGroupBuilder : FilterGroupBuilder<RegGroup, RegistryKeyGroupBuilder>
	{
		// Token: 0x06000128 RID: 296 RVA: 0x00005BB3 File Offset: 0x00003DB3
		public RegistryKeyGroupBuilder()
		{
			this.regKeys = new List<RegistryKeyBuilder>();
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00005BC6 File Offset: 0x00003DC6
		public RegistryKeyBuilder AddRegistryKey(string keyName, params object[] args)
		{
			return this.AddRegistryKey(string.Format(keyName, args));
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00005BD8 File Offset: 0x00003DD8
		public RegistryKeyBuilder AddRegistryKey(string keyName)
		{
			RegistryKeyBuilder registryKeyBuilder = new RegistryKeyBuilder(keyName);
			this.regKeys.Add(registryKeyBuilder);
			return registryKeyBuilder;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00005BF9 File Offset: 0x00003DF9
		public override RegGroup ToPkgObject()
		{
			this.filterGroup.Keys.Clear();
			this.regKeys.ForEach(delegate(RegistryKeyBuilder x)
			{
				this.filterGroup.Keys.Add(x.ToPkgObject());
			});
			return base.ToPkgObject();
		}

		// Token: 0x040000F5 RID: 245
		private List<RegistryKeyBuilder> regKeys;
	}
}
