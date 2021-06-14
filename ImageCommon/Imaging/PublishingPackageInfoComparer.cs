using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000032 RID: 50
	public class PublishingPackageInfoComparer : EqualityComparer<PublishingPackageInfo>
	{
		// Token: 0x0600021C RID: 540 RVA: 0x000134FD File Offset: 0x000116FD
		protected PublishingPackageInfoComparer()
		{
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600021D RID: 541 RVA: 0x00013505 File Offset: 0x00011705
		public static EqualityComparer<PublishingPackageInfo> IgnorePaths
		{
			get
			{
				return new EqualityComparerPublishingPackage(PublishingPackageInfo.PublishingPackageInfoComparison.IgnorePaths);
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600021E RID: 542 RVA: 0x0001350D File Offset: 0x0001170D
		public static EqualityComparer<PublishingPackageInfo> UniqueID
		{
			get
			{
				return new EqualityComparerPublishingPackage(PublishingPackageInfo.PublishingPackageInfoComparison.OnlyUniqueID);
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600021F RID: 543 RVA: 0x00013515 File Offset: 0x00011715
		public static EqualityComparer<PublishingPackageInfo> UniqueIDAndFeatureID
		{
			get
			{
				return new EqualityComparerPublishingPackage(PublishingPackageInfo.PublishingPackageInfoComparison.OnlyUniqueIDAndFeatureID);
			}
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000529D File Offset: 0x0000349D
		public override bool Equals(PublishingPackageInfo x, PublishingPackageInfo y)
		{
			if (x == null)
			{
				return y == null;
			}
			return x.Equals(y);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x000052B0 File Offset: 0x000034B0
		public override int GetHashCode(PublishingPackageInfo pkg)
		{
			return pkg.GetHashCode();
		}
	}
}
