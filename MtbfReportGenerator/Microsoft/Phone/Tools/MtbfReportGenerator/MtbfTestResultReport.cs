using System;
using System.Xml.Serialization;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x0200000C RID: 12
	[Serializable]
	public class MtbfTestResultReport
	{
		// Token: 0x06000045 RID: 69 RVA: 0x000025D0 File Offset: 0x000007D0
		public MtbfTestResultReport()
		{
			this.Result = string.Empty;
			this.Expected = 0;
			this.Attempted = 0;
			this.Passed = 0;
			this.Failed = 0;
			this.Skipped = 0;
			this.Aborted = 0;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x0000260D File Offset: 0x0000080D
		public MtbfTestResultReport(string result, int expected, int attempted, int passed, int failed, int skipped, int aborted)
		{
			this.Result = result;
			this.Expected = expected;
			this.Attempted = attempted;
			this.Passed = passed;
			this.Failed = failed;
			this.Skipped = skipped;
			this.Aborted = aborted;
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000047 RID: 71 RVA: 0x0000264A File Offset: 0x0000084A
		// (set) Token: 0x06000048 RID: 72 RVA: 0x00002652 File Offset: 0x00000852
		[XmlText]
		public string Result { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000049 RID: 73 RVA: 0x0000265B File Offset: 0x0000085B
		// (set) Token: 0x0600004A RID: 74 RVA: 0x00002663 File Offset: 0x00000863
		[XmlAttribute("ExpectedCount")]
		public int Expected { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600004B RID: 75 RVA: 0x0000266C File Offset: 0x0000086C
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00002674 File Offset: 0x00000874
		[XmlAttribute("AttemptedCount")]
		public int Attempted { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600004D RID: 77 RVA: 0x0000267D File Offset: 0x0000087D
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00002685 File Offset: 0x00000885
		[XmlAttribute("PassedCount")]
		public int Passed { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600004F RID: 79 RVA: 0x0000268E File Offset: 0x0000088E
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00002696 File Offset: 0x00000896
		[XmlAttribute("FailedCount")]
		public int Failed { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000051 RID: 81 RVA: 0x0000269F File Offset: 0x0000089F
		// (set) Token: 0x06000052 RID: 82 RVA: 0x000026A7 File Offset: 0x000008A7
		[XmlAttribute("SkippedCount")]
		public int Skipped { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000026B0 File Offset: 0x000008B0
		// (set) Token: 0x06000054 RID: 84 RVA: 0x000026B8 File Offset: 0x000008B8
		[XmlAttribute("AbortedCount")]
		public int Aborted { get; set; }
	}
}
