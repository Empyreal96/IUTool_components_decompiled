using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000009 RID: 9
	[Serializable]
	public class MtbfLoopReport : MtbfReportDuration
	{
		// Token: 0x0600002A RID: 42 RVA: 0x000023BE File Offset: 0x000005BE
		public MtbfLoopReport()
		{
			this.Number = 0;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000023D8 File Offset: 0x000005D8
		public MtbfLoopReport(int loopNumber)
		{
			this.Number = loopNumber;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002C RID: 44 RVA: 0x000023F2 File Offset: 0x000005F2
		// (set) Token: 0x0600002D RID: 45 RVA: 0x000023FA File Offset: 0x000005FA
		[XmlAttribute("Number")]
		public int Number { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002403 File Offset: 0x00000603
		[XmlArray("Sections")]
		[XmlArrayItem("Section", typeof(MtbfSectionReport))]
		public List<MtbfSectionReport> SectionReports
		{
			get
			{
				return this.sectionReports;
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000240C File Offset: 0x0000060C
		public MtbfSectionReport AddSection(string sectionNumber)
		{
			MtbfSectionReport mtbfSectionReport = this.sectionReports.Find((MtbfSectionReport item) => item.Number.Equals(sectionNumber, StringComparison.OrdinalIgnoreCase));
			if (mtbfSectionReport == null)
			{
				mtbfSectionReport = new MtbfSectionReport(sectionNumber);
				this.sectionReports.Add(mtbfSectionReport);
			}
			return mtbfSectionReport;
		}

		// Token: 0x0400000C RID: 12
		[XmlIgnore]
		private List<MtbfSectionReport> sectionReports = new List<MtbfSectionReport>();
	}
}
