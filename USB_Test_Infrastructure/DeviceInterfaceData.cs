using System;
using System.Runtime.InteropServices;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000014 RID: 20
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct DeviceInterfaceData
	{
		// Token: 0x04000070 RID: 112
		public int Size;

		// Token: 0x04000071 RID: 113
		public Guid InterfaceClassGuid;

		// Token: 0x04000072 RID: 114
		public int Flags;

		// Token: 0x04000073 RID: 115
		public IntPtr Reserved;
	}
}
