using System;

namespace FFUComponents
{
	// Token: 0x02000037 RID: 55
	internal struct WinUsbPipeInformation
	{
		// Token: 0x040000B8 RID: 184
		public WinUsbPipeType PipeType;

		// Token: 0x040000B9 RID: 185
		public byte PipeId;

		// Token: 0x040000BA RID: 186
		public ushort MaximumPacketSize;

		// Token: 0x040000BB RID: 187
		public byte Interval;
	}
}
