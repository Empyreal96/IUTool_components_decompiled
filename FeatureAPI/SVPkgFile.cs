using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200001B RID: 27
	public class SVPkgFile : PkgFile
	{
		// Token: 0x060000A0 RID: 160 RVA: 0x00007254 File Offset: 0x00005454
		public SVPkgFile() : base(FeatureManifest.PackageGroups.SV)
		{
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x0000725D File Offset: 0x0000545D
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x00007265 File Offset: 0x00005465
		[XmlIgnore]
		public override string GroupValue
		{
			get
			{
				return this.SV;
			}
			set
			{
				this.SV = value;
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00007270 File Offset: 0x00005470
		public new void CopyPkgFile(PkgFile srcPkgFile)
		{
			base.CopyPkgFile(srcPkgFile);
			SVPkgFile svpkgFile = srcPkgFile as SVPkgFile;
			this.SV = svpkgFile.SV;
		}

		// Token: 0x04000090 RID: 144
		[XmlAttribute("SV")]
		public string SV;
	}
}
