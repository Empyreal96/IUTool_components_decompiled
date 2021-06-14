using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x02000008 RID: 8
	public class CompDBFeaturePackage
	{
		// Token: 0x06000037 RID: 55 RVA: 0x000044A5 File Offset: 0x000026A5
		public CompDBFeaturePackage()
		{
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000044B4 File Offset: 0x000026B4
		public CompDBFeaturePackage(string id, bool featureIdentifierPackage)
		{
			this.ID = id;
			this.FIP = featureIdentifierPackage;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000044D1 File Offset: 0x000026D1
		public CompDBFeaturePackage(CompDBFeaturePackage srcPkg)
		{
			this.ID = srcPkg.ID;
			this.FIP = srcPkg.FIP;
			this.UpdateType = srcPkg.UpdateType;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00004504 File Offset: 0x00002704
		public CompDBFeaturePackage SetUpdateType(CompDBFeaturePackage.UpdateTypes type)
		{
			this.UpdateType = type;
			return this;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x0000450E File Offset: 0x0000270E
		public override string ToString()
		{
			return this.ID + (this.FIP ? "(FIP)" : "");
		}

		// Token: 0x0400002C RID: 44
		[XmlAttribute]
		public string ID;

		// Token: 0x0400002D RID: 45
		[XmlAttribute]
		[DefaultValue(false)]
		public bool FIP;

		// Token: 0x0400002E RID: 46
		[XmlAttribute]
		[DefaultValue(CompDBFeaturePackage.UpdateTypes.Canonical)]
		public CompDBFeaturePackage.UpdateTypes UpdateType = CompDBFeaturePackage.UpdateTypes.Canonical;

		// Token: 0x0400002F RID: 47
		[XmlAttribute]
		public CompDBFeaturePackage.PackageTypes PackageType;

		// Token: 0x02000047 RID: 71
		public enum UpdateTypes
		{
			// Token: 0x040001BF RID: 447
			Removal,
			// Token: 0x040001C0 RID: 448
			Diff,
			// Token: 0x040001C1 RID: 449
			Canonical,
			// Token: 0x040001C2 RID: 450
			NoUpdate
		}

		// Token: 0x02000048 RID: 72
		public enum PackageTypes
		{
			// Token: 0x040001C4 RID: 452
			FeaturePackage,
			// Token: 0x040001C5 RID: 453
			MediaFileList,
			// Token: 0x040001C6 RID: 454
			MetadataESD
		}
	}
}
