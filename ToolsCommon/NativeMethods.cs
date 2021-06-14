using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000047 RID: 71
	internal static class NativeMethods
	{
		// Token: 0x060001D2 RID: 466
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern int IU_GetCanonicalUNCPath(string strPath, StringBuilder pathBuffer, int cchPathBuffer);

		// Token: 0x060001D3 RID: 467
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern int IU_FreeStringList(IntPtr rgFiles, int cFiles);

		// Token: 0x060001D4 RID: 468
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool IU_FileExists(string strFile);

		// Token: 0x060001D5 RID: 469
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool IU_DirectoryExists(string strDir);

		// Token: 0x060001D6 RID: 470
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern void IU_EnsureDirectoryExists(string path);

		// Token: 0x060001D7 RID: 471
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern void IU_CleanDirectory(string strPath, [MarshalAs(UnmanagedType.Bool)] bool bRemoveDirectory);

		// Token: 0x060001D8 RID: 472
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern int IU_GetAllFiles(string strFolder, string strSearchPattern, [MarshalAs(UnmanagedType.Bool)] bool fRecursive, out IntPtr rgFiles, out int cFiles);

		// Token: 0x060001D9 RID: 473
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern int IU_GetAllDirectories(string strFolder, string strSearchPattern, [MarshalAs(UnmanagedType.Bool)] bool fRecursive, out IntPtr rgDirectories, out int cDirectories);

		// Token: 0x060001DA RID: 474 RVA: 0x00009990 File Offset: 0x00007B90
		internal static int MakeHRFromErrorCode(int errorCode)
		{
			return -2147024896 | errorCode;
		}

		// Token: 0x060001DB RID: 475
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr va_list_arguments);

		// Token: 0x060001DC RID: 476
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeleteFile(string lpFileName);

		// Token: 0x060001DD RID: 477
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool RemoveDirectory(string lpPathName);

		// Token: 0x060001DE RID: 478
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool MoveFile(string lpPathNameFrom, string lpPathNameTo);

		// Token: 0x060001DF RID: 479
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CopyFile(string src, string dst, [MarshalAs(UnmanagedType.Bool)] bool failIfExists);

		// Token: 0x060001E0 RID: 480
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern SafeFileHandle CreateFile(string lpFileName, NativeMethods.EFileAccess dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		// Token: 0x060001E1 RID: 481
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern FileAttributes GetFileAttributes(string lpFileName);

		// Token: 0x060001E2 RID: 482
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool SetFileAttributes(string lpFileName, FileAttributes attributes);

		// Token: 0x040000E8 RID: 232
		private const string STRING_IUCOMMON_DLL = "UpdateDLL.dll";

		// Token: 0x040000E9 RID: 233
		private const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;

		// Token: 0x040000EA RID: 234
		private const CharSet CHAR_SET = CharSet.Unicode;

		// Token: 0x040000EB RID: 235
		internal const int ERROR_FILE_NOT_FOUND = 2;

		// Token: 0x040000EC RID: 236
		internal const int ERROR_PATH_NOT_FOUND = 3;

		// Token: 0x040000ED RID: 237
		internal const int ERROR_ACCESS_DENIED = 5;

		// Token: 0x040000EE RID: 238
		internal const int ERROR_INVALID_DRIVE = 15;

		// Token: 0x040000EF RID: 239
		internal const int ERROR_NO_MORE_FILES = 18;

		// Token: 0x040000F0 RID: 240
		internal const int ERROR_INVALID_NAME = 123;

		// Token: 0x040000F1 RID: 241
		internal const int ERROR_ALREADY_EXISTS = 183;

		// Token: 0x040000F2 RID: 242
		internal const int ERROR_FILENAME_EXCED_RANGE = 206;

		// Token: 0x040000F3 RID: 243
		internal const int ERROR_DIRECTORY = 267;

		// Token: 0x040000F4 RID: 244
		internal const int ERROR_OPERATION_ABORTED = 995;

		// Token: 0x040000F5 RID: 245
		internal const int INVALID_FILE_ATTRIBUTES = -1;

		// Token: 0x040000F6 RID: 246
		internal const int FORMAT_MESSAGE_IGNORE_INSERTS = 512;

		// Token: 0x040000F7 RID: 247
		internal const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;

		// Token: 0x040000F8 RID: 248
		internal const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192;

		// Token: 0x040000F9 RID: 249
		private const string STRING_KERNEL32_DLL = "kernel32.dll";

		// Token: 0x0200006C RID: 108
		[Flags]
		internal enum EFileAccess : uint
		{
			// Token: 0x04000199 RID: 409
			GenericRead = 2147483648U,
			// Token: 0x0400019A RID: 410
			GenericWrite = 1073741824U,
			// Token: 0x0400019B RID: 411
			GenericExecute = 536870912U,
			// Token: 0x0400019C RID: 412
			GenericAll = 268435456U
		}
	}
}
