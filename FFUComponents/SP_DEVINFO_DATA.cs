using System;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x02000039 RID: 57
	[StructLayout(LayoutKind.Sequential)]
	public class SP_DEVINFO_DATA
	{
		// Token: 0x0600010A RID: 266 RVA: 0x00004B3A File Offset: 0x00002D3A
		public SP_DEVINFO_DATA()
		{
			this.cbSize = (uint)Marshal.SizeOf<SP_DEVINFO_DATA>(this);
		}

		// Token: 0x040000C1 RID: 193
		public uint cbSize;

		// Token: 0x040000C2 RID: 194
		public Guid ClassGuid;

		// Token: 0x040000C3 RID: 195
		public uint DevInst;

		// Token: 0x040000C4 RID: 196
		public IntPtr Reserved;
	}
}
