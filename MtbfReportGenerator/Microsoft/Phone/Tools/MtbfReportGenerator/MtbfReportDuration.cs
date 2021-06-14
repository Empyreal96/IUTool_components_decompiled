using System;
using System.Xml.Serialization;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x0200000E RID: 14
	[Serializable]
	public class MtbfReportDuration
	{
		// Token: 0x06000059 RID: 89 RVA: 0x000026F4 File Offset: 0x000008F4
		public MtbfReportDuration()
		{
			this.DurationMilliSeconds = 0;
			this.StartTickCount = 0;
			this.EndTickCount = 0;
			this.StartTime = DateTime.MinValue;
			this.EndTime = DateTime.MinValue;
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00002727 File Offset: 0x00000927
		// (set) Token: 0x0600005B RID: 91 RVA: 0x0000272F File Offset: 0x0000092F
		[XmlAttribute("DurationMilliSeconds")]
		public int DurationMilliSeconds { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00002738 File Offset: 0x00000938
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00002740 File Offset: 0x00000940
		[XmlAttribute("StartTime")]
		public DateTime StartTime { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00002749 File Offset: 0x00000949
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00002751 File Offset: 0x00000951
		[XmlAttribute("EndTime")]
		public DateTime EndTime { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000060 RID: 96 RVA: 0x0000275A File Offset: 0x0000095A
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00002762 File Offset: 0x00000962
		[XmlAttribute("StartTickCount")]
		public int StartTickCount { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000062 RID: 98 RVA: 0x0000276B File Offset: 0x0000096B
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00002773 File Offset: 0x00000973
		[XmlAttribute("EndTickCount")]
		public int EndTickCount { get; set; }
	}
}
