using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000009 RID: 9
	[Flags]
	internal enum AccessRights : uint
	{
		// Token: 0x0400002B RID: 43
		Read = 2147483648U,
		// Token: 0x0400002C RID: 44
		Write = 1073741824U,
		// Token: 0x0400002D RID: 45
		Execute = 536870912U,
		// Token: 0x0400002E RID: 46
		All = 268435456U
	}
}
