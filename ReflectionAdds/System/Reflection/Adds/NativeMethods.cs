using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.Reflection.Adds
{
	// Token: 0x0200000C RID: 12
	internal static class NativeMethods
	{
		// Token: 0x0600006E RID: 110
		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, IntPtr securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

		// Token: 0x0600006F RID: 111
		[DllImport("kernel32.dll")]
		private static extern int GetFileType(SafeFileHandle handle);

		// Token: 0x06000070 RID: 112 RVA: 0x0000322C File Offset: 0x0000142C
		internal static SafeFileHandle SafeOpenFile(string fileName)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("fileName");
			}
			bool flag2 = fileName.Length == 0 || fileName.StartsWith("\\\\.\\", StringComparison.Ordinal);
			if (flag2)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidFileName, new object[]
				{
					fileName
				}));
			}
			SafeFileHandle safeFileHandle = NativeMethods.CreateFile(fileName, int.MinValue, FileShare.Read, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
			bool isInvalid = safeFileHandle.IsInvalid;
			if (isInvalid)
			{
				int hrforLastWin32Error = Marshal.GetHRForLastWin32Error();
				Marshal.ThrowExceptionForHR(hrforLastWin32Error);
			}
			else
			{
				int fileType = NativeMethods.GetFileType(safeFileHandle);
				bool flag3 = fileType != 1;
				if (flag3)
				{
					safeFileHandle.Dispose();
					throw new ArgumentException(Resources.UnsupportedImageType);
				}
			}
			return safeFileHandle;
		}

		// Token: 0x06000071 RID: 113
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern int GetFileSize(SafeFileHandle hFile, out int highSize);

		// Token: 0x06000072 RID: 114 RVA: 0x000032F4 File Offset: 0x000014F4
		internal static long FileSize(SafeFileHandle handle)
		{
			int num = 0;
			int fileSize = NativeMethods.GetFileSize(handle, out num);
			bool flag = fileSize == -1;
			if (flag)
			{
				int hrforLastWin32Error = Marshal.GetHRForLastWin32Error();
				Marshal.ThrowExceptionForHR(hrforLastWin32Error);
			}
			return (long)num << 32 | (long)((ulong)fileSize);
		}

		// Token: 0x06000073 RID: 115
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr handle);

		// Token: 0x06000074 RID: 116
		[DllImport("kernel32.dll", BestFitMapping = false, SetLastError = true)]
		public static extern NativeMethods.SafeWin32Handle CreateFileMapping(SafeFileHandle hFile, IntPtr lpFileMappingAttributes, NativeMethods.PageProtection flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

		// Token: 0x06000075 RID: 117
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern NativeMethods.SafeMapViewHandle MapViewOfFile(NativeMethods.SafeWin32Handle hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, IntPtr dwNumberOfBytesToMap);

		// Token: 0x06000076 RID: 118
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnmapViewOfFile(IntPtr baseAddress);

		// Token: 0x04000039 RID: 57
		private const string Kernel32LibraryName = "kernel32.dll";

		// Token: 0x0400003A RID: 58
		private const int FILE_TYPE_DISK = 1;

		// Token: 0x0400003B RID: 59
		private const int GENERIC_READ = -2147483648;

		// Token: 0x02000024 RID: 36
		public sealed class SafeWin32Handle : SafeHandleZeroOrMinusOneIsInvalid
		{
			// Token: 0x0600011A RID: 282 RVA: 0x00005438 File Offset: 0x00003638
			private SafeWin32Handle() : base(true)
			{
			}

			// Token: 0x0600011B RID: 283 RVA: 0x00005444 File Offset: 0x00003644
			protected override bool ReleaseHandle()
			{
				return NativeMethods.CloseHandle(this.handle);
			}
		}

		// Token: 0x02000025 RID: 37
		[Flags]
		public enum PageProtection : uint
		{
			// Token: 0x04000086 RID: 134
			NoAccess = 1U,
			// Token: 0x04000087 RID: 135
			Readonly = 2U,
			// Token: 0x04000088 RID: 136
			ReadWrite = 4U,
			// Token: 0x04000089 RID: 137
			WriteCopy = 8U,
			// Token: 0x0400008A RID: 138
			Execute = 16U,
			// Token: 0x0400008B RID: 139
			ExecuteRead = 32U,
			// Token: 0x0400008C RID: 140
			ExecuteReadWrite = 64U,
			// Token: 0x0400008D RID: 141
			ExecuteWriteCopy = 128U,
			// Token: 0x0400008E RID: 142
			Guard = 256U,
			// Token: 0x0400008F RID: 143
			NoCache = 512U,
			// Token: 0x04000090 RID: 144
			WriteCombine = 1024U
		}

		// Token: 0x02000026 RID: 38
		public sealed class SafeMapViewHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			// Token: 0x0600011C RID: 284 RVA: 0x00005438 File Offset: 0x00003638
			private SafeMapViewHandle() : base(true)
			{
			}

			// Token: 0x0600011D RID: 285 RVA: 0x00005464 File Offset: 0x00003664
			protected override bool ReleaseHandle()
			{
				return NativeMethods.UnmapViewOfFile(this.handle);
			}

			// Token: 0x1700006C RID: 108
			// (get) Token: 0x0600011E RID: 286 RVA: 0x00005484 File Offset: 0x00003684
			public IntPtr BaseAddress
			{
				get
				{
					return this.handle;
				}
			}
		}
	}
}
