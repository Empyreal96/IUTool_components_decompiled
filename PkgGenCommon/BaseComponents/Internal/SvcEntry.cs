using System;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000078 RID: 120
	public class SvcEntry : PkgFile
	{
		// Token: 0x0600029F RID: 671 RVA: 0x0000A228 File Offset: 0x00008428
		public override void Build(IPackageGenerator pkgGen, SatelliteId satelliteId)
		{
			SvcDll svcDll = this as SvcDll;
			bool flag = false;
			if (satelliteId != SatelliteId.Neutral)
			{
				throw new PkgGenException("SvcDll object should not be langauge/resolution specific", new object[0]);
			}
			if (svcDll != null)
			{
				flag = svcDll.BinaryInOneCorePkg;
			}
			if (!flag)
			{
				base.Build(pkgGen, SatelliteId.Neutral);
			}
		}
	}
}
