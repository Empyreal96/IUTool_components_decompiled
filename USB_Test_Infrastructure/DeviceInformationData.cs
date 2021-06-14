using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000015 RID: 21
	internal struct DeviceInformationData
	{
		// Token: 0x04000074 RID: 116
		public int Size;

		// Token: 0x04000075 RID: 117
		public Guid ClassGuid;

		// Token: 0x04000076 RID: 118
		public int DevInst;

		// Token: 0x04000077 RID: 119
		public IntPtr Reserved;
	}
}
