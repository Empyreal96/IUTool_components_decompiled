using System;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x0200003C RID: 60
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal class SP_DEVICE_INTERFACE_DETAIL_DATA
	{
		// Token: 0x0600010D RID: 269 RVA: 0x00004B78 File Offset: 0x00002D78
		public SP_DEVICE_INTERFACE_DETAIL_DATA()
		{
			if (IntPtr.Size == 4)
			{
				this.cbSize = 6U;
				return;
			}
			this.cbSize = 8U;
		}

		// Token: 0x040000CB RID: 203
		public uint cbSize;

		// Token: 0x040000CC RID: 204
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string DevicePath;
	}
}
