using System;

namespace FFUComponents
{
	// Token: 0x0200002C RID: 44
	internal enum WinUsbPolicyType : uint
	{
		// Token: 0x04000075 RID: 117
		ShortPacketTerminate = 1U,
		// Token: 0x04000076 RID: 118
		AutoClearStall,
		// Token: 0x04000077 RID: 119
		PipeTransferTimeout,
		// Token: 0x04000078 RID: 120
		IgnoreShortPackets,
		// Token: 0x04000079 RID: 121
		AllowPartialReads,
		// Token: 0x0400007A RID: 122
		AutoFlush,
		// Token: 0x0400007B RID: 123
		RawIO
	}
}
