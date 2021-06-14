using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Composition.ToolBox.IO
{
	// Token: 0x0200001B RID: 27
	internal class NativeMethods
	{
		// Token: 0x06000090 RID: 144
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern int GetFullPathName(string inPath, int stringSize, StringBuilder longPath, StringBuilder fileName);

		// Token: 0x06000091 RID: 145
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern uint GetFileAttributes(string lpFileName);

		// Token: 0x06000092 RID: 146
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);

		// Token: 0x06000093 RID: 147
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool DeleteFile(string lpExistingFileName);

		// Token: 0x06000094 RID: 148
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool SetFileAttributes(string lpExistingFileName, NativeMethods.EFileAttributes dwFlagsAndAttributes);

		// Token: 0x06000095 RID: 149
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindFirstFile(string lpFileName, out NativeMethods.WIN32_FIND_DATA lpFindFileData);

		// Token: 0x06000096 RID: 150
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindNextFile(IntPtr hFindFile, out NativeMethods.WIN32_FIND_DATA lpFindFileData);

		// Token: 0x06000097 RID: 151
		[DllImport("kernel32.dll")]
		public static extern bool FindClose(IntPtr hFindFile);

		// Token: 0x06000098 RID: 152
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern int CreateDirectory(string pszPath, IntPtr psa);

		// Token: 0x06000099 RID: 153
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern int RemoveDirectory(string pszPath);

		// Token: 0x0600009A RID: 154
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeFileHandle CreateFile(string lpFileName, NativeMethods.EFileAccess dwDesiredAccess, NativeMethods.EFileShare dwShareMode, IntPtr lpSecurityAttributes, NativeMethods.ECreationDisposition dwCreationDisposition, NativeMethods.EFileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

		// Token: 0x0600009B RID: 155
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern uint IU_GetStagedAndCompressedSize(string file, out ulong fileSize, out ulong stagedSize, out ulong compressedSize);

		// Token: 0x04000064 RID: 100
		internal const int MAX_PATH = 260;

		// Token: 0x04000065 RID: 101
		private const string KERNEL32 = "kernel32.dll";

		// Token: 0x04000066 RID: 102
		private const CharSet CHAR_SET = CharSet.Unicode;

		// Token: 0x02000021 RID: 33
		internal enum ECreationDisposition : uint
		{
			// Token: 0x04000077 RID: 119
			New = 1U,
			// Token: 0x04000078 RID: 120
			CreateAlways,
			// Token: 0x04000079 RID: 121
			OpenExisting,
			// Token: 0x0400007A RID: 122
			OpenAlways,
			// Token: 0x0400007B RID: 123
			TruncateExisting
		}

		// Token: 0x02000022 RID: 34
		[Flags]
		internal enum EFileAccess : uint
		{
			// Token: 0x0400007D RID: 125
			GenericRead = 2147483648U,
			// Token: 0x0400007E RID: 126
			GenericWrite = 1073741824U,
			// Token: 0x0400007F RID: 127
			GenericExecute = 536870912U,
			// Token: 0x04000080 RID: 128
			GenericAll = 268435456U
		}

		// Token: 0x02000023 RID: 35
		[Flags]
		internal enum EFileAttributes : uint
		{
			// Token: 0x04000082 RID: 130
			Readonly = 1U,
			// Token: 0x04000083 RID: 131
			Hidden = 2U,
			// Token: 0x04000084 RID: 132
			System = 4U,
			// Token: 0x04000085 RID: 133
			Directory = 16U,
			// Token: 0x04000086 RID: 134
			Archive = 32U,
			// Token: 0x04000087 RID: 135
			Device = 64U,
			// Token: 0x04000088 RID: 136
			Normal = 128U,
			// Token: 0x04000089 RID: 137
			Temporary = 256U,
			// Token: 0x0400008A RID: 138
			SparseFile = 512U,
			// Token: 0x0400008B RID: 139
			ReparsePoint = 1024U,
			// Token: 0x0400008C RID: 140
			Compressed = 2048U,
			// Token: 0x0400008D RID: 141
			Offline = 4096U,
			// Token: 0x0400008E RID: 142
			NotContentIndexed = 8192U,
			// Token: 0x0400008F RID: 143
			Encrypted = 16384U,
			// Token: 0x04000090 RID: 144
			Write_Through = 2147483648U,
			// Token: 0x04000091 RID: 145
			Overlapped = 1073741824U,
			// Token: 0x04000092 RID: 146
			NoBuffering = 536870912U,
			// Token: 0x04000093 RID: 147
			RandomAccess = 268435456U,
			// Token: 0x04000094 RID: 148
			SequentialScan = 134217728U,
			// Token: 0x04000095 RID: 149
			DeleteOnClose = 67108864U,
			// Token: 0x04000096 RID: 150
			BackupSemantics = 33554432U,
			// Token: 0x04000097 RID: 151
			PosixSemantics = 16777216U,
			// Token: 0x04000098 RID: 152
			OpenReparsePoint = 2097152U,
			// Token: 0x04000099 RID: 153
			OpenNoRecall = 1048576U,
			// Token: 0x0400009A RID: 154
			FirstPipeInstance = 524288U
		}

		// Token: 0x02000024 RID: 36
		[Flags]
		internal enum EFileShare : uint
		{
			// Token: 0x0400009C RID: 156
			None = 0U,
			// Token: 0x0400009D RID: 157
			Read = 1U,
			// Token: 0x0400009E RID: 158
			Write = 2U,
			// Token: 0x0400009F RID: 159
			Delete = 4U
		}

		// Token: 0x02000025 RID: 37
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct WIN32_FIND_DATA
		{
			// Token: 0x040000A0 RID: 160
			internal FileAttributes dwFileAttributes;

			// Token: 0x040000A1 RID: 161
			internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;

			// Token: 0x040000A2 RID: 162
			internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;

			// Token: 0x040000A3 RID: 163
			internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;

			// Token: 0x040000A4 RID: 164
			internal int nFileSizeHigh;

			// Token: 0x040000A5 RID: 165
			internal int nFileSizeLow;

			// Token: 0x040000A6 RID: 166
			internal int dwReserved0;

			// Token: 0x040000A7 RID: 167
			internal int dwReserved1;

			// Token: 0x040000A8 RID: 168
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			internal string cFileName;

			// Token: 0x040000A9 RID: 169
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			internal string cAlternate;
		}
	}
}
