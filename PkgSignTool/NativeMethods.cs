using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgSignTool
{
	// Token: 0x02000002 RID: 2
	internal class NativeMethods
	{
		// Token: 0x06000001 RID: 1
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetFileTime(SafeFileHandle hFile, IntPtr lpCreationTime, IntPtr lpLastAccessTime, ref long lpLastWriteTime);

		// Token: 0x06000002 RID: 2
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetFileTime(SafeFileHandle hFile, ref long lpCreationTime, IntPtr lpLastAccessTime, IntPtr lpLastWriteTime);

		// Token: 0x06000003 RID: 3
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetFileTime(SafeFileHandle hFile, out long lpCreationTime, IntPtr lpLastAccessTime, IntPtr lpLastWriteTime);

		// Token: 0x06000004 RID: 4
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeFileHandle CreateFile(string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

		// Token: 0x06000005 RID: 5 RVA: 0x00002050 File Offset: 0x00000250
		public static void SetLastWriteTimeLong(string file, DateTime time)
		{
			using (SafeFileHandle safeFileHandle = NativeMethods.CreateFile(LongPath.GetFullPathUNC(file), 1073741824U, 2U, IntPtr.Zero, 3U, 128U, IntPtr.Zero))
			{
				if (safeFileHandle.IsInvalid)
				{
					throw new Exception("CreateFile() failed while calling SetLastWriteTimeLong().  GetLastError() = " + Marshal.GetLastWin32Error().ToString(CultureInfo.InvariantCulture));
				}
				long num = time.ToFileTime();
				if (!NativeMethods.SetFileTime(safeFileHandle, IntPtr.Zero, IntPtr.Zero, ref num))
				{
					throw new Exception("SetFileTime() failed while calling SetLastWriteTimeLong().  GetLastError() = " + Marshal.GetLastWin32Error().ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002104 File Offset: 0x00000304
		public static void SetCreationTimeLong(string file, DateTime time)
		{
			using (SafeFileHandle safeFileHandle = NativeMethods.CreateFile(LongPath.GetFullPathUNC(file), 1073741824U, 2U, IntPtr.Zero, 3U, 128U, IntPtr.Zero))
			{
				if (safeFileHandle.IsInvalid)
				{
					throw new Exception("CreateFile() failed while calling SetCreationTimeLong().  GetLastError() = " + Marshal.GetLastWin32Error().ToString(CultureInfo.InvariantCulture));
				}
				long num = time.ToFileTime();
				if (!NativeMethods.SetFileTime(safeFileHandle, ref num, IntPtr.Zero, IntPtr.Zero))
				{
					throw new Exception("SetFileTime() failed while calling SetCreationTimeLong().  GetLastError() = " + Marshal.GetLastWin32Error().ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021B8 File Offset: 0x000003B8
		public static DateTime GetCreationTimeLong(string file)
		{
			DateTime result;
			using (SafeFileHandle safeFileHandle = NativeMethods.CreateFile(LongPath.GetFullPathUNC(file), 2147483648U, 1U, IntPtr.Zero, 3U, 128U, IntPtr.Zero))
			{
				if (safeFileHandle.IsInvalid)
				{
					throw new Exception("CreateFile() failed while calling GetCreationTimeLong().  GetLastError() = " + Marshal.GetLastWin32Error().ToString(CultureInfo.InvariantCulture));
				}
				long fileTime;
				if (!NativeMethods.GetFileTime(safeFileHandle, out fileTime, IntPtr.Zero, IntPtr.Zero))
				{
					throw new Exception("GetFileTime() failed while calling GetCreationTimeLong().  GetLastError() = " + Marshal.GetLastWin32Error().ToString(CultureInfo.InvariantCulture));
				}
				result = DateTime.FromFileTime(fileTime);
			}
			return result;
		}

		// Token: 0x04000001 RID: 1
		private const uint FILE_ATTRIBUTE_NORMAL = 128U;

		// Token: 0x04000002 RID: 2
		private const uint GENERIC_READ = 2147483648U;

		// Token: 0x04000003 RID: 3
		private const uint GENERIC_WRITE = 1073741824U;

		// Token: 0x04000004 RID: 4
		private const uint CREATE_NEW = 1U;

		// Token: 0x04000005 RID: 5
		private const uint CREATE_ALWAYS = 2U;

		// Token: 0x04000006 RID: 6
		private const uint OPEN_EXISTING = 3U;

		// Token: 0x04000007 RID: 7
		private const uint FILE_SHARE_READ = 1U;

		// Token: 0x04000008 RID: 8
		private const uint FILE_SHARE_WRITE = 2U;
	}
}
