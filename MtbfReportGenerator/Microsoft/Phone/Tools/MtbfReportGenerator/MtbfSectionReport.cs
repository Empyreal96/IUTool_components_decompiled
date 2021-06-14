using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x0200000A RID: 10
	[Serializable]
	public class MtbfSectionReport : MtbfReportDuration
	{
		// Token: 0x06000030 RID: 48 RVA: 0x0000245A File Offset: 0x0000065A
		public MtbfSectionReport()
		{
			this.Number = string.Empty;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002478 File Offset: 0x00000678
		public MtbfSectionReport(string sectionNumber)
		{
			this.Number = sectionNumber;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002492 File Offset: 0x00000692
		// (set) Token: 0x06000033 RID: 51 RVA: 0x0000249A File Offset: 0x0000069A
		[XmlAttribute("Number")]
		public string Number { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000024A3 File Offset: 0x000006A3
		[XmlArray("Tests")]
		[XmlArrayItem("Test", typeof(MtbfTestReport))]
		public List<MtbfTestReport> TestReports
		{
			get
			{
				return this.testReports;
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000024AB File Offset: 0x000006AB
		public void AddTest(MtbfTestReport testReport)
		{
			this.testReports.Add(testReport);
		}

		// Token: 0x0400000E RID: 14
		[XmlIgnore]
		private List<MtbfTestReport> testReports = new List<MtbfTestReport>();
	}
}
