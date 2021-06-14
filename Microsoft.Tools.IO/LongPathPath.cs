using System;
using System.IO;

namespace Microsoft.Tools.IO
{
	// Token: 0x02000005 RID: 5
	public static class LongPathPath
	{
		// Token: 0x06000059 RID: 89 RVA: 0x000032F8 File Offset: 0x000014F8
		public static string GetFullPath(string path)
		{
			path = LongPathPath.NormalizePath(path, true);
			path = LongPathCommon.NormalizeLongPath(path);
			return LongPathCommon.RemoveLongPathPrefix(path);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003311 File Offset: 0x00001511
		public static string GetPathRoot(string path)
		{
			if (path == null)
			{
				return null;
			}
			path = LongPathPath.NormalizePath(path, false);
			if (path == null)
			{
				return null;
			}
			return path.Substring(0, LongPathPath.GetRootLength(path));
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003334 File Offset: 0x00001534
		public static string GetDirectoryName(string path)
		{
			if (path != null)
			{
				LongPathPath.CheckInvalidPathChars(path);
				path = LongPathPath.NormalizePath(path, false);
				int rootLength = LongPathPath.GetRootLength(path);
				int num = path.Length;
				if (num > rootLength)
				{
					num = path.Length;
					if (num == rootLength)
					{
						return null;
					}
					while (num > rootLength && path[--num] != LongPathPath.DirectorySeparatorChar && path[num] != LongPathPath.AltDirectorySeparatorChar)
					{
					}
					string text = path.Substring(0, num);
					if (num > rootLength)
					{
						return text.TrimEnd(new char[]
						{
							LongPathPath.DirectorySeparatorChar,
							LongPathPath.AltDirectorySeparatorChar
						});
					}
					return text;
				}
			}
			return null;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000033C3 File Offset: 0x000015C3
		public static string Combine(string path1, string path2)
		{
			return Path.Combine(path1, path2);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000033CC File Offset: 0x000015CC
		public static string Combine(string path1, string path2, string path3)
		{
			return Path.Combine(path1, path2, path3);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000033D6 File Offset: 0x000015D6
		public static string Combine(string path1, string path2, string path3, string path4)
		{
			return Path.Combine(path1, path2, path3, path4);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000033E1 File Offset: 0x000015E1
		public static string Combine(params string[] paths)
		{
			return Path.Combine(paths);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000033E9 File Offset: 0x000015E9
		public static string GetFileName(string path)
		{
			return Path.GetFileName(path);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000033F1 File Offset: 0x000015F1
		public static string GetFileNameWithoutExtension(string path)
		{
			return Path.GetFileNameWithoutExtension(path);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000033F9 File Offset: 0x000015F9
		public static bool IsPathRooted(string path)
		{
			return Path.IsPathRooted(path);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003401 File Offset: 0x00001601
		public static string GetRandomFileName()
		{
			return Path.GetRandomFileName();
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003408 File Offset: 0x00001608
		public static string ChangeExtension(string path, string extension)
		{
			return Path.ChangeExtension(path, extension);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003411 File Offset: 0x00001611
		public static string GetExtension(string path)
		{
			return Path.GetExtension(path);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003419 File Offset: 0x00001619
		public static char[] GetInvalidPathChars()
		{
			return Path.GetInvalidPathChars();
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003420 File Offset: 0x00001620
		public static char[] GetInvalidFileNameChars()
		{
			return Path.GetInvalidFileNameChars();
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003427 File Offset: 0x00001627
		public static string GetTempPath()
		{
			return Path.GetTempPath();
		}

		// Token: 0x06000069 RID: 105 RVA: 0x0000342E File Offset: 0x0000162E
		public static string GetTempFileName()
		{
			return Path.GetTempFileName();
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003435 File Offset: 0x00001635
		public static bool HasExtension(string path)
		{
			return Path.HasExtension(path);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003440 File Offset: 0x00001640
		internal static void CheckInvalidPathChars(string path)
		{
			foreach (int num in path)
			{
				if (num == 34 || num == 60 || num == 62 || num == 124 || num < 32)
				{
					throw new ArgumentException("Illegal characters in path.");
				}
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000348C File Offset: 0x0000168C
		internal static int GetRootLength(string path)
		{
			LongPathPath.CheckInvalidPathChars(path);
			int i = 0;
			int length = path.Length;
			if (length >= 1 && LongPathPath.IsDirectorySeparator(path[0]))
			{
				i = 1;
				if (length >= 2 && LongPathPath.IsDirectorySeparator(path[1]))
				{
					i = 2;
					int num = 2;
					while (i < length)
					{
						if ((path[i] == LongPathPath.DirectorySeparatorChar || path[i] == LongPathPath.AltDirectorySeparatorChar) && --num <= 0)
						{
							break;
						}
						i++;
					}
				}
			}
			else if (length >= 2 && path[1] == LongPathPath.VolumeSeparatorChar)
			{
				i = 2;
				if (length >= 3 && LongPathPath.IsDirectorySeparator(path[2]))
				{
					i++;
				}
			}
			return i;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000352D File Offset: 0x0000172D
		internal static bool IsDirectorySeparator(char c)
		{
			return c == LongPathPath.DirectorySeparatorChar || c == LongPathPath.AltDirectorySeparatorChar;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003544 File Offset: 0x00001744
		private static string NormalizePath(string path, bool fullCheck)
		{
			if (fullCheck)
			{
				path = path.TrimEnd(LongPathPath.TrimEndChars);
				LongPathPath.CheckInvalidPathChars(path);
			}
			string text = path.Substring(0, LongPathPath.GetRootLength(path));
			path = path.Remove(0, text.Length);
			path = path.Replace(LongPathPath.AltDirectorySeparatorChar, LongPathPath.DirectorySeparatorChar);
			string text2 = new string(new char[]
			{
				LongPathPath.DirectorySeparatorChar,
				LongPathPath.DirectorySeparatorChar
			});
			do
			{
				path = path.Replace(text2, LongPathPath.DirectorySeparatorChar.ToString());
			}
			while (path.Contains(text2));
			path = path.Insert(0, text);
			return path;
		}

		// Token: 0x04000002 RID: 2
		public static readonly char DirectorySeparatorChar = Path.DirectorySeparatorChar;

		// Token: 0x04000003 RID: 3
		public static readonly char AltDirectorySeparatorChar = Path.AltDirectorySeparatorChar;

		// Token: 0x04000004 RID: 4
		public static readonly char VolumeSeparatorChar = Path.VolumeSeparatorChar;

		// Token: 0x04000005 RID: 5
		[Obsolete("Please use GetInvalidPathChars or GetInvalidFileNameChars instead.")]
		public static readonly char[] InvalidPathChars = Path.InvalidPathChars;

		// Token: 0x04000006 RID: 6
		public static readonly char PathSeparator = Path.PathSeparator;

		// Token: 0x04000007 RID: 7
		internal static readonly char[] TrimEndChars = new char[]
		{
			'\t',
			'\n',
			'\v',
			'\f',
			'\r',
			' ',
			'\u0085',
			'\u00a0'
		};
	}
}
