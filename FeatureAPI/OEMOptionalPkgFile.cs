using System;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200001A RID: 26
	public class OEMOptionalPkgFile : OptionalPkgFile
	{
		// Token: 0x0600009E RID: 158 RVA: 0x00007239 File Offset: 0x00005439
		public OEMOptionalPkgFile() : base(FeatureManifest.PackageGroups.OEMFEATURE)
		{
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00007243 File Offset: 0x00005443
		public OEMOptionalPkgFile(OptionalPkgFile srcPkg) : base(srcPkg)
		{
			this.FMGroup = FeatureManifest.PackageGroups.OEMFEATURE;
		}
	}
}
