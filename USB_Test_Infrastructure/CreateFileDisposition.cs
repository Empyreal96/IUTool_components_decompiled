using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x0200000B RID: 11
	internal enum CreateFileDisposition : uint
	{
		// Token: 0x04000034 RID: 52
		CreateNew = 1U,
		// Token: 0x04000035 RID: 53
		CreateAlways,
		// Token: 0x04000036 RID: 54
		CreateExisting,
		// Token: 0x04000037 RID: 55
		OpenAlways,
		// Token: 0x04000038 RID: 56
		TruncateExisting
	}
}
