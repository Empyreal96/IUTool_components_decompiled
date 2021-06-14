using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Tools.IO.Interop
{
	// Token: 0x02000007 RID: 7
	internal static class NativeMethods
	{
		// Token: 0x06000071 RID: 113 RVA: 0x00003745 File Offset: 0x00001945
		internal static int MakeHRFromErrorCode(int errorCode)
		{
			return -2147024896 | errorCode;
		}

		// Token: 0x06000072 RID: 114
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CopyFile(string src, string dst, [MarshalAs(UnmanagedType.Bool)] bool failIfExists);

		// Token: 0x06000073 RID: 115
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern SafeFindHandle FindFirstFile(string lpFileName, out NativeMethods.WIN32_FIND_DATA lpFindFileData);

		// Token: 0x06000074 RID: 116
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FindNextFile(SafeFindHandle hFindFile, out NativeMethods.WIN32_FIND_DATA lpFindFileData);

		// Token: 0x06000075 RID: 117
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FindClose(IntPtr hFindFile);

		// Token: 0x06000076 RID: 118
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern uint GetFullPathName(string lpFileName, uint nBufferLength, StringBuilder lpBuffer, IntPtr mustBeNull);

		// Token: 0x06000077 RID: 119
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeleteFile(string lpFileName);

		// Token: 0x06000078 RID: 120
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeleteVolumeMountPoint(string mountPoint);

		// Token: 0x06000079 RID: 121
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool RemoveDirectory(string lpPathName);

		// Token: 0x0600007A RID: 122
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes);

		// Token: 0x0600007B RID: 123
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool MoveFile(string lpPathNameFrom, string lpPathNameTo);

		// Token: 0x0600007C RID: 124
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern SafeFileHandle CreateFile(string lpFileName, NativeMethods.EFileAccess dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		// Token: 0x0600007D RID: 125
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern FileAttributes GetFileAttributes(string lpFileName);

		// Token: 0x0600007E RID: 126
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr va_list_arguments);

		// Token: 0x0600007F RID: 127
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetFileAttributesEx(string name, int fileInfoLevel, ref NativeMethods.WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);

		// Token: 0x06000080 RID: 128
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		internal static extern int SetErrorMode(int newMode);

		// Token: 0x06000081 RID: 129
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetFileAttributes(string name, int attr);

		// Token: 0x06000082 RID: 130
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetFileTime(SafeFileHandle hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);

		// Token: 0x06000083 RID: 131
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern int GetFileType(SafeFileHandle handle);

		// Token: 0x06000084 RID: 132 RVA: 0x00003750 File Offset: 0x00001950
		internal static SafeFileHandle SafeCreateFile(string lpFileName, NativeMethods.EFileAccess dwDesiredAccess, uint dwShareMode, IntPtr securityAttrs, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile)
		{
			SafeFileHandle safeFileHandle = NativeMethods.CreateFile(lpFileName, dwDesiredAccess, dwShareMode, securityAttrs, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
			if (!safeFileHandle.IsInvalid && NativeMethods.GetFileType(safeFileHandle) != 1)
			{
				safeFileHandle.Dispose();
				throw new NotSupportedException("FileStream was asked to open a device that was not a file. For support for devices like 'com1:' or 'lpt1:', call CreateFile, then use the FileStream constructors that take an OS handle as an IntPtr.");
			}
			return safeFileHandle;
		}

		// Token: 0x04000022 RID: 34
		internal const int ERROR_FILE_NOT_FOUND = 2;

		// Token: 0x04000023 RID: 35
		internal const int ERROR_PATH_NOT_FOUND = 3;

		// Token: 0x04000024 RID: 36
		internal const int ERROR_ACCESS_DENIED = 5;

		// Token: 0x04000025 RID: 37
		internal const int ERROR_INVALID_DRIVE = 15;

		// Token: 0x04000026 RID: 38
		internal const int ERROR_NO_MORE_FILES = 18;

		// Token: 0x04000027 RID: 39
		internal const int ERROR_NOT_READY = 21;

		// Token: 0x04000028 RID: 40
		internal const int ERROR_INVALID_PARAMETER = 87;

		// Token: 0x04000029 RID: 41
		internal const int ERROR_INVALID_NAME = 123;

		// Token: 0x0400002A RID: 42
		internal const int ERROR_ALREADY_EXISTS = 183;

		// Token: 0x0400002B RID: 43
		internal const int ERROR_FILENAME_EXCED_RANGE = 206;

		// Token: 0x0400002C RID: 44
		internal const int ERROR_DIRECTORY = 267;

		// Token: 0x0400002D RID: 45
		internal const int ERROR_OPERATION_ABORTED = 995;

		// Token: 0x0400002E RID: 46
		internal const int INVALID_FILE_ATTRIBUTES = -1;

		// Token: 0x0400002F RID: 47
		internal const int SEM_FAILCRITICALERRORS = 1;

		// Token: 0x04000030 RID: 48
		internal const int FILE_TYPE_DISK = 1;

		// Token: 0x04000031 RID: 49
		internal const int FILE_TYPE_CHAR = 2;

		// Token: 0x04000032 RID: 50
		internal const int FILE_TYPE_PIPE = 3;

		// Token: 0x04000033 RID: 51
		internal const int MAX_PATH = 260;

		// Token: 0x04000034 RID: 52
		internal const int MAX_LONG_PATH = 32000;

		// Token: 0x04000035 RID: 53
		internal const int MAX_ALTERNATE = 14;

		// Token: 0x04000036 RID: 54
		internal const string LongPathPrefix = "\\\\?\\";

		// Token: 0x04000037 RID: 55
		internal const string LongUncPathPrefix = "\\\\?\\UNC\\";

		// Token: 0x04000038 RID: 56
		internal const int FORMAT_MESSAGE_IGNORE_INSERTS = 512;

		// Token: 0x04000039 RID: 57
		internal const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;

		// Token: 0x0400003A RID: 58
		internal const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192;

		// Token: 0x0400003B RID: 59
		internal const int IO_REPARSE_TAG_MOUNT_POINT = -1610612733;

		// Token: 0x0200000B RID: 11
		[Flags]
		internal enum EFileAccess : uint
		{
			// Token: 0x0400004F RID: 79
			GenericRead = 2147483648U,
			// Token: 0x04000050 RID: 80
			GenericWrite = 1073741824U,
			// Token: 0x04000051 RID: 81
			GenericExecute = 536870912U,
			// Token: 0x04000052 RID: 82
			GenericAll = 268435456U
		}

		// Token: 0x0200000C RID: 12
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct WIN32_FIND_DATA
		{
			// Token: 0x04000053 RID: 83
			internal FileAttributes dwFileAttributes;

			// Token: 0x04000054 RID: 84
			internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;

			// Token: 0x04000055 RID: 85
			internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;

			// Token: 0x04000056 RID: 86
			internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;

			// Token: 0x04000057 RID: 87
			internal int nFileSizeHigh;

			// Token: 0x04000058 RID: 88
			internal int nFileSizeLow;

			// Token: 0x04000059 RID: 89
			internal int dwReserved0;

			// Token: 0x0400005A RID: 90
			internal int dwReserved1;

			// Token: 0x0400005B RID: 91
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			internal string cFileName;

			// Token: 0x0400005C RID: 92
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			internal string cAlternate;
		}

		// Token: 0x0200000D RID: 13
		[Serializable]
		internal struct WIN32_FILE_ATTRIBUTE_DATA
		{
			// Token: 0x06000090 RID: 144 RVA: 0x00003B40 File Offset: 0x00001D40
			[SecurityCritical]
			internal void PopulateFrom(NativeMethods.WIN32_FIND_DATA findData)
			{
				this.fileAttributes = (int)findData.dwFileAttributes;
				this.ftCreationTimeLow = (uint)findData.ftCreationTime.dwLowDateTime;
				this.ftCreationTimeHigh = (uint)findData.ftCreationTime.dwHighDateTime;
				this.ftLastAccessTimeLow = (uint)findData.ftLastAccessTime.dwLowDateTime;
				this.ftLastAccessTimeHigh = (uint)findData.ftLastAccessTime.dwHighDateTime;
				this.ftLastWriteTimeLow = (uint)findData.ftLastWriteTime.dwLowDateTime;
				this.ftLastWriteTimeHigh = (uint)findData.ftLastWriteTime.dwHighDateTime;
				this.fileSizeHigh = findData.nFileSizeHigh;
				this.fileSizeLow = findData.nFileSizeLow;
			}

			// Token: 0x0400005D RID: 93
			internal int fileAttributes;

			// Token: 0x0400005E RID: 94
			internal uint ftCreationTimeLow;

			// Token: 0x0400005F RID: 95
			internal uint ftCreationTimeHigh;

			// Token: 0x04000060 RID: 96
			internal uint ftLastAccessTimeLow;

			// Token: 0x04000061 RID: 97
			internal uint ftLastAccessTimeHigh;

			// Token: 0x04000062 RID: 98
			internal uint ftLastWriteTimeLow;

			// Token: 0x04000063 RID: 99
			internal uint ftLastWriteTimeHigh;

			// Token: 0x04000064 RID: 100
			internal int fileSizeHigh;

			// Token: 0x04000065 RID: 101
			internal int fileSizeLow;
		}
	}
}
