using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Tools.IO.Interop;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Tools.IO
{
	// Token: 0x02000003 RID: 3
	public static class LongPathDirectory
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002528 File Offset: 0x00000728
		public static void Create(string path)
		{
			LongPathDirectory.InternalCreateDirectory(LongPathCommon.NormalizeLongPath(path), path);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002536 File Offset: 0x00000736
		public static void Delete(string path)
		{
			LongPathDirectory.InternalDelete(LongPathCommon.NormalizeLongPath(path), path, false);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002545 File Offset: 0x00000745
		public static void Delete(string path, bool recursive)
		{
			LongPathDirectory.InternalDelete(LongPathCommon.NormalizeLongPath(path), path, recursive);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002554 File Offset: 0x00000754
		public static bool Exists(string path)
		{
			bool flag;
			return LongPathCommon.Exists(path, out flag) && flag;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000256E File Offset: 0x0000076E
		public static IEnumerable<string> EnumerateDirectories(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return LongPathDirectory.InternalEnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000258A File Offset: 0x0000078A
		public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return LongPathDirectory.InternalEnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000025B0 File Offset: 0x000007B0
		public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", "Enum value was out of legal range");
			}
			return LongPathDirectory.InternalEnumerateDirectories(path, searchPattern, searchOption);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000025ED File Offset: 0x000007ED
		private static IEnumerable<string> InternalEnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			return LongPathDirectory.EnumerateFileSystemNames(path, searchPattern, searchOption, false, true);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000025F9 File Offset: 0x000007F9
		public static IEnumerable<string> EnumerateFiles(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return LongPathDirectory.InternalEnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002615 File Offset: 0x00000815
		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return LongPathDirectory.InternalEnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000263B File Offset: 0x0000083B
		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", "Enum value was out of legal range");
			}
			return LongPathDirectory.InternalEnumerateFiles(path, searchPattern, searchOption);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002678 File Offset: 0x00000878
		private static IEnumerable<string> InternalEnumerateFiles(string path, string searchPattern, SearchOption searchOption)
		{
			return LongPathDirectory.EnumerateFileSystemNames(path, searchPattern, searchOption, true, false);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002684 File Offset: 0x00000884
		public static IEnumerable<string> EnumerateFileSystemEntries(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return LongPathDirectory.InternalEnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000026A0 File Offset: 0x000008A0
		public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return LongPathDirectory.InternalEnumerateFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000026C6 File Offset: 0x000008C6
		public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", "Enum value was out of legal range");
			}
			return LongPathDirectory.InternalEnumerateFileSystemEntries(path, searchPattern, searchOption);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002703 File Offset: 0x00000903
		private static IEnumerable<string> InternalEnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
		{
			return LongPathDirectory.EnumerateFileSystemNames(path, searchPattern, searchOption, true, true);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x0000270F File Offset: 0x0000090F
		private static IEnumerable<string> EnumerateFileSystemNames(string path, string searchPattern, SearchOption searchOption, bool includeFiles, bool includeDirs)
		{
			return LongPathDirectory.EnumerateFileSystemEntries(path, searchPattern, includeFiles, includeDirs, searchOption);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000271C File Offset: 0x0000091C
		public static DateTime GetCreationTime(string path)
		{
			return LongPathFile.GetCreationTime(path);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002724 File Offset: 0x00000924
		public static void SetCreationTime(string path, DateTime creationTime)
		{
			LongPathDirectory.SetCreationTimeUtc(path, creationTime.ToUniversalTime());
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002733 File Offset: 0x00000933
		public static DateTime GetCreationTimeUtc(string path)
		{
			return LongPathFile.GetCreationTimeUtc(path);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000273C File Offset: 0x0000093C
		public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
		{
			using (SafeFileHandle safeFileHandle = LongPathDirectory.OpenHandle(path))
			{
				LongPathCommon.SetFileTimes(safeFileHandle, creationTimeUtc.ToFileTimeUtc(), 0L, 0L);
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002780 File Offset: 0x00000980
		public static DateTime GetLastAccessTime(string path)
		{
			return LongPathFile.GetLastAccessTime(path);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002788 File Offset: 0x00000988
		public static void SetLastAccessTime(string path, DateTime lastAccessTime)
		{
			LongPathDirectory.SetLastAccessTimeUtc(path, lastAccessTime.ToUniversalTime());
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002797 File Offset: 0x00000997
		public static DateTime GetLastAccessTimeUtc(string path)
		{
			return LongPathFile.GetLastAccessTimeUtc(path);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000027A0 File Offset: 0x000009A0
		public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
		{
			using (SafeFileHandle safeFileHandle = LongPathDirectory.OpenHandle(path))
			{
				LongPathCommon.SetFileTimes(safeFileHandle, 0L, lastAccessTimeUtc.ToFileTimeUtc(), 0L);
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000027E4 File Offset: 0x000009E4
		public static DateTime GetLastWriteTime(string path)
		{
			return LongPathFile.GetLastWriteTime(path);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000027EC File Offset: 0x000009EC
		public static void SetLastWriteTime(string path, DateTime lastWriteTime)
		{
			LongPathDirectory.SetLastWriteTimeUtc(path, lastWriteTime.ToUniversalTime());
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000027FB File Offset: 0x000009FB
		public static DateTime GetLastWriteTimeUtc(string path)
		{
			return LongPathFile.GetLastWriteTimeUtc(path);
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002804 File Offset: 0x00000A04
		public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
		{
			using (SafeFileHandle safeFileHandle = LongPathDirectory.OpenHandle(path))
			{
				LongPathCommon.SetFileTimes(safeFileHandle, 0L, 0L, lastWriteTimeUtc.ToFileTimeUtc());
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002848 File Offset: 0x00000A48
		public static FileAttributes GetAttributes(string path)
		{
			return LongPathCommon.GetAttributes(path);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002850 File Offset: 0x00000A50
		public static void SetAttributes(string path, FileAttributes directoryAttributes)
		{
			LongPathCommon.SetAttributes(path, directoryAttributes);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000285C File Offset: 0x00000A5C
		private static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, bool includeFiles, bool includeDirectories, SearchOption searchOption)
		{
			string normalizedSearchPattern = LongPathDirectory.NormalizeSearchPatternForIterator(searchPattern);
			string normalizedPath = LongPathCommon.NormalizeLongPath(path);
			FileAttributes fileAttributes;
			int num = LongPathCommon.TryGetDirectoryAttributes(normalizedPath, out fileAttributes);
			if (num != 0)
			{
				throw LongPathCommon.GetExceptionFromWin32Error(num, "path");
			}
			return LongPathDirectory.EnumerateFileSystemIterator(normalizedPath, normalizedSearchPattern, includeFiles, includeDirectories, searchOption);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002898 File Offset: 0x00000A98
		private static string NormalizeSearchPatternForIterator(string searchPattern)
		{
			string text = searchPattern.TrimEnd(LongPathPath.TrimEndChars);
			if (text.Equals("."))
			{
				text = "*";
			}
			LongPathDirectory.CheckSearchPattern(text);
			return text;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000028CC File Offset: 0x00000ACC
		[SecuritySafeCritical]
		internal static void CheckSearchPattern(string searchPattern)
		{
			int num;
			while ((num = searchPattern.IndexOf("..", StringComparison.Ordinal)) != -1)
			{
				if (num + 2 == searchPattern.Length)
				{
					throw new ArgumentException("Search pattern cannot contain '..' to move up directories and can be contained only internally in file/directory names, as in 'a..b'.");
				}
				if (searchPattern[num + 2] == LongPathPath.DirectorySeparatorChar || searchPattern[num + 2] == LongPathPath.AltDirectorySeparatorChar)
				{
					throw new ArgumentException("Search pattern cannot contain '..' to move up directories and can be contained only internally in file/directory names, as in 'a..b'.");
				}
				searchPattern = searchPattern.Substring(num + 2);
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002938 File Offset: 0x00000B38
		private static IEnumerable<string> EnumerateFileSystemIterator(string normalizedPath, string normalizedSearchPattern, bool includeFiles, bool includeDirectories, SearchOption searchOption)
		{
			if (normalizedSearchPattern.Length == 0)
			{
				yield break;
			}
			Queue<string> directoryQueue = new Queue<string>();
			directoryQueue.Enqueue(normalizedPath);
			while (directoryQueue.Count > 0)
			{
				normalizedPath = directoryQueue.Dequeue();
				string path = LongPathCommon.RemoveLongPathPrefix(normalizedPath);
				if (searchOption == SearchOption.AllDirectories)
				{
					foreach (string path2 in LongPathDirectory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly))
					{
						directoryQueue.Enqueue(LongPathCommon.NormalizeLongPath(path2));
					}
				}
				NativeMethods.WIN32_FIND_DATA win32_FIND_DATA;
				using (SafeFindHandle handle = LongPathDirectory.BeginFind(Path.Combine(normalizedPath, normalizedSearchPattern), out win32_FIND_DATA))
				{
					if (handle == null)
					{
						if (searchOption == SearchOption.TopDirectoryOnly)
						{
							yield break;
						}
						continue;
					}
					else
					{
						do
						{
							string currentFileName = win32_FIND_DATA.cFileName;
							if (LongPathDirectory.IsDirectory(win32_FIND_DATA.dwFileAttributes))
							{
								if (!LongPathDirectory.IsCurrentOrParentDirectory(currentFileName))
								{
									if (searchOption == SearchOption.AllDirectories)
									{
										string item = Path.Combine(normalizedPath, currentFileName);
										if (!directoryQueue.Contains(item))
										{
											directoryQueue.Enqueue(item);
										}
									}
									if (includeDirectories)
									{
										yield return Path.Combine(path, currentFileName);
									}
								}
							}
							else if (includeFiles)
							{
								yield return Path.Combine(path, currentFileName);
							}
							currentFileName = null;
						}
						while (NativeMethods.FindNextFile(handle, out win32_FIND_DATA));
						int lastWin32Error = Marshal.GetLastWin32Error();
						if (lastWin32Error != 18)
						{
							throw LongPathCommon.GetExceptionFromWin32Error(lastWin32Error, "path");
						}
					}
				}
				SafeFindHandle handle = null;
				path = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002968 File Offset: 0x00000B68
		private static SafeFindHandle BeginFind(string normalizedPathWithSearchPattern, out NativeMethods.WIN32_FIND_DATA findData)
		{
			SafeFindHandle safeFindHandle = NativeMethods.FindFirstFile(normalizedPathWithSearchPattern, out findData);
			if (!safeFindHandle.IsInvalid)
			{
				return safeFindHandle;
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 2)
			{
				throw LongPathCommon.GetExceptionFromWin32Error(lastWin32Error, "path");
			}
			return null;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000299E File Offset: 0x00000B9E
		internal static bool IsDirectory(FileAttributes attributes)
		{
			return (attributes & FileAttributes.Directory) == FileAttributes.Directory;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000029A8 File Offset: 0x00000BA8
		internal static void InternalCreateDirectory(string normalizedPath, string path)
		{
			int num = normalizedPath.Length;
			if (num >= 2 && LongPathPath.IsDirectorySeparator(normalizedPath[num - 1]))
			{
				num--;
			}
			int rootLength = LongPathPath.GetRootLength(normalizedPath);
			if (num == 2 && LongPathPath.IsDirectorySeparator(normalizedPath[1]))
			{
				throw new IOException(string.Format("The specified directory '{0}' cannot be created.", path));
			}
			List<string> list = new List<string>();
			bool flag = false;
			if (num > rootLength)
			{
				int num2 = num - 1;
				while (num2 >= rootLength && !flag)
				{
					string text = normalizedPath.Substring(0, num2 + 1);
					if (!LongPathDirectory.InternalExists(text))
					{
						list.Add(text);
					}
					else
					{
						flag = true;
					}
					while (num2 > rootLength && normalizedPath[num2] != Path.DirectorySeparatorChar && normalizedPath[num2] != Path.AltDirectorySeparatorChar)
					{
						num2--;
					}
					num2--;
				}
			}
			int count = list.Count;
			if (list.Count != 0)
			{
				string[] array = new string[list.Count];
				list.CopyTo(array, 0);
				for (int i = 0; i < array.Length; i++)
				{
					ref string ptr = ref array[i];
					ptr += "\\.";
				}
			}
			bool flag2 = true;
			int num3 = 0;
			while (list.Count > 0)
			{
				List<string> list2 = list;
				string text2 = list2[list2.Count - 1];
				List<string> list3 = list;
				list3.RemoveAt(list3.Count - 1);
				if (text2.Length >= 32000)
				{
					throw new PathTooLongException("The specified file name or path is too long, or a component of the specified path is too long.");
				}
				flag2 = NativeMethods.CreateDirectory(text2, IntPtr.Zero);
				if (!flag2 && num3 == 0)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error != 183)
					{
						num3 = lastWin32Error;
					}
					else if (LongPathFile.InternalExists(text2) || (!LongPathDirectory.InternalExists(text2, out lastWin32Error) && lastWin32Error == 5))
					{
						num3 = lastWin32Error;
					}
				}
			}
			if (count == 0 && !flag)
			{
				if (!LongPathDirectory.InternalExists(LongPathDirectory.InternalGetDirectoryRoot(normalizedPath)))
				{
					throw new DirectoryNotFoundException(string.Format("Could not find a part of the path '{0}'", LongPathDirectory.InternalGetDirectoryRoot(path)));
				}
				return;
			}
			else
			{
				if (!flag2 && num3 != 0)
				{
					throw LongPathCommon.GetExceptionFromWin32Error(num3, "path");
				}
				return;
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002B98 File Offset: 0x00000D98
		internal static bool InternalExists(string normalizedPath)
		{
			int num;
			return LongPathDirectory.InternalExists(normalizedPath, out num);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002BB0 File Offset: 0x00000DB0
		internal static bool InternalExists(string normalizedPath, out int lastError)
		{
			NativeMethods.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(NativeMethods.WIN32_FILE_ATTRIBUTE_DATA);
			lastError = LongPathCommon.FillAttributeInfo(normalizedPath, ref win32_FILE_ATTRIBUTE_DATA, false, false);
			return lastError == 0 && win32_FILE_ATTRIBUTE_DATA.fileAttributes != -1 && (win32_FILE_ATTRIBUTE_DATA.fileAttributes & 16) != 0;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002BEC File Offset: 0x00000DEC
		internal static string InternalGetDirectoryRoot(string path)
		{
			if (path == null)
			{
				return null;
			}
			return path.Substring(0, LongPathPath.GetRootLength(path));
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002C00 File Offset: 0x00000E00
		internal static void InternalDelete(string normalizedPath, string userPath, bool recursive)
		{
			NativeMethods.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(NativeMethods.WIN32_FILE_ATTRIBUTE_DATA);
			LongPathCommon.FillAttributeInfo(normalizedPath, ref win32_FILE_ATTRIBUTE_DATA, false, true);
			if ((win32_FILE_ATTRIBUTE_DATA.fileAttributes & 1024) != 0)
			{
				recursive = false;
			}
			LongPathDirectory.DeleteHelper(normalizedPath, userPath, recursive);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002C3C File Offset: 0x00000E3C
		private static void DeleteHelper(string fullPath, string userPath, bool recursive)
		{
			Exception ex = null;
			int num;
			if (recursive)
			{
				NativeMethods.WIN32_FIND_DATA win32_FIND_DATA = default(NativeMethods.WIN32_FIND_DATA);
				using (SafeFindHandle safeFindHandle = NativeMethods.FindFirstFile(fullPath + Path.DirectorySeparatorChar.ToString() + "*", out win32_FIND_DATA))
				{
					if (safeFindHandle.IsInvalid)
					{
						throw LongPathCommon.GetExceptionFromLastWin32Error("path");
					}
					for (;;)
					{
						if ((win32_FIND_DATA.dwFileAttributes & FileAttributes.Directory) <= (FileAttributes)0)
						{
							goto IL_152;
						}
						if (!win32_FIND_DATA.cFileName.Equals(".") && !win32_FIND_DATA.cFileName.Equals(".."))
						{
							if ((win32_FIND_DATA.dwFileAttributes & FileAttributes.ReparsePoint) == (FileAttributes)0)
							{
								string fullPath2 = Path.Combine(fullPath, win32_FIND_DATA.cFileName);
								string userPath2 = Path.Combine(userPath, win32_FIND_DATA.cFileName);
								try
								{
									LongPathDirectory.DeleteHelper(fullPath2, userPath2, recursive);
									goto IL_185;
								}
								catch (Exception ex2)
								{
									if (ex == null)
									{
										ex = ex2;
									}
									goto IL_185;
								}
							}
							if (win32_FIND_DATA.dwReserved0 == -1610612733 && !NativeMethods.DeleteVolumeMountPoint(Path.Combine(fullPath, win32_FIND_DATA.cFileName + Path.DirectorySeparatorChar.ToString())))
							{
								num = Marshal.GetLastWin32Error();
								try
								{
									throw LongPathCommon.GetExceptionFromWin32Error(num, "path");
								}
								catch (Exception ex3)
								{
									if (ex == null)
									{
										ex = ex3;
									}
								}
							}
							if (!NativeMethods.RemoveDirectory(Path.Combine(fullPath, win32_FIND_DATA.cFileName)))
							{
								num = Marshal.GetLastWin32Error();
								try
								{
									throw LongPathCommon.GetExceptionFromWin32Error(num, "path");
								}
								catch (Exception ex4)
								{
									if (ex == null)
									{
										ex = ex4;
									}
									goto IL_185;
								}
								goto IL_152;
							}
						}
						IL_185:
						if (!NativeMethods.FindNextFile(safeFindHandle, out win32_FIND_DATA))
						{
							break;
						}
						continue;
						IL_152:
						if (NativeMethods.DeleteFile(Path.Combine(fullPath, win32_FIND_DATA.cFileName)))
						{
							goto IL_185;
						}
						num = Marshal.GetLastWin32Error();
						if (num != 2)
						{
							try
							{
								throw LongPathCommon.GetExceptionFromWin32Error(num, "path");
							}
							catch (Exception ex5)
							{
								if (ex == null)
								{
									ex = ex5;
								}
							}
							goto IL_185;
						}
						goto IL_185;
					}
					num = Marshal.GetLastWin32Error();
				}
				if (ex != null)
				{
					throw ex;
				}
				if (num != 0 && num != 18)
				{
					throw LongPathCommon.GetExceptionFromWin32Error(num, "path");
				}
			}
			if (NativeMethods.RemoveDirectory(fullPath))
			{
				return;
			}
			num = Marshal.GetLastWin32Error();
			if (num == 2)
			{
				num = 3;
			}
			if (num == 5)
			{
				throw new IOException(string.Format("Access to the path '{0}' is denied.", userPath));
			}
			throw LongPathCommon.GetExceptionFromWin32Error(num, "path");
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002EB8 File Offset: 0x000010B8
		private static SafeFileHandle OpenHandle(string path)
		{
			string text = LongPathCommon.NormalizeLongPath(path);
			string pathRoot = LongPathPath.GetPathRoot(text);
			if (pathRoot == text && pathRoot[1] == Path.VolumeSeparatorChar)
			{
				throw new ArgumentException("Path must not be a drive.", "path");
			}
			SafeFileHandle safeFileHandle = NativeMethods.SafeCreateFile(text, NativeMethods.EFileAccess.GenericWrite, 6U, IntPtr.Zero, 3U, 33554432U, IntPtr.Zero);
			if (safeFileHandle.IsInvalid)
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error("path");
			}
			return safeFileHandle;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002F29 File Offset: 0x00001129
		private static bool IsCurrentOrParentDirectory(string directoryName)
		{
			return directoryName.Equals(".", StringComparison.OrdinalIgnoreCase) || directoryName.Equals("..", StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x04000001 RID: 1
		private const int FILE_FLAG_BACKUP_SEMANTICS = 33554432;
	}
}
