using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Tools.IO.Interop;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Tools.IO
{
	// Token: 0x02000002 RID: 2
	internal static class LongPathCommon
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		internal static string NormalizeSearchPattern(string searchPattern)
		{
			if (string.IsNullOrEmpty(searchPattern) || searchPattern == ".")
			{
				return "*";
			}
			return searchPattern;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000206E File Offset: 0x0000026E
		internal static string NormalizeLongPath(string path)
		{
			return LongPathCommon.NormalizeLongPath(path, "path");
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000207C File Offset: 0x0000027C
		internal static string NormalizeLongPath(string path, string parameterName)
		{
			if (path == null)
			{
				throw new ArgumentNullException(parameterName);
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "'{0}' cannot be an empty string.", new object[]
				{
					parameterName
				}), parameterName);
			}
			StringBuilder stringBuilder = new StringBuilder(path.Length + 1);
			uint fullPathName = NativeMethods.GetFullPathName(path, (uint)stringBuilder.Capacity, stringBuilder, IntPtr.Zero);
			if ((ulong)fullPathName > (ulong)((long)stringBuilder.Capacity))
			{
				stringBuilder.Capacity = (int)fullPathName;
				fullPathName = NativeMethods.GetFullPathName(path, fullPathName, stringBuilder, IntPtr.Zero);
			}
			if (fullPathName == 0U)
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error(parameterName);
			}
			if (fullPathName > 32000U)
			{
				throw LongPathCommon.GetExceptionFromWin32Error(206, parameterName);
			}
			return LongPathCommon.AddLongPathPrefix(stringBuilder.ToString());
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002128 File Offset: 0x00000328
		private static bool TryNormalizeLongPath(string path, out string result)
		{
			try
			{
				result = LongPathCommon.NormalizeLongPath(path);
				return true;
			}
			catch (ArgumentException)
			{
			}
			catch (PathTooLongException)
			{
			}
			result = null;
			return false;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000216C File Offset: 0x0000036C
		private static string AddLongPathPrefix(string path)
		{
			if (!path.StartsWith("\\\\"))
			{
				return "\\\\?\\" + path;
			}
			return "\\\\?\\UNC\\" + path.Substring(2);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002198 File Offset: 0x00000398
		internal static string RemoveLongPathPrefix(string normalizedPath)
		{
			if (!normalizedPath.StartsWith("\\\\?\\UNC\\"))
			{
				return normalizedPath.Substring("\\\\?\\".Length);
			}
			return "\\\\" + normalizedPath.Substring("\\\\?\\UNC\\".Length);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021D4 File Offset: 0x000003D4
		internal static bool Exists(string path, out bool isDirectory)
		{
			string normalizedPath;
			FileAttributes attributes;
			if (LongPathCommon.TryNormalizeLongPath(path, out normalizedPath) && LongPathCommon.TryGetFileAttributes(normalizedPath, out attributes) == 0)
			{
				isDirectory = LongPathDirectory.IsDirectory(attributes);
				return true;
			}
			isDirectory = false;
			return false;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002204 File Offset: 0x00000404
		internal static int TryGetDirectoryAttributes(string normalizedPath, out FileAttributes attributes)
		{
			int result = LongPathCommon.TryGetFileAttributes(normalizedPath, out attributes);
			if (!LongPathDirectory.IsDirectory(attributes))
			{
				result = 267;
			}
			return result;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002229 File Offset: 0x00000429
		internal static int TryGetFileAttributes(string normalizedPath, out FileAttributes attributes)
		{
			attributes = NativeMethods.GetFileAttributes(normalizedPath);
			if (attributes == (FileAttributes)(-1))
			{
				return Marshal.GetLastWin32Error();
			}
			return 0;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000223F File Offset: 0x0000043F
		internal static FileAttributes GetAttributes(string path)
		{
			return (FileAttributes)LongPathCommon.GetWin32FileAttributeData(LongPathCommon.NormalizeLongPath(path)).fileAttributes;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002254 File Offset: 0x00000454
		internal static void SetAttributes(string path, FileAttributes attributes)
		{
			if (NativeMethods.SetFileAttributes(LongPathCommon.NormalizeLongPath(path), (int)attributes))
			{
				return;
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == 5)
			{
				throw new ArgumentException("Access to the path is denied.");
			}
			if (lastWin32Error == 87)
			{
				throw new ArgumentException("Invalid File or Directory attributes value.");
			}
			throw LongPathCommon.GetExceptionFromWin32Error(lastWin32Error, "path");
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000022A0 File Offset: 0x000004A0
		internal static void SetFileTimes(SafeFileHandle hFile, long creationTime, long accessTime, long writeTime)
		{
			if (!NativeMethods.SetFileTime(hFile, ref creationTime, ref accessTime, ref writeTime))
			{
				throw LongPathCommon.GetExceptionFromWin32Error(Marshal.GetLastWin32Error(), "path");
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000022C0 File Offset: 0x000004C0
		internal static NativeMethods.WIN32_FILE_ATTRIBUTE_DATA GetWin32FileAttributeData(string normalizedPath)
		{
			NativeMethods.WIN32_FILE_ATTRIBUTE_DATA result = default(NativeMethods.WIN32_FILE_ATTRIBUTE_DATA);
			int num = LongPathCommon.FillAttributeInfo(normalizedPath, ref result, false, false);
			if (num != 0)
			{
				throw LongPathCommon.GetExceptionFromWin32Error(num, "path");
			}
			return result;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000022F0 File Offset: 0x000004F0
		internal static int FillAttributeInfo(string normalizedLongPath, ref NativeMethods.WIN32_FILE_ATTRIBUTE_DATA data, bool tryagain, bool returnErrorOnNotFound)
		{
			int num = 0;
			if (tryagain)
			{
				string lpFileName = normalizedLongPath.TrimEnd(new char[]
				{
					Path.DirectorySeparatorChar,
					Path.AltDirectorySeparatorChar
				});
				int errorMode = NativeMethods.SetErrorMode(1);
				NativeMethods.WIN32_FIND_DATA findData;
				try
				{
					bool flag = false;
					SafeFindHandle safeFindHandle = NativeMethods.FindFirstFile(lpFileName, out findData);
					try
					{
						if (safeFindHandle.IsInvalid)
						{
							flag = true;
							num = Marshal.GetLastWin32Error();
							if ((num == 2 || num == 3 || num == 21) && !returnErrorOnNotFound)
							{
								num = 0;
								data.fileAttributes = -1;
							}
							return num;
						}
					}
					finally
					{
						try
						{
							safeFindHandle.Close();
						}
						catch
						{
							if (!flag)
							{
								throw LongPathCommon.GetExceptionFromLastWin32Error("handle");
							}
						}
					}
				}
				finally
				{
					NativeMethods.SetErrorMode(errorMode);
				}
				data.PopulateFrom(findData);
				return num;
			}
			int errorMode2 = NativeMethods.SetErrorMode(1);
			bool fileAttributesEx;
			try
			{
				fileAttributesEx = NativeMethods.GetFileAttributesEx(normalizedLongPath, 0, ref data);
			}
			finally
			{
				NativeMethods.SetErrorMode(errorMode2);
			}
			if (!fileAttributesEx)
			{
				num = Marshal.GetLastWin32Error();
				if (num != 2 && num != 3 && num != 21)
				{
					return LongPathCommon.FillAttributeInfo(normalizedLongPath, ref data, true, returnErrorOnNotFound);
				}
				if (!returnErrorOnNotFound)
				{
					num = 0;
					data.fileAttributes = -1;
				}
			}
			return num;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002418 File Offset: 0x00000618
		internal static NativeMethods.EFileAccess GetUnderlyingAccess(FileAccess access)
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

		// Token: 0x06000010 RID: 16 RVA: 0x0000244C File Offset: 0x0000064C
		internal static Exception GetExceptionFromLastWin32Error(string parameterName = "path")
		{
			return LongPathCommon.GetExceptionFromWin32Error(Marshal.GetLastWin32Error(), parameterName);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000245C File Offset: 0x0000065C
		internal static Exception GetExceptionFromWin32Error(int errorCode, string parameterName = "path")
		{
			string messageFromErrorCode = LongPathCommon.GetMessageFromErrorCode(errorCode);
			if (errorCode <= 15)
			{
				switch (errorCode)
				{
				case 2:
					return new FileNotFoundException(messageFromErrorCode);
				case 3:
					return new DirectoryNotFoundException(messageFromErrorCode);
				case 4:
					break;
				case 5:
					return new UnauthorizedAccessException(messageFromErrorCode);
				default:
					if (errorCode == 15)
					{
						return new DriveNotFoundException(messageFromErrorCode);
					}
					break;
				}
			}
			else
			{
				if (errorCode == 123)
				{
					return new ArgumentException(messageFromErrorCode, parameterName);
				}
				if (errorCode == 206)
				{
					return new PathTooLongException(messageFromErrorCode);
				}
				if (errorCode == 995)
				{
					return new OperationCanceledException(messageFromErrorCode);
				}
			}
			return new IOException(messageFromErrorCode, NativeMethods.MakeHRFromErrorCode(errorCode));
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000024EC File Offset: 0x000006EC
		private static string GetMessageFromErrorCode(int errorCode)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			int dwFlags = 12800;
			IntPtr zero = IntPtr.Zero;
			int dwLanguageId = 0;
			StringBuilder stringBuilder2 = stringBuilder;
			NativeMethods.FormatMessage(dwFlags, zero, errorCode, dwLanguageId, stringBuilder2, stringBuilder2.Capacity, IntPtr.Zero);
			return stringBuilder.ToString();
		}
	}
}
