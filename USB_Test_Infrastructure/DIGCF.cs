using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x0200000E RID: 14
	[Flags]
	internal enum DIGCF
	{
		// Token: 0x0400004A RID: 74
		Default = 1,
		// Token: 0x0400004B RID: 75
		Present = 2,
		// Token: 0x0400004C RID: 76
		AllClasses = 4,
		// Token: 0x0400004D RID: 77
		Profile = 8,
		// Token: 0x0400004E RID: 78
		DeviceInterface = 16
	}
}
