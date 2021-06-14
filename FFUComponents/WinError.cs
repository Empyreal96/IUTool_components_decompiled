using System;

namespace FFUComponents
{
	// Token: 0x02000030 RID: 48
	internal enum WinError : uint
	{
		// Token: 0x0400008A RID: 138
		Success,
		// Token: 0x0400008B RID: 139
		FileNotFound = 2U,
		// Token: 0x0400008C RID: 140
		NoMoreFiles = 18U,
		// Token: 0x0400008D RID: 141
		NotReady = 21U,
		// Token: 0x0400008E RID: 142
		GeneralFailure = 31U,
		// Token: 0x0400008F RID: 143
		InvalidParameter = 87U,
		// Token: 0x04000090 RID: 144
		SemTimeout = 121U,
		// Token: 0x04000091 RID: 145
		InsufficientBuffer,
		// Token: 0x04000092 RID: 146
		WaitTimeout = 258U,
		// Token: 0x04000093 RID: 147
		OperationAborted = 995U,
		// Token: 0x04000094 RID: 148
		IoPending = 997U,
		// Token: 0x04000095 RID: 149
		DeviceNotConnected = 1167U,
		// Token: 0x04000096 RID: 150
		TimeZoneIdInvalid = 4294967295U,
		// Token: 0x04000097 RID: 151
		InvalidHandleValue = 4294967295U,
		// Token: 0x04000098 RID: 152
		PathNotFound = 3U,
		// Token: 0x04000099 RID: 153
		AlreadyExists = 183U,
		// Token: 0x0400009A RID: 154
		NoMoreItems = 259U
	}
}
