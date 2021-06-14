using System;

namespace FFUComponents
{
	// Token: 0x02000035 RID: 53
	[Flags]
	internal enum DICSFlags : uint
	{
		// Token: 0x040000AC RID: 172
		Global = 1U,
		// Token: 0x040000AD RID: 173
		ConfigSpecific = 2U,
		// Token: 0x040000AE RID: 174
		ConfigGeneral = 4U
	}
}
