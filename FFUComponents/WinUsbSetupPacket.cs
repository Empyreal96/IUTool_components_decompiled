using System;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x02000038 RID: 56
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct WinUsbSetupPacket
	{
		// Token: 0x040000BC RID: 188
		public byte RequestType;

		// Token: 0x040000BD RID: 189
		public byte Request;

		// Token: 0x040000BE RID: 190
		public ushort Value;

		// Token: 0x040000BF RID: 191
		public ushort Index;

		// Token: 0x040000C0 RID: 192
		public ushort Length;
	}
}
