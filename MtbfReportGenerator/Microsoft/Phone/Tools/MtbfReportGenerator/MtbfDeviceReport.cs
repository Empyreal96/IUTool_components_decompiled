using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000007 RID: 7
	[Serializable]
	public class MtbfDeviceReport
	{
		// Token: 0x0600001B RID: 27 RVA: 0x00002245 File Offset: 0x00000445
		public MtbfDeviceReport()
		{
			this.DeviceNumber = 0;
			this.Mix = new MtbfMix();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000226A File Offset: 0x0000046A
		public MtbfDeviceReport(int deviceNumber)
		{
			this.DeviceNumber = deviceNumber;
			this.Mix = new MtbfMix();
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001D RID: 29 RVA: 0x0000228F File Offset: 0x0000048F
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002297 File Offset: 0x00000497
		[XmlIgnore]
		public MtbfMix Mix { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001F RID: 31 RVA: 0x000022A0 File Offset: 0x000004A0
		// (set) Token: 0x06000020 RID: 32 RVA: 0x000022A8 File Offset: 0x000004A8
		[XmlAttribute("Number")]
		public int DeviceNumber { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000022B1 File Offset: 0x000004B1
		[XmlArray("Groups")]
		[XmlArrayItem("Group", typeof(MtbfGroupReport))]
		public List<MtbfGroupReport> GroupReports
		{
			get
			{
				return this.groupReports;
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000022BC File Offset: 0x000004BC
		public MtbfGroupReport GetGroupReport(string groupName)
		{
			return this.groupReports.Find((MtbfGroupReport item) => item.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000022F0 File Offset: 0x000004F0
		public MtbfGroupReport AddGroup(string groupName)
		{
			MtbfGroupReport mtbfGroupReport = this.GetGroupReport(groupName);
			if (mtbfGroupReport == null)
			{
				mtbfGroupReport = new MtbfGroupReport(groupName);
				this.groupReports.Add(mtbfGroupReport);
			}
			return mtbfGroupReport;
		}

		// Token: 0x04000007 RID: 7
		[XmlIgnore]
		private List<MtbfGroupReport> groupReports = new List<MtbfGroupReport>();
	}
}
