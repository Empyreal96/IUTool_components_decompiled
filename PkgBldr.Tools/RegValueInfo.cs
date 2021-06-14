using System;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200001B RID: 27
	public sealed class RegValueInfo
	{
		// Token: 0x06000116 RID: 278 RVA: 0x000050D0 File Offset: 0x000032D0
		public RegValueInfo()
		{
		}

		// Token: 0x06000117 RID: 279 RVA: 0x000057B0 File Offset: 0x000039B0
		public RegValueInfo(RegValueInfo regValueInfo)
		{
			if (regValueInfo == null)
			{
				throw new ArgumentNullException("regValueInfo");
			}
			this.Type = regValueInfo.Type;
			this.KeyName = regValueInfo.KeyName;
			this.ValueName = regValueInfo.ValueName;
			this.Value = regValueInfo.Value;
			this.Partition = regValueInfo.Partition;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000580D File Offset: 0x00003A0D
		public void SetRegValueType(string strType)
		{
			this.Type = RegUtil.RegValueTypeForString(strType);
		}

		// Token: 0x0400004F RID: 79
		public RegValueType Type;

		// Token: 0x04000050 RID: 80
		public string KeyName;

		// Token: 0x04000051 RID: 81
		public string ValueName;

		// Token: 0x04000052 RID: 82
		public string Value;

		// Token: 0x04000053 RID: 83
		public string Partition;
	}
}
