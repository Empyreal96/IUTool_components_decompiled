using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x0200000B RID: 11
	public class EqualityComparerCompDBPackage : EqualityComparer<CompDBPackageInfo>
	{
		// Token: 0x0600006C RID: 108 RVA: 0x000052B8 File Offset: 0x000034B8
		public EqualityComparerCompDBPackage(CompDBPackageInfo.CompDBPackageInfoComparison compareType)
		{
			this._compareType = compareType;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000052C7 File Offset: 0x000034C7
		public override bool Equals(CompDBPackageInfo x, CompDBPackageInfo y)
		{
			if (x == null)
			{
				return y == null;
			}
			return x.Equals(y, this._compareType);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000052E0 File Offset: 0x000034E0
		public override int GetHashCode(CompDBPackageInfo pkg)
		{
			return pkg.GetHashCode(this._compareType);
		}

		// Token: 0x04000040 RID: 64
		private CompDBPackageInfo.CompDBPackageInfoComparison _compareType;
	}
}
