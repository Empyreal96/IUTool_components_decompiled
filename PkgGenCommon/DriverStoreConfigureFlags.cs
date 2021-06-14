using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x02000016 RID: 22
	[Flags]
	internal enum DriverStoreConfigureFlags : uint
	{
		// Token: 0x04000088 RID: 136
		None = 0U,
		// Token: 0x04000089 RID: 137
		Force = 1U,
		// Token: 0x0400008A RID: 138
		ActiveOnly = 2U,
		// Token: 0x0400008B RID: 139
		SourceConfigurations = 65536U,
		// Token: 0x0400008C RID: 140
		SourceDeviceIds = 131072U,
		// Token: 0x0400008D RID: 141
		TargetDeviceNodes = 1048576U
	}
}
