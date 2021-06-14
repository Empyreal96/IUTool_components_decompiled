using System;

namespace FFUComponents
{
	// Token: 0x0200002D RID: 45
	[Flags]
	internal enum AccessRights : uint
	{
		// Token: 0x0400007D RID: 125
		Read = 2147483648U,
		// Token: 0x0400007E RID: 126
		Write = 1073741824U,
		// Token: 0x0400007F RID: 127
		Execute = 536870912U,
		// Token: 0x04000080 RID: 128
		All = 268435456U
	}
}
