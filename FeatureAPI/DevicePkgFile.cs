using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000015 RID: 21
	public class DevicePkgFile : PkgFile
	{
		// Token: 0x0600008C RID: 140 RVA: 0x000070FB File Offset: 0x000052FB
		public DevicePkgFile() : base(FeatureManifest.PackageGroups.DEVICE)
		{
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00007104 File Offset: 0x00005304
		public DevicePkgFile(FeatureManifest.PackageGroups fmGroup) : base(fmGroup)
		{
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600008E RID: 142 RVA: 0x0000710D File Offset: 0x0000530D
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00007115 File Offset: 0x00005315
		[XmlIgnore]
		public override string GroupValue
		{
			get
			{
				return this.Device;
			}
			set
			{
				this.Device = value;
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00007120 File Offset: 0x00005320
		public new void CopyPkgFile(PkgFile srcPkgFile)
		{
			base.CopyPkgFile(srcPkgFile);
			DevicePkgFile devicePkgFile = srcPkgFile as DevicePkgFile;
			this.Device = devicePkgFile.Device;
		}

		// Token: 0x0400008D RID: 141
		[XmlAttribute("Device")]
		public string Device;
	}
}
