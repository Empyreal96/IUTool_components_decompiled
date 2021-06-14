using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000006 RID: 6
	[Flags]
	internal enum UsbRequest
	{
		// Token: 0x04000011 RID: 17
		DeviceToHost = 128,
		// Token: 0x04000012 RID: 18
		HostToDevice = 0,
		// Token: 0x04000013 RID: 19
		Standard = 0,
		// Token: 0x04000014 RID: 20
		Class = 32,
		// Token: 0x04000015 RID: 21
		Vendor = 64,
		// Token: 0x04000016 RID: 22
		Reserved = 96,
		// Token: 0x04000017 RID: 23
		ForDevice = 0,
		// Token: 0x04000018 RID: 24
		ForInterface = 1,
		// Token: 0x04000019 RID: 25
		ForEndpoint = 2,
		// Token: 0x0400001A RID: 26
		ForOther = 3
	}
}
