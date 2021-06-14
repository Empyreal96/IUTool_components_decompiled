using System;
using System.Xml.Serialization;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x0200000D RID: 13
	[Serializable]
	public class MtbfTestLogReport
	{
		// Token: 0x06000055 RID: 85 RVA: 0x000026C1 File Offset: 0x000008C1
		public MtbfTestLogReport()
		{
			this.Name = string.Empty;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000026D4 File Offset: 0x000008D4
		public MtbfTestLogReport(string fileName)
		{
			this.Name = fileName;
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000057 RID: 87 RVA: 0x000026E3 File Offset: 0x000008E3
		// (set) Token: 0x06000058 RID: 88 RVA: 0x000026EB File Offset: 0x000008EB
		[XmlAttribute("FileName")]
		public string Name { get; set; }
	}
}
