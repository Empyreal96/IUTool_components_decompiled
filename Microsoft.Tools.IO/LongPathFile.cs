using System;
using System.IO;
using Microsoft.Tools.IO.Interop;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Tools.IO
{
	// Token: 0x02000004 RID: 4
	public static class LongPathFile
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00002F48 File Offset: 0x00001148
		public static bool Exists(string path)
		{
			bool flag;
			return LongPathCommon.Exists(path, out flag) && !flag;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002F65 File Offset: 0x00001165
		public static void Delete(string path)
		{
			if (!NativeMethods.DeleteFile(LongPathCommon.NormalizeLongPath(path)))
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error("path");
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002F80 File Offset: 0x00001180
		public static void Move(string sourcePath, string destinationPath)
		{
			string lpPathNameFrom = LongPathCommon.NormalizeLongPath(sourcePath, "sourcePath");
			string lpPathNameTo = LongPathCommon.NormalizeLongPath(destinationPath, "destinationPath");
			if (!NativeMethods.MoveFile(lpPathNameFrom, lpPathNameTo))
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error("path");
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002FB8 File Offset: 0x000011B8
		public static void Copy(string sourcePath, string destinationPath, bool overwrite)
		{
			string src = LongPathCommon.NormalizeLongPath(sourcePath, "sourcePath");
			string dst = LongPathCommon.NormalizeLongPath(destinationPath, "destinationPath");
			if (!NativeMethods.CopyFile(src, dst, !overwrite))
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error("path");
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002FF3 File Offset: 0x000011F3
		public static FileStream Open(string path, FileMode mode, FileAccess access)
		{
			return LongPathFile.Open(path, mode, access, FileShare.None);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002FFE File Offset: 0x000011FE
		public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
		{
			return LongPathFile.Open(path, mode, access, share, 0, FileOptions.None);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x0000300B File Offset: 0x0000120B
		public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
		{
			if (bufferSize == 0)
			{
				bufferSize = 1024;
			}
			return new FileStream(LongPathFile.GetFileHandle(LongPathCommon.NormalizeLongPath(path), mode, access, share, options), access, bufferSize, (options & FileOptions.Asynchronous) == FileOptions.Asynchronous);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003040 File Offset: 0x00001240
		public static DateTime GetCreationTime(string path)
		{
			return LongPathFile.GetCreationTimeUtc(path).ToLocalTime();
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000305B File Offset: 0x0000125B
		public static void SetCreationTime(string path, DateTime creationTime)
		{
			LongPathFile.SetCreationTimeUtc(path, creationTime.ToUniversalTime());
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000306C File Offset: 0x0000126C
		public static DateTime GetCreationTimeUtc(string path)
		{
			NativeMethods.WIN32_FILE_ATTRIBUTE_DATA win32FileAttributeData = LongPathCommon.GetWin32FileAttributeData(LongPathCommon.NormalizeLongPath(path));
			return DateTime.FromFileTimeUtc((long)((ulong)win32FileAttributeData.ftCreationTimeHigh << 32 | (ulong)win32FileAttributeData.ftCreationTimeLow));
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000309C File Offset: 0x0000129C
		public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
		{
			using (SafeFileHandle safeFileHandle = LongPathFile.OpenHandle(LongPathCommon.NormalizeLongPath(path)))
			{
				LongPathCommon.SetFileTimes(safeFileHandle, creationTimeUtc.ToFileTimeUtc(), 0L, 0L);
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000030E4 File Offset: 0x000012E4
		public static DateTime GetLastAccessTime(string path)
		{
			return LongPathFile.GetLastAccessTimeUtc(path).ToLocalTime();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000030FF File Offset: 0x000012FF
		public static void SetLastAccessTime(string path, DateTime lastAccessTime)
		{
			LongPathFile.SetLastAccessTimeUtc(path, lastAccessTime.ToUniversalTime());
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003110 File Offset: 0x00001310
		public static DateTime GetLastAccessTimeUtc(string path)
		{
			NativeMethods.WIN32_FILE_ATTRIBUTE_DATA win32FileAttributeData = LongPathCommon.GetWin32FileAttributeData(LongPathCommon.NormalizeLongPath(path));
			return DateTime.FromFileTimeUtc((long)((ulong)win32FileAttributeData.ftLastAccessTimeHigh << 32 | (ulong)win32FileAttributeData.ftLastAccessTimeLow));
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003140 File Offset: 0x00001340
		public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
		{
			using (SafeFileHandle safeFileHandle = LongPathFile.OpenHandle(LongPathCommon.NormalizeLongPath(path)))
			{
				LongPathCommon.SetFileTimes(safeFileHandle, 0L, lastAccessTimeUtc.ToFileTimeUtc(), 0L);
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003188 File Offset: 0x00001388
		public static DateTime GetLastWriteTime(string path)
		{
			return LongPathFile.GetLastWriteTimeUtc(path).ToLocalTime();
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000031A3 File Offset: 0x000013A3
		public static void SetLastWriteTime(string path, DateTime lastWriteTime)
		{
			LongPathFile.SetLastWriteTimeUtc(path, lastWriteTime.ToUniversalTime());
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000031B4 File Offset: 0x000013B4
		public static DateTime GetLastWriteTimeUtc(string path)
		{
			NativeMethods.WIN32_FILE_ATTRIBUTE_DATA win32FileAttributeData = LongPathCommon.GetWin32FileAttributeData(LongPathCommon.NormalizeLongPath(path));
			return DateTime.FromFileTimeUtc((long)((ulong)win32FileAttributeData.ftLastWriteTimeHigh << 32 | (ulong)win32FileAttributeData.ftLastWriteTimeLow));
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000031E4 File Offset: 0x000013E4
		public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
		{
			using (SafeFileHandle safeFileHandle = LongPathFile.OpenHandle(LongPathCommon.NormalizeLongPath(path)))
			{
				LongPathCommon.SetFileTimes(safeFileHandle, 0L, 0L, lastWriteTimeUtc.ToFileTimeUtc());
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002848 File Offset: 0x00000A48
		public static FileAttributes GetAttributes(string path)
		{
			return LongPathCommon.GetAttributes(path);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002850 File Offset: 0x00000A50
		public static void SetAttributes(string path, FileAttributes fileAttributes)
		{
			LongPathCommon.SetAttributes(path, fileAttributes);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000322C File Offset: 0x0000142C
		public static long GetFileLengthBytes(string path)
		{
			NativeMethods.WIN32_FILE_ATTRIBUTE_DATA win32FileAttributeData = LongPathCommon.GetWin32FileAttributeData(LongPathCommon.NormalizeLongPath(path));
			if ((win32FileAttributeData.fileAttributes & 16) != 0)
			{
				throw new FileNotFoundException(string.Format("Could not find file '{0}'", path), path);
			}
			return (long)win32FileAttributeData.fileSizeHigh << 32 | ((long)win32FileAttributeData.fileSizeLow & (long)((ulong)-1));
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003278 File Offset: 0x00001478
		internal static bool InternalExists(string normalizedPath)
		{
			NativeMethods.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(NativeMethods.WIN32_FILE_ATTRIBUTE_DATA);
			return LongPathCommon.FillAttributeInfo(normalizedPath, ref win32_FILE_ATTRIBUTE_DATA, false, false) == 0 && win32_FILE_ATTRIBUTE_DATA.fileAttributes != -1 && (win32_FILE_ATTRIBUTE_DATA.fileAttributes & 16) == 0;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000032B0 File Offset: 0x000014B0
		private static SafeFileHandle GetFileHandle(string normalizedPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
		{
			NativeMethods.EFileAccess underlyingAccess = LongPathCommon.GetUnderlyingAccess(access);
			SafeFileHandle safeFileHandle = NativeMethods.CreateFile(normalizedPath, underlyingAccess, (uint)share, IntPtr.Zero, (uint)mode, (uint)options, IntPtr.Zero);
			if (safeFileHandle.IsInvalid)
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error("path");
			}
			return safeFileHandle;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000032EC File Offset: 0x000014EC
		private static SafeFileHandle OpenHandle(string normalizedPath)
		{
			return LongPathFile.GetFileHandle(normalizedPath, FileMode.Open, FileAccess.Write, FileShare.None, FileOptions.None);
		}
	}
}
