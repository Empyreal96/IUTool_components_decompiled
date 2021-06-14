using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200001C RID: 28
	public class SOCPkgFile : PkgFile
	{
		// Token: 0x060000A4 RID: 164 RVA: 0x00007104 File Offset: 0x00005304
		public SOCPkgFile(FeatureManifest.PackageGroups fmGroup) : base(fmGroup)
		{
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00007297 File Offset: 0x00005497
		public SOCPkgFile() : base(FeatureManifest.PackageGroups.SOC)
		{
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x000072A0 File Offset: 0x000054A0
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x000072A8 File Offset: 0x000054A8
		[XmlIgnore]
		public override string GroupValue
		{
			get
			{
				return this.SOC;
			}
			set
			{
				this.SOC = value;
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000072B4 File Offset: 0x000054B4
		public new void CopyPkgFile(PkgFile srcPkgFile)
		{
			base.CopyPkgFile(srcPkgFile);
			SOCPkgFile socpkgFile = srcPkgFile as SOCPkgFile;
			this.SOC = socpkgFile.SOC;
		}

		// Token: 0x04000091 RID: 145
		[XmlAttribute("SOC")]
		public string SOC;
	}
}
