using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x0200001A RID: 26
	[Flags]
	internal enum DriverPackageEnumFilesFlag : uint
	{
		// Token: 0x0400009B RID: 155
		Copy = 1U,
		// Token: 0x0400009C RID: 156
		Delete = 2U,
		// Token: 0x0400009D RID: 157
		Rename = 4U,
		// Token: 0x0400009E RID: 158
		Inf = 16U,
		// Token: 0x0400009F RID: 159
		Catalog = 32U,
		// Token: 0x040000A0 RID: 160
		Binaries = 64U,
		// Token: 0x040000A1 RID: 161
		CopyInfs = 128U,
		// Token: 0x040000A2 RID: 162
		IncludeInfs = 256U,
		// Token: 0x040000A3 RID: 163
		External = 4096U,
		// Token: 0x040000A4 RID: 164
		UniqueSource = 8192U,
		// Token: 0x040000A5 RID: 165
		UniqueDestination = 16384U
	}
}
