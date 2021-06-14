using System;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000060 RID: 96
	[XmlRoot(ElementName = "BootConfigurationDatabase", Namespace = "http://schemas.microsoft.com/phone/2011/10/BootConfiguration", IsNullable = false)]
	public class BcdInput
	{
		// Token: 0x0600045D RID: 1117 RVA: 0x000139A3 File Offset: 0x00011BA3
		private BcdInput()
		{
			this.SaveKeyToRegistry = true;
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x0600045E RID: 1118 RVA: 0x000139B2 File Offset: 0x00011BB2
		// (set) Token: 0x0600045F RID: 1119 RVA: 0x000139BA File Offset: 0x00011BBA
		[XmlAttribute]
		public bool SaveKeyToRegistry { get; set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000460 RID: 1120 RVA: 0x000139C3 File Offset: 0x00011BC3
		// (set) Token: 0x06000461 RID: 1121 RVA: 0x000139CB File Offset: 0x00011BCB
		[XmlAttribute]
		public bool IncludeDescriptions { get; set; }

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x000139D4 File Offset: 0x00011BD4
		// (set) Token: 0x06000463 RID: 1123 RVA: 0x000139DC File Offset: 0x00011BDC
		[XmlAttribute]
		public bool IncludeRegistryHeader { get; set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x000139E5 File Offset: 0x00011BE5
		// (set) Token: 0x06000465 RID: 1125 RVA: 0x000139ED File Offset: 0x00011BED
		public BcdObjectsInput Objects { get; set; }

		// Token: 0x06000466 RID: 1126 RVA: 0x000139F6 File Offset: 0x00011BF6
		public void SaveAsRegFile(StreamWriter writer, string path)
		{
			this.Objects.SaveAsRegFile(writer, path);
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x00013A05 File Offset: 0x00011C05
		public void SaveAsRegData(BcdRegData bcdRegData, string path)
		{
			this.Objects.SaveAsRegData(bcdRegData, path);
		}
	}
}
