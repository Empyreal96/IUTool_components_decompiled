using System;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x0200003B RID: 59
	[StructLayout(LayoutKind.Sequential)]
	public class SP_DEVICE_INTERFACE_DATA
	{
		// Token: 0x0600010C RID: 268 RVA: 0x00004B64 File Offset: 0x00002D64
		public SP_DEVICE_INTERFACE_DATA()
		{
			this.cbSize = (uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>(this);
		}

		// Token: 0x040000C7 RID: 199
		public uint cbSize;

		// Token: 0x040000C8 RID: 200
		public Guid InterfaceClassGuid;

		// Token: 0x040000C9 RID: 201
		public uint Flags;

		// Token: 0x040000CA RID: 202
		private IntPtr Reserved;
	}
}
