using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x02000017 RID: 23
	[Flags]
	internal enum DriverStoreReflectFlag : uint
	{
		// Token: 0x0400008F RID: 143
		None = 0U,
		// Token: 0x04000090 RID: 144
		FilesOnly = 1U,
		// Token: 0x04000091 RID: 145
		ActiveDrivers = 2U,
		// Token: 0x04000092 RID: 146
		ExternalOnly = 4U,
		// Token: 0x04000093 RID: 147
		Configurations = 8U
	}
}
