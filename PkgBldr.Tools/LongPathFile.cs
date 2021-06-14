using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000013 RID: 19
	public static class LongPathFile
	{
		// Token: 0x0600009E RID: 158 RVA: 0x00003C3C File Offset: 0x00001E3C
		public static bool Exists(string path)
		{
			return NativeMethods.IU_FileExists(path);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00003C44 File Offset: 0x00001E44
		public static FileAttributes GetAttributes(string path)
		{
			return LongPathCommon.GetAttributes(path);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00003C4C File Offset: 0x00001E4C
		public static void SetAttributes(string path, FileAttributes attributes)
		{
			if (!NativeMethods.SetFileAttributes(LongPathCommon.NormalizeLongPath(path), attributes))
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error();
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00003C64 File Offset: 0x00001E64
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

		// Token: 0x060000A2 RID: 162 RVA: 0x00003C9C File Offset: 0x00001E9C
		public static void Move(string sourcePath, string destinationPath)
		{
			string lpPathNameFrom = LongPathCommon.NormalizeLongPath(sourcePath);
			string lpPathNameTo = LongPathCommon.NormalizeLongPath(destinationPath);
			if (!NativeMethods.MoveFile(lpPathNameFrom, lpPathNameTo))
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error();
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00003CC4 File Offset: 0x00001EC4
		public static void Copy(string sourcePath, string destinationPath, bool overwrite)
		{
			string src = LongPathCommon.NormalizeLongPath(sourcePath);
			string dst = LongPathCommon.NormalizeLongPath(destinationPath);
			if (!NativeMethods.CopyFile(src, dst, !overwrite))
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error();
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00003CF0 File Offset: 0x00001EF0
		public static void Copy(string sourcePath, string destinationPath)
		{
			LongPathFile.Copy(sourcePath, destinationPath, false);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00003CFA File Offset: 0x00001EFA
		public static FileStream Open(string path, FileMode mode, FileAccess access)
		{
			return LongPathFile.Open(path, mode, access, FileShare.None, 0, FileOptions.None);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00003D07 File Offset: 0x00001F07
		public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
		{
			return LongPathFile.Open(path, mode, access, share, 0, FileOptions.None);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00003D14 File Offset: 0x00001F14
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

		// Token: 0x060000A8 RID: 168 RVA: 0x00003D63 File Offset: 0x00001F63
		public static FileStream OpenRead(string path)
		{
			return LongPathFile.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00003D6E File Offset: 0x00001F6E
		public static FileStream OpenWrite(string path)
		{
			return LongPathFile.Open(path, FileMode.Create, FileAccess.ReadWrite);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00003D78 File Offset: 0x00001F78
		public static StreamWriter CreateText(string path)
		{
			return new StreamWriter(LongPathFile.Open(path, FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00003D8C File Offset: 0x00001F8C
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

		// Token: 0x060000AC RID: 172 RVA: 0x00003DD8 File Offset: 0x00001FD8
		public static void WriteAllBytes(string path, byte[] contents)
		{
			using (FileStream fileStream = LongPathFile.OpenWrite(path))
			{
				fileStream.Write(contents, 0, contents.Length);
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00003E14 File Offset: 0x00002014
		public static string ReadAllText(string path, Encoding encoding)
		{
			string result;
			using (StreamReader streamReader = new StreamReader(LongPathFile.OpenRead(path), encoding, true))
			{
				result = streamReader.ReadToEnd();
			}
			return result;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00003E54 File Offset: 0x00002054
		public static string ReadAllText(string path)
		{
			return LongPathFile.ReadAllText(path, Encoding.Default);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00003E64 File Offset: 0x00002064
		public static void WriteAllText(string path, string contents, Encoding encoding)
		{
			using (StreamWriter streamWriter = new StreamWriter(LongPathFile.OpenWrite(path), encoding))
			{
				streamWriter.Write(contents);
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00003EA4 File Offset: 0x000020A4
		public static void WriteAllText(string path, string contents)
		{
			LongPathFile.WriteAllText(path, contents, new UTF8Encoding(false));
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00003EB4 File Offset: 0x000020B4
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

		// Token: 0x060000B2 RID: 178 RVA: 0x00003F10 File Offset: 0x00002110
		public static string[] ReadAllLines(string path)
		{
			return LongPathFile.ReadAllLines(path, Encoding.Default);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00003F20 File Offset: 0x00002120
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

		// Token: 0x060000B4 RID: 180 RVA: 0x00003F8C File Offset: 0x0000218C
		public static void WriteAllLines(string path, IEnumerable<string> contents)
		{
			LongPathFile.WriteAllLines(path, contents, new UTF8Encoding(false));
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00003F9C File Offset: 0x0000219C
		public static void AppendAllText(string path, string contents, Encoding encoding)
		{
			using (StreamWriter streamWriter = new StreamWriter(LongPathFile.Open(path, FileMode.Append, FileAccess.ReadWrite), encoding))
			{
				streamWriter.Write(contents);
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00003FDC File Offset: 0x000021DC
		public static void AppendAllText(string path, string contents)
		{
			LongPathFile.AppendAllText(path, contents, new UTF8Encoding(false));
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00003FEC File Offset: 0x000021EC
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "False positive")]
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

		// Token: 0x060000B8 RID: 184 RVA: 0x0000402A File Offset: 0x0000222A
		private static FileMode GetUnderlyingMode(FileMode mode)
		{
			if (mode == FileMode.Append)
			{
				return FileMode.OpenOrCreate;
			}
			return mode;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00004033 File Offset: 0x00002233
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
