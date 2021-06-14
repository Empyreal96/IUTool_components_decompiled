using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000012 RID: 18
	internal struct WinUsbPipeInformation
	{
		// Token: 0x04000067 RID: 103
		public WinUsbPipeType PipeType;

		// Token: 0x04000068 RID: 104
		public byte PipeId;

		// Token: 0x04000069 RID: 105
		public ushort MaximumPacketSize;

		// Token: 0x0400006A RID: 106
		public byte Interval;
	}
}
