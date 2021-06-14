using System;
using System.Runtime.InteropServices;

namespace USB_Test_Infrastructure
{
	// Token: 0x0200000F RID: 15
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct TimeZoneInformation
	{
		// Token: 0x0400004F RID: 79
		public int Bias;

		// Token: 0x04000050 RID: 80
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string StandardName;

		// Token: 0x04000051 RID: 81
		public SystemTime StandardDate;

		// Token: 0x04000052 RID: 82
		public int StandardBias;

		// Token: 0x04000053 RID: 83
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string DaylightName;

		// Token: 0x04000054 RID: 84
		public SystemTime DaylightDate;

		// Token: 0x04000055 RID: 85
		public int DaylightBias;
	}
}
