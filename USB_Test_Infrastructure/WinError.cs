using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x0200000D RID: 13
	internal enum WinError : uint
	{
		// Token: 0x0400003C RID: 60
		Success,
		// Token: 0x0400003D RID: 61
		FileNotFound = 2U,
		// Token: 0x0400003E RID: 62
		NoMoreFiles = 18U,
		// Token: 0x0400003F RID: 63
		NotReady = 21U,
		// Token: 0x04000040 RID: 64
		GeneralFailure = 31U,
		// Token: 0x04000041 RID: 65
		InvalidParameter = 87U,
		// Token: 0x04000042 RID: 66
		InsufficientBuffer = 122U,
		// Token: 0x04000043 RID: 67
		IoPending = 997U,
		// Token: 0x04000044 RID: 68
		DeviceNotConnected = 1167U,
		// Token: 0x04000045 RID: 69
		TimeZoneIdInvalid = 4294967295U,
		// Token: 0x04000046 RID: 70
		InvalidHandleValue = 4294967295U,
		// Token: 0x04000047 RID: 71
		PathNotFound = 3U,
		// Token: 0x04000048 RID: 72
		AlreadyExists = 183U
	}
}
