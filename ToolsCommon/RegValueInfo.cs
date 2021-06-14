using System;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200004D RID: 77
	public sealed class RegValueInfo
	{
		// Token: 0x06000220 RID: 544 RVA: 0x00004FC0 File Offset: 0x000031C0
		public RegValueInfo()
		{
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000A3FC File Offset: 0x000085FC
		public RegValueInfo(RegValueInfo regValueInfo)
		{
			this.Type = regValueInfo.Type;
			this.KeyName = regValueInfo.KeyName;
			this.ValueName = regValueInfo.ValueName;
			this.Value = regValueInfo.Value;
			this.Partition = regValueInfo.Partition;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000A44B File Offset: 0x0000864B
		public void SetRegValueType(string strType)
		{
			this.Type = RegUtil.RegValueTypeForString(strType);
		}

		// Token: 0x04000107 RID: 263
		public RegValueType Type;

		// Token: 0x04000108 RID: 264
		public string KeyName;

		// Token: 0x04000109 RID: 265
		public string ValueName;

		// Token: 0x0400010A RID: 266
		public string Value;

		// Token: 0x0400010B RID: 267
		public string Partition;
	}
}
