using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000012 RID: 18
	public static class LongPathDirectory
	{
		// Token: 0x0600008D RID: 141 RVA: 0x00003A08 File Offset: 0x00001C08
		public static void CreateDirectory(string path)
		{
			try
			{
				NativeMethods.IU_EnsureDirectoryExists(path);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00003A30 File Offset: 0x00001C30
		public static void Delete(string path)
		{
			string text = LongPathCommon.NormalizeLongPath(path);
			if (!LongPathDirectory.Exists(text))
			{
				return;
			}
			if (!NativeMethods.RemoveDirectory(text))
			{
				throw LongPathCommon.GetExceptionFromLastWin32Error();
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00003A5B File Offset: 0x00001C5B
		public static void Delete(string path, bool recursive)
		{
			if (recursive)
			{
				NativeMethods.IU_CleanDirectory(path, true);
				return;
			}
			LongPathDirectory.Delete(path);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00003A6E File Offset: 0x00001C6E
		public static bool Exists(string path)
		{
			return NativeMethods.IU_DirectoryExists(path);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00003A78 File Offset: 0x00001C78
		public static FileAttributes GetAttributes(string path)
		{
			FileAttributes attributes = LongPathCommon.GetAttributes(path);
			if (!attributes.HasFlag(FileAttributes.Directory))
			{
				throw LongPathCommon.GetExceptionFromWin32Error(267);
			}
			return attributes;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00003AAC File Offset: 0x00001CAC
		public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOptions)
		{
			return LongPathDirectory.GetDirectories(path, searchPattern, searchOptions);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00003AB6 File Offset: 0x00001CB6
		public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
		{
			return LongPathDirectory.EnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00003AC0 File Offset: 0x00001CC0
		public static IEnumerable<string> EnumerateDirectories(string path)
		{
			return LongPathDirectory.EnumerateDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00003AD0 File Offset: 0x00001CD0
		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults")]
		public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOptions)
		{
			if (searchOptions != SearchOption.AllDirectories && searchOptions != SearchOption.TopDirectoryOnly)
			{
				throw new NotImplementedException("Unknown search option: " + searchOptions);
			}
			bool fRecursive = searchOptions == SearchOption.AllDirectories;
			IntPtr zero = IntPtr.Zero;
			int num = 0;
			string strFolder = LongPath.Combine(path, LongPath.GetDirectoryName(searchPattern));
			string fileName = Path.GetFileName(searchPattern);
			int num2 = NativeMethods.IU_GetAllDirectories(strFolder, fileName, fRecursive, out zero, out num);
			if (num2 != 0)
			{
				throw LongPathCommon.GetExceptionFromWin32Error(num2);
			}
			string[] result;
			try
			{
				result = LongPathCommon.ConvertPtrArrayToStringArray(zero, num);
			}
			finally
			{
				NativeMethods.IU_FreeStringList(zero, num);
			}
			return result;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003B5C File Offset: 0x00001D5C
		public static string[] GetDirectories(string path, string searchPattern)
		{
			return LongPathDirectory.GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00003B66 File Offset: 0x00001D66
		public static string[] GetDirectories(string path)
		{
			return LongPathDirectory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00003B74 File Offset: 0x00001D74
		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOptions)
		{
			return LongPathDirectory.GetFiles(path, searchPattern, searchOptions);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00003B7E File Offset: 0x00001D7E
		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
		{
			return LongPathDirectory.EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00003B88 File Offset: 0x00001D88
		public static IEnumerable<string> EnumerateFiles(string path)
		{
			return LongPathDirectory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00003B98 File Offset: 0x00001D98
		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults")]
		public static string[] GetFiles(string path, string searchPattern, SearchOption searchOptions)
		{
			if (searchOptions != SearchOption.AllDirectories && searchOptions != SearchOption.TopDirectoryOnly)
			{
				throw new NotImplementedException("Unknown search option: " + searchOptions);
			}
			bool fRecursive = searchOptions == SearchOption.AllDirectories;
			IntPtr zero = IntPtr.Zero;
			int num = 0;
			string strFolder = LongPath.Combine(path, Path.GetDirectoryName(searchPattern));
			string fileName = Path.GetFileName(searchPattern);
			int num2 = NativeMethods.IU_GetAllFiles(strFolder, fileName, fRecursive, out zero, out num);
			if (num2 != 0)
			{
				throw LongPathCommon.GetExceptionFromWin32Error(num2);
			}
			string[] result;
			try
			{
				result = LongPathCommon.ConvertPtrArrayToStringArray(zero, num);
			}
			finally
			{
				NativeMethods.IU_FreeStringList(zero, num);
			}
			return result;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00003C24 File Offset: 0x00001E24
		public static string[] GetFiles(string path, string searchPattern)
		{
			return LongPathDirectory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00003C2E File Offset: 0x00001E2E
		public static string[] GetFiles(string path)
		{
			return LongPathDirectory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
		}

		// Token: 0x0400002C RID: 44
		public const string ALL_FILE_PATTERN = "*.*";
	}
}
