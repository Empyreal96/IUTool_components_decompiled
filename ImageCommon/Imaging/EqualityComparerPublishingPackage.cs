using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000033 RID: 51
	public class EqualityComparerPublishingPackage : EqualityComparer<PublishingPackageInfo>
	{
		// Token: 0x06000222 RID: 546 RVA: 0x0001351D File Offset: 0x0001171D
		public EqualityComparerPublishingPackage(PublishingPackageInfo.PublishingPackageInfoComparison compareType)
		{
			this._compareType = compareType;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0001352C File Offset: 0x0001172C
		public override bool Equals(PublishingPackageInfo x, PublishingPackageInfo y)
		{
			if (x == null)
			{
				return y == null;
			}
			return x.Equals(y, this._compareType);
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00013545 File Offset: 0x00011745
		public override int GetHashCode(PublishingPackageInfo pkg)
		{
			return pkg.GetHashCode(this._compareType);
		}

		// Token: 0x0400016B RID: 363
		private PublishingPackageInfo.PublishingPackageInfoComparison _compareType;
	}
}
