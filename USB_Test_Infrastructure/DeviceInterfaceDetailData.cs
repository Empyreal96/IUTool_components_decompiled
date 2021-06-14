using System;
using System.Runtime.InteropServices;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000016 RID: 22
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
	internal struct DeviceInterfaceDetailData
	{
		// Token: 0x04000078 RID: 120
		public int Size;

		// Token: 0x04000079 RID: 121
		public char DevicePath;
	}
}
