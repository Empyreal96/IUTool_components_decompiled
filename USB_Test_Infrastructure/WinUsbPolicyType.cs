using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000008 RID: 8
	internal enum WinUsbPolicyType : uint
	{
		// Token: 0x04000021 RID: 33
		ShortPacketTerminate = 1U,
		// Token: 0x04000022 RID: 34
		AutoClearStall,
		// Token: 0x04000023 RID: 35
		PipeTransferTimeout,
		// Token: 0x04000024 RID: 36
		IgnoreShortPackets,
		// Token: 0x04000025 RID: 37
		AllowPartialReads,
		// Token: 0x04000026 RID: 38
		AutoFlush,
		// Token: 0x04000027 RID: 39
		RawIO,
		// Token: 0x04000028 RID: 40
		MaximumTransferSize,
		// Token: 0x04000029 RID: 41
		ResetPipeOnResume
	}
}
