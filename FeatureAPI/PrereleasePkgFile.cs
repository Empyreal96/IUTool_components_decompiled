using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000014 RID: 20
	public class PrereleasePkgFile : PkgFile
	{
		// Token: 0x06000088 RID: 136 RVA: 0x000070B7 File Offset: 0x000052B7
		public PrereleasePkgFile() : base(FeatureManifest.PackageGroups.PRERELEASE)
		{
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000089 RID: 137 RVA: 0x000070C1 File Offset: 0x000052C1
		// (set) Token: 0x0600008A RID: 138 RVA: 0x000070C9 File Offset: 0x000052C9
		[XmlIgnore]
		public override string GroupValue
		{
			get
			{
				return this.Type;
			}
			set
			{
				this.Type = value;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000070D4 File Offset: 0x000052D4
		public new void CopyPkgFile(PkgFile srcPkgFile)
		{
			base.CopyPkgFile(srcPkgFile);
			PrereleasePkgFile prereleasePkgFile = srcPkgFile as PrereleasePkgFile;
			this.Type = prereleasePkgFile.Type;
		}

		// Token: 0x0400008C RID: 140
		[XmlAttribute("Type")]
		public string Type;
	}
}
