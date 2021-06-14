using System;

namespace Microsoft.Windows.ImageTools
{
	// Token: 0x02000002 RID: 2
	internal enum DeviceStatus
	{
		// Token: 0x04000002 RID: 2
		CONNECTED,
		// Token: 0x04000003 RID: 3
		FLASHING,
		// Token: 0x04000004 RID: 4
		TRANSFER_WIM,
		// Token: 0x04000005 RID: 5
		BOOTING_WIM,
		// Token: 0x04000006 RID: 6
		DONE,
		// Token: 0x04000007 RID: 7
		EXCEPTION,
		// Token: 0x04000008 RID: 8
		ERROR,
		// Token: 0x04000009 RID: 9
		MESSAGE
	}
}
