using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x02000013 RID: 19
	[Flags]
	internal enum DriverStoreImportFlag : uint
	{
		// Token: 0x0400006C RID: 108
		None = 0U,
		// Token: 0x0400006D RID: 109
		SkipTempCopy = 1U,
		// Token: 0x0400006E RID: 110
		SkipExternalFileCheck = 2U,
		// Token: 0x0400006F RID: 111
		NoRestorePoint = 4U,
		// Token: 0x04000070 RID: 112
		NonInteractive = 8U,
		// Token: 0x04000071 RID: 113
		Replace = 32U,
		// Token: 0x04000072 RID: 114
		Hardlink = 64U,
		// Token: 0x04000073 RID: 115
		PublishSameName = 256U,
		// Token: 0x04000074 RID: 116
		Inbox = 512U,
		// Token: 0x04000075 RID: 117
		F6 = 1024U,
		// Token: 0x04000076 RID: 118
		BaseVersion = 2048U,
		// Token: 0x04000077 RID: 119
		SystemDefaultLocale = 4096U,
		// Token: 0x04000078 RID: 120
		SystemCritical = 8192U
	}
}
