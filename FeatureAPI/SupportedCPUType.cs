using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000027 RID: 39
	public class SupportedCPUType
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00008280 File Offset: 0x00006480
		[XmlIgnore]
		public List<CpuId> WowGuestCpuIds
		{
			get
			{
				if (this._wowGuestCpuIds == null)
				{
					this._wowGuestCpuIds = new List<CpuId>();
					if (!string.IsNullOrEmpty(this.WowGuestCpuTypesStr))
					{
						foreach (string value in this.WowGuestCpuTypesStr.Split(new char[]
						{
							';'
						}, StringSplitOptions.RemoveEmptyEntries))
						{
							this._wowGuestCpuIds.Add(CpuIdParser.Parse(value));
						}
					}
				}
				return this._wowGuestCpuIds;
			}
		}

		// Token: 0x040000C6 RID: 198
		[XmlAttribute("HostCpuType")]
		public string CpuType;

		// Token: 0x040000C7 RID: 199
		[XmlAttribute("WowGuestCpuTypes")]
		public string WowGuestCpuTypesStr;

		// Token: 0x040000C8 RID: 200
		private List<CpuId> _wowGuestCpuIds;
	}
}
