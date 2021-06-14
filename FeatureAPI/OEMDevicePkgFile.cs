using System;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000016 RID: 22
	public class OEMDevicePkgFile : DevicePkgFile
	{
		// Token: 0x06000091 RID: 145 RVA: 0x00007147 File Offset: 0x00005347
		public OEMDevicePkgFile() : base(FeatureManifest.PackageGroups.OEMDEVICEPLATFORM)
		{
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00007150 File Offset: 0x00005350
		public new void CopyPkgFile(PkgFile srcPkgFile)
		{
			base.CopyPkgFile(srcPkgFile);
		}
	}
}
