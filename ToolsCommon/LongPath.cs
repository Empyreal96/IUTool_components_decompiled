using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000049 RID: 73
	public static class LongPath
	{
		// Token: 0x060001EC RID: 492 RVA: 0x00009B3C File Offset: 0x00007D3C
		public static string GetDirectoryName(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path", "Path cannot be null.");
			}
			if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
			{
				throw new ArgumentException("Path cannot contain invalid characters.", "path");
			}
			int num = path.LastIndexOfAny(new char[]
			{
				Path.DirectorySeparatorChar,
				Path.VolumeSeparatorChar
			});
			if (num == -1)
			{
				return null;
			}
			return path.Substring(0, num);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00009BA8 File Offset: 0x00007DA8
		public static string GetFullPath(string path)
		{
			string text = LongPathCommon.NormalizeLongPath(path);
			if (text.StartsWith("\\\\?\\UNC\\", StringComparison.OrdinalIgnoreCase))
			{
				return "\\\\" + text.Substring("\\\\?\\UNC\\".Length);
			}
			if (text.StartsWith("\\\\?\\", StringComparison.OrdinalIgnoreCase))
			{
				return text.Substring("\\\\?\\".Length);
			}
			return text;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x00009C05 File Offset: 0x00007E05
		public static string GetFullPathUNC(string path)
		{
			return LongPathCommon.NormalizeLongPath(path);
		}

		// Token: 0x060001EF RID: 495 RVA: 0x00009C10 File Offset: 0x00007E10
		public static string GetPathRoot(string path)
		{
			if (path == null)
			{
				return null;
			}
			if (path == string.Empty || path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
			{
				throw new ArgumentException("Path cannot be empty or contain invalid characters.", "path");
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

		// Token: 0x060001F0 RID: 496 RVA: 0x00009CDF File Offset: 0x00007EDF
		public static string Combine(string path, string file)
		{
			return string.Format("{0}\\{1}", path.TrimEnd(new char[]
			{
				'\\'
			}), file.Trim(new char[]
			{
				'\\'
			}));
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00009D0D File Offset: 0x00007F0D
		public static string GetFileName(string path)
		{
			return Regex.Match(path, "\\\\[^\\\\]+$").Value.TrimStart(new char[]
			{
				'\\'
			});
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00009D30 File Offset: 0x00007F30
		public static string GetExtension(string path)
		{
			if (path == null)
			{
				return null;
			}
			string text = Regex.Match(path.ToLowerInvariant(), "\\.[^\\.]+$").Value.TrimStart(new char[]
			{
				'.'
			});
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			return "." + text;
		}

		// Token: 0x040000FB RID: 251
		private const string UNC_PREFIX = "\\\\";

		// Token: 0x040000FC RID: 252
		private const string LONGPATH_PREFIX = "\\\\?\\";

		// Token: 0x040000FD RID: 253
		private const string LONGPATH_UNC_PREFIX = "\\\\?\\UNC\\";
	}
}
