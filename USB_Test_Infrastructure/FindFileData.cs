using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000017 RID: 23
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct FindFileData
	{
		// Token: 0x0400007A RID: 122
		public uint dwFileAttributes;

		// Token: 0x0400007B RID: 123
		public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;

		// Token: 0x0400007C RID: 124
		public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;

		// Token: 0x0400007D RID: 125
		public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;

		// Token: 0x0400007E RID: 126
		public uint FileSizeHigh;

		// Token: 0x0400007F RID: 127
		public uint FileSizeLow;

		// Token: 0x04000080 RID: 128
		public uint Reserved0;

		// Token: 0x04000081 RID: 129
		public uint Reserved1;

		// Token: 0x04000082 RID: 130
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string FileName;

		// Token: 0x04000083 RID: 131
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
		public string Alternate;
	}
}
