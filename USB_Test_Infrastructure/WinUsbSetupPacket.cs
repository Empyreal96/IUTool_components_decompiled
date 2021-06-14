using System;
using System.Runtime.InteropServices;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000013 RID: 19
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct WinUsbSetupPacket
	{
		// Token: 0x0400006B RID: 107
		public byte RequestType;

		// Token: 0x0400006C RID: 108
		public byte Request;

		// Token: 0x0400006D RID: 109
		public ushort Value;

		// Token: 0x0400006E RID: 110
		public ushort Index;

		// Token: 0x0400006F RID: 111
		public ushort Length;
	}
}
