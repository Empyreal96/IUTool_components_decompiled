using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000006 RID: 6
	[XmlRoot("MtbfReport")]
	[Serializable]
	public class MtbfReport
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000021CC File Offset: 0x000003CC
		[XmlArray("Devices")]
		[XmlArrayItem("Device", typeof(MtbfDeviceReport))]
		public List<MtbfDeviceReport> DeviceReports
		{
			get
			{
				return this.deviceReports;
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000021D4 File Offset: 0x000003D4
		public static MtbfReport LoadFromXml(string fileName)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(MtbfReport));
			MtbfReport result;
			using (StreamReader streamReader = new StreamReader(fileName))
			{
				result = (MtbfReport)xmlSerializer.Deserialize(streamReader);
			}
			return result;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002224 File Offset: 0x00000424
		public void AddDeviceReport(MtbfDeviceReport deviceReport)
		{
			this.deviceReports.Add(deviceReport);
		}

		// Token: 0x04000006 RID: 6
		[XmlIgnore]
		private List<MtbfDeviceReport> deviceReports = new List<MtbfDeviceReport>();
	}
}
