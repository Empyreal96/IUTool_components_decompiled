using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000017 RID: 23
	public class ReleasePkgFile : PkgFile
	{
		// Token: 0x06000093 RID: 147 RVA: 0x00007159 File Offset: 0x00005359
		public ReleasePkgFile() : base(FeatureManifest.PackageGroups.RELEASE)
		{
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00007162 File Offset: 0x00005362
		// (set) Token: 0x06000095 RID: 149 RVA: 0x0000716A File Offset: 0x0000536A
		[XmlIgnore]
		public override string GroupValue
		{
			get
			{
				return this.ReleaseType;
			}
			set
			{
				this.ReleaseType = value;
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00007174 File Offset: 0x00005374
		public new void CopyPkgFile(PkgFile srcPkgFile)
		{
			base.CopyPkgFile(srcPkgFile);
			ReleasePkgFile releasePkgFile = srcPkgFile as ReleasePkgFile;
			this.ReleaseType = releasePkgFile.ReleaseType;
		}

		// Token: 0x0400008E RID: 142
		[XmlAttribute("ReleaseType")]
		public string ReleaseType;
	}
}
