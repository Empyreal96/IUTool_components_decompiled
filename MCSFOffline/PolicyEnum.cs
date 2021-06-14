using System;
using System.Xml.Linq;

namespace Microsoft.WindowsPhone.MCSF.Offline
{
	// Token: 0x02000006 RID: 6
	public class PolicyEnum
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002833 File Offset: 0x00000A33
		// (set) Token: 0x06000028 RID: 40 RVA: 0x0000283B File Offset: 0x00000A3B
		public string Value { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002844 File Offset: 0x00000A44
		// (set) Token: 0x0600002A RID: 42 RVA: 0x0000284C File Offset: 0x00000A4C
		public string FriendlyName { get; set; }

		// Token: 0x0600002B RID: 43 RVA: 0x00002858 File Offset: 0x00000A58
		public PolicyEnum(XElement option, bool parseInteger)
		{
			if (parseInteger)
			{
				this.Value = Extensions.ParseInt((string)option.LocalAttribute("Value")).ToString();
			}
			else
			{
				this.Value = (string)option.LocalAttribute("Value");
			}
			this.FriendlyName = (((string)option.LocalAttribute("FriendlyName")) ?? this.Value);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000028C9 File Offset: 0x00000AC9
		public PolicyEnum(string friendlyName, string value)
		{
			this.FriendlyName = friendlyName;
			this.Value = value;
		}
	}
}
