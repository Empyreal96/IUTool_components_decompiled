using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200000F RID: 15
	internal static class NativeMethods
	{
		// Token: 0x0600006C RID: 108
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern int IU_GetCanonicalUNCPath(string strPath, StringBuilder pathBuffer, int cchPathBuffer);

		// Token: 0x0600006D RID: 109
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern int IU_FreeStringList(IntPtr rgFiles, int cFiles);

		// Token: 0x0600006E RID: 110
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool IU_FileExists(string strFile);

		// Token: 0x0600006F RID: 111
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool IU_DirectoryExists(string strDir);

		// Token: 0x06000070 RID: 112
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern void IU_EnsureDirectoryExists(string path);

		// Token: 0x06000071 RID: 113
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern void IU_CleanDirectory(string strPath, [MarshalAs(UnmanagedType.Bool)] bool bRemoveDirectory);

		// Token: 0x06000072 RID: 114
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern int IU_GetAllFiles(string strFolder, string strSearchPattern, [MarshalAs(UnmanagedType.Bool)] bool fRecursive, out IntPtr rgFiles, out int cFiles);

		// Token: 0x06000073 RID: 115
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern int IU_GetAllDirectories(string strFolder, string strSearchPattern, [MarshalAs(UnmanagedType.Bool)] bool fRecursive, out IntPtr rgDirectories, out int cDirectories);

		// Token: 0x06000074 RID: 116 RVA: 0x000036F9 File Offset: 0x000018F9
		internal static int MakeHRFromErrorCode(int errorCode)
		{
			return -2147024896 | errorCode;
		}

		// Token: 0x06000075 RID: 117
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr va_list_arguments);

		// Token: 0x06000076 RID: 118
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeleteFile(string lpFileName);

		// Token: 0x06000077 RID: 119
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool RemoveDirectory(string lpPathName);

		// Token: 0x06000078 RID: 120
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool MoveFile(string lpPathNameFrom, string lpPathNameTo);

		// Token: 0x06000079 RID: 121
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CopyFile(string src, string dst, [MarshalAs(UnmanagedType.Bool)] bool failIfExists);

		// Token: 0x0600007A RID: 122
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern SafeFileHandle CreateFile(string lpFileName, NativeMethods.EFileAccess dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		// Token: 0x0600007B RID: 123
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern FileAttributes GetFileAttributes(string lpFileName);

		// Token: 0x0600007C RID: 124
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool SetFileAttributes(string lpFileName, FileAttributes attributes);

		// Token: 0x04000018 RID: 24
		private const string STRING_IUCOMMON_DLL = "UpdateDLL.dll";

		// Token: 0x04000019 RID: 25
		private const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;

		// Token: 0x0400001A RID: 26
		private const CharSet CHAR_SET = CharSet.Unicode;

		// Token: 0x0400001B RID: 27
		internal const int ERROR_FILE_NOT_FOUND = 2;

		// Token: 0x0400001C RID: 28
		internal const int ERROR_PATH_NOT_FOUND = 3;

		// Token: 0x0400001D RID: 29
		internal const int ERROR_ACCESS_DENIED = 5;

		// Token: 0x0400001E RID: 30
		internal const int ERROR_INVALID_DRIVE = 15;

		// Token: 0x0400001F RID: 31
		internal const int ERROR_NO_MORE_FILES = 18;

		// Token: 0x04000020 RID: 32
		internal const int ERROR_INVALID_NAME = 123;

		// Token: 0x04000021 RID: 33
		internal const int ERROR_ALREADY_EXISTS = 183;

		// Token: 0x04000022 RID: 34
		internal const int ERROR_FILENAME_EXCED_RANGE = 206;

		// Token: 0x04000023 RID: 35
		internal const int ERROR_DIRECTORY = 267;

		// Token: 0x04000024 RID: 36
		internal const int ERROR_OPERATION_ABORTED = 995;

		// Token: 0x04000025 RID: 37
		internal const int INVALID_FILE_ATTRIBUTES = -1;

		// Token: 0x04000026 RID: 38
		internal const int FORMAT_MESSAGE_IGNORE_INSERTS = 512;

		// Token: 0x04000027 RID: 39
		internal const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;

		// Token: 0x04000028 RID: 40
		internal const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192;

		// Token: 0x04000029 RID: 41
		private const string STRING_KERNEL32_DLL = "kernel32.dll";

		// Token: 0x0200003C RID: 60
		[Flags]
		internal enum EFileAccess : uint
		{
			// Token: 0x040000A4 RID: 164
			GenericRead = 2147483648U,
			// Token: 0x040000A5 RID: 165
			GenericWrite = 1073741824U,
			// Token: 0x040000A6 RID: 166
			GenericExecute = 536870912U,
			// Token: 0x040000A7 RID: 167
			GenericAll = 268435456U
		}
	}
}
