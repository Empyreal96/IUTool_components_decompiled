using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x02000015 RID: 21
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class Setting : IDefinedIn
	{
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600014E RID: 334 RVA: 0x00007E98 File Offset: 0x00006098
		// (set) Token: 0x0600014F RID: 335 RVA: 0x00007EA0 File Offset: 0x000060A0
		[XmlIgnore]
		public string DefinedInFile { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000150 RID: 336 RVA: 0x00007EA9 File Offset: 0x000060A9
		// (set) Token: 0x06000151 RID: 337 RVA: 0x00007EB1 File Offset: 0x000060B1
		[XmlAttribute]
		public string Name { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00007EBA File Offset: 0x000060BA
		// (set) Token: 0x06000153 RID: 339 RVA: 0x00007EC2 File Offset: 0x000060C2
		[XmlAttribute]
		public string Value { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00007ECB File Offset: 0x000060CB
		// (set) Token: 0x06000155 RID: 341 RVA: 0x00007ED3 File Offset: 0x000060D3
		[XmlAttribute]
		public string Type { get; set; }
	}
}
