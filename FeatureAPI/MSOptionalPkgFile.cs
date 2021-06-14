using System;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000019 RID: 25
	public class MSOptionalPkgFile : OptionalPkgFile
	{
		// Token: 0x0600009C RID: 156 RVA: 0x0000721E File Offset: 0x0000541E
		public MSOptionalPkgFile() : base(FeatureManifest.PackageGroups.MSFEATURE)
		{
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00007228 File Offset: 0x00005428
		public MSOptionalPkgFile(OptionalPkgFile srcPkg) : base(srcPkg)
		{
			this.FMGroup = FeatureManifest.PackageGroups.MSFEATURE;
		}
	}
}
