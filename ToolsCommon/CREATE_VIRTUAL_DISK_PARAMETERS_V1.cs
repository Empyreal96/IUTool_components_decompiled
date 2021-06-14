using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000036 RID: 54
	[CLSCompliant(false)]
	public struct CREATE_VIRTUAL_DISK_PARAMETERS_V1
	{
		// Token: 0x040000B1 RID: 177
		public Guid UniqueId;

		// Token: 0x040000B2 RID: 178
		public ulong MaximumSize;

		// Token: 0x040000B3 RID: 179
		public uint BlockSizeInBytes;

		// Token: 0x040000B4 RID: 180
		public uint SectorSizeInBytes;

		// Token: 0x040000B5 RID: 181
		[MarshalAs(UnmanagedType.LPWStr)]
		public string ParentPath;

		// Token: 0x040000B6 RID: 182
		[MarshalAs(UnmanagedType.LPWStr)]
		public string SourcePath;
	}
}
