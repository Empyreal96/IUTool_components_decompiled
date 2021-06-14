using System;
using System.Runtime.InteropServices;

namespace Microsoft.Phone.Test.TestMetadata.Helper
{
	// Token: 0x0200000C RID: 12
	internal static class NativeMethods
	{
		// Token: 0x06000012 RID: 18
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		// Token: 0x06000013 RID: 19
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int CloseHandle(IntPtr hObject);

		// Token: 0x06000014 RID: 20
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr pFileMappigAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

		// Token: 0x06000015 RID: 21
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint GetFileSize(IntPtr hFile, ref uint lpFileSizeHigh);

		// Token: 0x06000016 RID: 22
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, IntPtr dwNumberOfBytesToMap);

		// Token: 0x06000017 RID: 23
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int UnmapViewOfFile(IntPtr lpBaseAddress);

		// Token: 0x06000018 RID: 24
		[DllImport("dbgHelp.dll", SetLastError = true)]
		public static extern IntPtr ImageDirectoryEntryToDataEx(IntPtr imageBase, int mappedAsImage, ushort directoryEntry, [In] [Out] ref uint size, [In] [Out] ref IntPtr foundHeader);

		// Token: 0x06000019 RID: 25
		[DllImport("dbgHelp.dll", SetLastError = true)]
		public static extern IntPtr ImageNtHeader(IntPtr imageBase);

		// Token: 0x0600001A RID: 26
		[DllImport("dbgHelp.dll", SetLastError = true)]
		public static extern IntPtr ImageRvaToVa(IntPtr ntHeaders, IntPtr imageBase, uint rva, IntPtr lastRvaSection);

		// Token: 0x04000050 RID: 80
		public const uint FileShareRead = 1U;

		// Token: 0x04000051 RID: 81
		public const uint FileShareWrite = 2U;

		// Token: 0x04000052 RID: 82
		public const uint FileShareDelete = 4U;

		// Token: 0x04000053 RID: 83
		public const uint OpenExisting = 3U;

		// Token: 0x04000054 RID: 84
		public const uint GenericRead = 2147483648U;

		// Token: 0x04000055 RID: 85
		public const uint GenericWrite = 1073741824U;

		// Token: 0x04000056 RID: 86
		public const uint FileFlagNoBuffering = 536870912U;

		// Token: 0x04000057 RID: 87
		public const uint FileReadAttributes = 128U;

		// Token: 0x04000058 RID: 88
		public const uint FileWriteAttributes = 256U;

		// Token: 0x04000059 RID: 89
		public const uint ErrorInsufficientBuffer = 122U;

		// Token: 0x0400005A RID: 90
		public const int InvalidHandleValue = -1;

		// Token: 0x0400005B RID: 91
		public const int ErrorAlreadyExists = 183;

		// Token: 0x0400005C RID: 92
		public const uint PageReadonly = 2U;

		// Token: 0x0400005D RID: 93
		public const uint FileMapRead = 4U;

		// Token: 0x0400005E RID: 94
		public const ushort ImageDirectoryEntryExport = 0;

		// Token: 0x0400005F RID: 95
		public const ushort ImageDirectoryEntryImport = 1;

		// Token: 0x04000060 RID: 96
		public const ushort ImageDirectoryEntryDelayLoadImport = 13;

		// Token: 0x04000061 RID: 97
		public const ushort ImageDirectoryEntryComDescriptor = 14;
	}
}
