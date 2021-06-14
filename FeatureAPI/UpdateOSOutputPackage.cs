using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200000B RID: 11
	public class UpdateOSOutputPackage
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00003844 File Offset: 0x00001A44
		public override string ToString()
		{
			return this.Name + ":" + this.Partition;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002C RID: 44 RVA: 0x0000385C File Offset: 0x00001A5C
		[XmlIgnore]
		public string Name
		{
			get
			{
				return PackageTools.BuildPackageName(this.Identity.Owner, this.Identity.Component, this.Identity.SubComponent, this.Culture, this.Resolution);
			}
		}

		// Token: 0x04000035 RID: 53
		public string Description;

		// Token: 0x04000036 RID: 54
		public string UpdateState;

		// Token: 0x04000037 RID: 55
		public string PackageFile;

		// Token: 0x04000038 RID: 56
		public string PackageIdentity;

		// Token: 0x04000039 RID: 57
		public UpdateOSOutputIdentity Identity;

		// Token: 0x0400003A RID: 58
		public ReleaseType ReleaseType;

		// Token: 0x0400003B RID: 59
		public OwnerType OwnerType;

		// Token: 0x0400003C RID: 60
		public BuildType BuildType;

		// Token: 0x0400003D RID: 61
		[XmlIgnore]
		public CpuId CpuType;

		// Token: 0x0400003E RID: 62
		[XmlElement("CpuType")]
		public string CpuTypeStr;

		// Token: 0x0400003F RID: 63
		public string Culture;

		// Token: 0x04000040 RID: 64
		public string Resolution;

		// Token: 0x04000041 RID: 65
		public string Partition;

		// Token: 0x04000042 RID: 66
		[DefaultValue(false)]
		public bool IsRemoval;

		// Token: 0x04000043 RID: 67
		[DefaultValue(false)]
		public bool IsBinaryPartition;

		// Token: 0x04000044 RID: 68
		public int Result;
	}
}
