using System;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x0200003D RID: 61
	[StructLayout(LayoutKind.Sequential)]
	internal class USB_NODE_CONNECTION_INFORMATION_EX_V2
	{
		// Token: 0x0600010E RID: 270 RVA: 0x00004B97 File Offset: 0x00002D97
		public USB_NODE_CONNECTION_INFORMATION_EX_V2(uint portNumber)
		{
			this.ConnectionIndex = portNumber;
			this.Length = (uint)Marshal.SizeOf<USB_NODE_CONNECTION_INFORMATION_EX_V2>(this);
			this.SupportedUsbProtocols = 7U;
			this.Flags = 0U;
		}

		// Token: 0x040000CD RID: 205
		public uint ConnectionIndex;

		// Token: 0x040000CE RID: 206
		public uint Length;

		// Token: 0x040000CF RID: 207
		public uint SupportedUsbProtocols;

		// Token: 0x040000D0 RID: 208
		public uint Flags;
	}
}
