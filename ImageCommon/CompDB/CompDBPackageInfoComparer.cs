using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x0200000A RID: 10
	public class CompDBPackageInfoComparer : EqualityComparer<CompDBPackageInfo>
	{
		// Token: 0x06000064 RID: 100 RVA: 0x0000526D File Offset: 0x0000346D
		protected CompDBPackageInfoComparer()
		{
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00005275 File Offset: 0x00003475
		public static EqualityComparer<CompDBPackageInfo> Standard
		{
			get
			{
				return new EqualityComparerCompDBPackage(CompDBPackageInfo.CompDBPackageInfoComparison.Standard);
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000066 RID: 102 RVA: 0x0000527D File Offset: 0x0000347D
		public static EqualityComparer<CompDBPackageInfo> IgnorePackageHash
		{
			get
			{
				return new EqualityComparerCompDBPackage(CompDBPackageInfo.CompDBPackageInfoComparison.IgnorePayloadHashes);
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000067 RID: 103 RVA: 0x00005285 File Offset: 0x00003485
		public static EqualityComparer<CompDBPackageInfo> IgnorePaths
		{
			get
			{
				return new EqualityComparerCompDBPackage(CompDBPackageInfo.CompDBPackageInfoComparison.IgnorePayloadPaths);
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000068 RID: 104 RVA: 0x0000528D File Offset: 0x0000348D
		public static EqualityComparer<CompDBPackageInfo> UniqueID
		{
			get
			{
				return new EqualityComparerCompDBPackage(CompDBPackageInfo.CompDBPackageInfoComparison.OnlyUniqueID);
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00005295 File Offset: 0x00003495
		public static EqualityComparer<CompDBPackageInfo> UniqueIDAndFeatureID
		{
			get
			{
				return new EqualityComparerCompDBPackage(CompDBPackageInfo.CompDBPackageInfoComparison.OnlyUniqueIDAndFeatureID);
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000529D File Offset: 0x0000349D
		public override bool Equals(CompDBPackageInfo x, CompDBPackageInfo y)
		{
			if (x == null)
			{
				return y == null;
			}
			return x.Equals(y);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000052B0 File Offset: 0x000034B0
		public override int GetHashCode(CompDBPackageInfo pkg)
		{
			return pkg.GetHashCode();
		}
	}
}
