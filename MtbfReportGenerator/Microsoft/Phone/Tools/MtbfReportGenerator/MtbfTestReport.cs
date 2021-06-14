using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x0200000B RID: 11
	[Serializable]
	public class MtbfTestReport : MtbfReportDuration
	{
		// Token: 0x06000036 RID: 54 RVA: 0x000024BC File Offset: 0x000006BC
		public MtbfTestReport()
		{
			this.Name = string.Empty;
			this.Command = string.Empty;
			this.Parameter = string.Empty;
			this.Result = new MtbfTestResultReport();
			this.Index = 0;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000250D File Offset: 0x0000070D
		public MtbfTestReport(string testName, int index, string command, string parameter)
		{
			this.Name = testName;
			this.Index = index;
			this.Command = command;
			this.Parameter = parameter;
			this.Result = new MtbfTestResultReport();
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002548 File Offset: 0x00000748
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002550 File Offset: 0x00000750
		[XmlAttribute("Name")]
		public string Name { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002559 File Offset: 0x00000759
		// (set) Token: 0x0600003B RID: 59 RVA: 0x00002561 File Offset: 0x00000761
		[XmlAttribute("Index")]
		public int Index { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003C RID: 60 RVA: 0x0000256A File Offset: 0x0000076A
		// (set) Token: 0x0600003D RID: 61 RVA: 0x00002572 File Offset: 0x00000772
		[XmlAttribute("Command")]
		public string Command { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003E RID: 62 RVA: 0x0000257B File Offset: 0x0000077B
		// (set) Token: 0x0600003F RID: 63 RVA: 0x00002583 File Offset: 0x00000783
		[XmlAttribute("Parameter")]
		public string Parameter { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000040 RID: 64 RVA: 0x0000258C File Offset: 0x0000078C
		// (set) Token: 0x06000041 RID: 65 RVA: 0x00002594 File Offset: 0x00000794
		[XmlElement("Result")]
		public MtbfTestResultReport Result { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000042 RID: 66 RVA: 0x0000259D File Offset: 0x0000079D
		[XmlArray("Logs")]
		[XmlArrayItem("Log", typeof(MtbfTestLogReport))]
		public List<MtbfTestLogReport> LogReports
		{
			get
			{
				return this.logReports;
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000025A5 File Offset: 0x000007A5
		public void SetResult(string result, int expected, int attempted, int passed, int failed, int skipped, int aborted)
		{
			this.Result = new MtbfTestResultReport(result, expected, attempted, passed, failed, skipped, aborted);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000025BD File Offset: 0x000007BD
		public void AddLogFile(string fileName)
		{
			this.logReports.Add(new MtbfTestLogReport(fileName));
		}

		// Token: 0x04000010 RID: 16
		[XmlIgnore]
		private List<MtbfTestLogReport> logReports = new List<MtbfTestLogReport>();
	}
}
