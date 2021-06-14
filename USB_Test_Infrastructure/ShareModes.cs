using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x0200000A RID: 10
	[Flags]
	internal enum ShareModes : uint
	{
		// Token: 0x04000030 RID: 48
		Read = 1U,
		// Token: 0x04000031 RID: 49
		Write = 2U,
		// Token: 0x04000032 RID: 50
		Delete = 4U
	}
}
