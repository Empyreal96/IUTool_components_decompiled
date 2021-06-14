using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200004B RID: 75
	public static class LongPathFile
	{
		// Token: 0x06000204 RID: 516 RVA: 0x00009FD0 File Offset: 0x000081D0
		public static bool Exists(string path)
		{
			return NativeMethods.IU_FileExists(path);
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00009FD8 File Offset: 0x000081D8
		public static FileAttributes GetAttributes(string path)
		{
			return LongPathCommon.GetAttributes(path);
		}

		// Token: 0x06000206 RID: 518 RVA: 0x00009FE0 File Offset: 0x000081E0
		public static void SetAttributes(string path, FileAttributes attributes)
		{
			if (!NativeMethods.SetFileAttributes(LongPathCommon.NormalizeLongPath(path), attributes))
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error();
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00009FF8 File Offset: 0x000081F8
		public static void Delete(string path)
		{
			string lpFileName = LongPathCommon.NormalizeLongPath(path);
			if (!LongPathFile.Exists(path))
			{
				return;
			}
			if (!NativeMethods.DeleteFile(lpFileName))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 2)
				{
					throw LongPathCommon.GetExceptionFromWin32Error(lastWin32Error);
				}
			}
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000A030 File Offset: 0x00008230
		public static void Move(string sourcePath, string destinationPath)
		{
			string lpPathNameFrom = LongPathCommon.NormalizeLongPath(sourcePath);
			string lpPathNameTo = LongPathCommon.NormalizeLongPath(destinationPath);
			if (!NativeMethods.MoveFile(lpPathNameFrom, lpPathNameTo))
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error();
			}
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000A058 File Offset: 0x00008258
		public static void Copy(string sourcePath, string destinationPath, bool overwrite)
		{
			string src = LongPathCommon.NormalizeLongPath(sourcePath);
			string dst = LongPathCommon.NormalizeLongPath(destinationPath);
			if (!NativeMethods.CopyFile(src, dst, !overwrite))
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error();
			}
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000A084 File Offset: 0x00008284
		public static void Copy(string sourcePath, string destinationPath)
		{
			LongPathFile.Copy(sourcePath, destinationPath, false);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000A08E File Offset: 0x0000828E
		public static FileStream Open(string path, FileMode mode, FileAccess access)
		{
			return LongPathFile.Open(path, mode, access, FileShare.None, 0, FileOptions.None);
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000A09B File Offset: 0x0000829B
		public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
		{
			return LongPathFile.Open(path, mode, access, share, 0, FileOptions.None);
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000A0A8 File Offset: 0x000082A8
		public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
		{
			if (bufferSize == 0)
			{
				bufferSize = 1024;
			}
			FileStream fileStream = new FileStream(LongPathFile.GetFileHandle(LongPathCommon.NormalizeLongPath(path), mode, access, share, options), access, bufferSize, (options & FileOptions.Asynchronous) == FileOptions.Asynchronous);
			if (mode == FileMode.Append)
			{
				fileStream.Seek(0L, SeekOrigin.End);
			}
			return fileStream;
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000A0F7 File Offset: 0x000082F7
		public static FileStream OpenRead(string path)
		{
			return LongPathFile.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000A102 File Offset: 0x00008302
		public static FileStream OpenWrite(string path)
		{
			return LongPathFile.Open(path, FileMode.Create, FileAccess.ReadWrite);
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000A10C File Offset: 0x0000830C
		public static StreamWriter CreateText(string path)
		{
			return new StreamWriter(LongPathFile.Open(path, FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8);
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000A120 File Offset: 0x00008320
		public static byte[] ReadAllBytes(string path)
		{
			byte[] result;
			using (FileStream fileStream = LongPathFile.OpenRead(path))
			{
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				result = array;
			}
			return result;
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000A16C File Offset: 0x0000836C
		public static void WriteAllBytes(string path, byte[] contents)
		{
			using (FileStream fileStream = LongPathFile.OpenWrite(path))
			{
				fileStream.Write(contents, 0, contents.Length);
			}
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000A1A8 File Offset: 0x000083A8
		public static string ReadAllText(string path, Encoding encoding)
		{
			string result;
			using (StreamReader streamReader = new StreamReader(LongPathFile.OpenRead(path), encoding, true))
			{
				result = streamReader.ReadToEnd();
			}
			return result;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000A1E8 File Offset: 0x000083E8
		public static string ReadAllText(string path)
		{
			return LongPathFile.ReadAllText(path, Encoding.Default);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000A1F8 File Offset: 0x000083F8
		public static void WriteAllText(string path, string contents, Encoding encoding)
		{
			using (StreamWriter streamWriter = new StreamWriter(LongPathFile.OpenWrite(path), encoding))
			{
				streamWriter.Write(contents);
			}
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000A238 File Offset: 0x00008438
		public static void WriteAllText(string path, string contents)
		{
			LongPathFile.WriteAllText(path, contents, new UTF8Encoding(false));
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000A248 File Offset: 0x00008448
		public static string[] ReadAllLines(string path, Encoding encoding)
		{
			string[] result;
			using (StreamReader streamReader = new StreamReader(LongPathFile.OpenRead(path), encoding, true))
			{
				List<string> list = new List<string>();
				while (!streamReader.EndOfStream)
				{
					list.Add(streamReader.ReadLine());
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000A2A4 File Offset: 0x000084A4
		public static string[] ReadAllLines(string path)
		{
			return LongPathFile.ReadAllLines(path, Encoding.Default);
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000A2B4 File Offset: 0x000084B4
		public static void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
		{
			using (StreamWriter streamWriter = new StreamWriter(LongPathFile.OpenWrite(path), encoding))
			{
				foreach (string value in contents)
				{
					streamWriter.WriteLine(value);
				}
			}
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000A320 File Offset: 0x00008520
		public static void WriteAllLines(string path, IEnumerable<string> contents)
		{
			LongPathFile.WriteAllLines(path, contents, new UTF8Encoding(false));
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000A330 File Offset: 0x00008530
		public static void AppendAllText(string path, string contents, Encoding encoding)
		{
			using (StreamWriter streamWriter = new StreamWriter(LongPathFile.Open(path, FileMode.Append, FileAccess.ReadWrite), encoding))
			{
				streamWriter.Write(contents);
			}
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000A370 File Offset: 0x00008570
		public static void AppendAllText(string path, string contents)
		{
			LongPathFile.AppendAllText(path, contents, new UTF8Encoding(false));
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000A380 File Offset: 0x00008580
		private static SafeFileHandle GetFileHandle(string normalizedPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
		{
			NativeMethods.EFileAccess underlyingAccess = LongPathFile.GetUnderlyingAccess(access);
			FileMode underlyingMode = LongPathFile.GetUnderlyingMode(mode);
			SafeFileHandle safeFileHandle = NativeMethods.CreateFile(normalizedPath, underlyingAccess, (uint)share, IntPtr.Zero, (uint)underlyingMode, (uint)options, IntPtr.Zero);
			if (safeFileHandle.IsInvalid)
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error();
			}
			return safeFileHandle;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000A3BE File Offset: 0x000085BE
		private static FileMode GetUnderlyingMode(FileMode mode)
		{
			if (mode == FileMode.Append)
			{
				return FileMode.OpenOrCreate;
			}
			return mode;
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000A3C7 File Offset: 0x000085C7
		private static NativeMethods.EFileAccess GetUnderlyingAccess(FileAccess access)
		{
			switch (access)
			{
			case FileAccess.Read:
				return (NativeMethods.EFileAccess)2147483648U;
			case FileAccess.Write:
				return NativeMethods.EFileAccess.GenericWrite;
			case FileAccess.ReadWrite:
				return (NativeMethods.EFileAccess)3221225472U;
			default:
				throw new ArgumentOutOfRangeException("access");
			}
		}
	}
}
