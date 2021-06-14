using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000011 RID: 17
	public static class LongPath
	{
		// Token: 0x06000086 RID: 134 RVA: 0x000038A1 File Offset: 0x00001AA1
		public static string GetFullPath(string path)
		{
			return LongPathCommon.NormalizeLongPath(path).Substring("\\\\?\\".Length);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000038B8 File Offset: 0x00001AB8
		public static string GetPathRoot(string path)
		{
			if (path == null)
			{
				return null;
			}
			if (path == string.Empty || path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
			{
				throw new ArgumentException("Path parameter was empty or otherwise invalid.");
			}
			if (!Path.IsPathRooted(path))
			{
				return string.Empty;
			}
			if (path.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase))
			{
				int num = path.IndexOf(Path.DirectorySeparatorChar, "\\\\".Length);
				if (num == -1)
				{
					return path;
				}
				int num2 = path.IndexOf(Path.DirectorySeparatorChar, num + 1);
				if (num2 == -1)
				{
					return path;
				}
				return path.Substring(0, num2);
			}
			else
			{
				if (path.IndexOf(Path.VolumeSeparatorChar) != 1)
				{
					return string.Empty;
				}
				if (path.Length <= 2 || path[2] != Path.DirectorySeparatorChar)
				{
					return path.Substring(0, 2);
				}
				return path.Substring(0, 3);
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00003982 File Offset: 0x00001B82
		public static string GetFileName(string path)
		{
			return Path.GetFileName(path);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000398A File Offset: 0x00001B8A
		public static string GetDirectoryName(string path)
		{
			return Path.GetDirectoryName(path);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003992 File Offset: 0x00001B92
		public static string RemoveExtension(string path)
		{
			return Regex.Replace(path, "\\.[^\\.]+$", "");
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000039A4 File Offset: 0x00001BA4
		public static string GetExtension(string path)
		{
			return Regex.Match(path.ToLowerInvariant(), "\\.[^\\.]+$").Value.TrimStart(new char[]
			{
				'.'
			});
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000039CC File Offset: 0x00001BCC
		public static string Combine(string path, string file)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", new object[]
			{
				path.TrimEnd(new char[]
				{
					'\\'
				}),
				file
			});
		}

		// Token: 0x0400002B RID: 43
		private const string LONGPATH_PREFIX = "\\\\?\\";
	}
}
