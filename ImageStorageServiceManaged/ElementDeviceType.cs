using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200000A RID: 10
	[CLSCompliant(false)]
	public enum ElementDeviceType : uint
	{
		// Token: 0x040000B0 RID: 176
		BootDevice = 1U,
		// Token: 0x040000B1 RID: 177
		Partition,
		// Token: 0x040000B2 RID: 178
		File,
		// Token: 0x040000B3 RID: 179
		RamDisk,
		// Token: 0x040000B4 RID: 180
		Unknown,
		// Token: 0x040000B5 RID: 181
		QualifiedPartition,
		// Token: 0x040000B6 RID: 182
		LocateDevice = 8U
	}
}
