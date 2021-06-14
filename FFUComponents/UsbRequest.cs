using System;

namespace FFUComponents
{
	// Token: 0x0200002A RID: 42
	[Flags]
	internal enum UsbRequest
	{
		// Token: 0x04000065 RID: 101
		DeviceToHost = 128,
		// Token: 0x04000066 RID: 102
		HostToDevice = 0,
		// Token: 0x04000067 RID: 103
		Standard = 0,
		// Token: 0x04000068 RID: 104
		Class = 32,
		// Token: 0x04000069 RID: 105
		Vendor = 64,
		// Token: 0x0400006A RID: 106
		Reserved = 96,
		// Token: 0x0400006B RID: 107
		ForDevice = 0,
		// Token: 0x0400006C RID: 108
		ForInterface = 1,
		// Token: 0x0400006D RID: 109
		ForEndpoint = 2,
		// Token: 0x0400006E RID: 110
		ForOther = 3
	}
}
