using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x02000014 RID: 20
	[Flags]
	internal enum DriverStoreOfflineAddDriverPackageFlags : uint
	{
		// Token: 0x0400007A RID: 122
		None = 0U,
		// Token: 0x0400007B RID: 123
		SkipInstall = 1U,
		// Token: 0x0400007C RID: 124
		Inbox = 2U,
		// Token: 0x0400007D RID: 125
		F6 = 4U,
		// Token: 0x0400007E RID: 126
		SkipExternalFilePresenceCheck = 8U,
		// Token: 0x0400007F RID: 127
		NoTempCopy = 16U,
		// Token: 0x04000080 RID: 128
		UseHardLinks = 32U,
		// Token: 0x04000081 RID: 129
		InstallOnly = 64U,
		// Token: 0x04000082 RID: 130
		ReplacePackage = 128U,
		// Token: 0x04000083 RID: 131
		Force = 256U,
		// Token: 0x04000084 RID: 132
		BaseVersion = 512U
	}
}
