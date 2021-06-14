using System;

namespace Microsoft.Windows.ImageTools
{
	// Token: 0x02000006 RID: 6
	[Flags]
	internal enum LoggingModeConstant : uint
	{
		// Token: 0x0400001F RID: 31
		PrivateLoggerMode = 2048U,
		// Token: 0x04000020 RID: 32
		PrivateInProc = 131072U
	}
}
