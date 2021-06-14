using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000008 RID: 8
	[Serializable]
	public class MtbfGroupReport : MtbfReportDuration
	{
		// Token: 0x06000024 RID: 36 RVA: 0x0000231C File Offset: 0x0000051C
		public MtbfGroupReport()
		{
			this.Name = string.Empty;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000233A File Offset: 0x0000053A
		public MtbfGroupReport(string name)
		{
			this.Name = name;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002354 File Offset: 0x00000554
		// (set) Token: 0x06000027 RID: 39 RVA: 0x0000235C File Offset: 0x0000055C
		[XmlAttribute("Name")]
		public string Name { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002365 File Offset: 0x00000565
		[XmlArray("Loops")]
		[XmlArrayItem("Loop", typeof(MtbfLoopReport))]
		public List<MtbfLoopReport> LoopReports
		{
			get
			{
				return this.loopReports;
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002370 File Offset: 0x00000570
		public MtbfLoopReport AddLoop(int loopNumber)
		{
			MtbfLoopReport mtbfLoopReport = this.loopReports.Find((MtbfLoopReport item) => item.Number == loopNumber);
			if (mtbfLoopReport == null)
			{
				mtbfLoopReport = new MtbfLoopReport(loopNumber);
				this.loopReports.Add(mtbfLoopReport);
			}
			return mtbfLoopReport;
		}

		// Token: 0x0400000A RID: 10
		[XmlIgnore]
		private List<MtbfLoopReport> loopReports = new List<MtbfLoopReport>();
	}
}
