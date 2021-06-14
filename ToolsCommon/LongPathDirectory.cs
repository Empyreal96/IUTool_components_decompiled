using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200004A RID: 74
	public static class LongPathDirectory
	{
		// Token: 0x060001F3 RID: 499 RVA: 0x00009D84 File Offset: 0x00007F84
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

		// Token: 0x060001F4 RID: 500 RVA: 0x00009DAC File Offset: 0x00007FAC
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

		// Token: 0x060001F5 RID: 501 RVA: 0x00009DD7 File Offset: 0x00007FD7
		public static void Delete(string path, bool recursive)
		{
			if (recursive)
			{
				NativeMethods.IU_CleanDirectory(path, true);
				return;
			}
			LongPathDirectory.Delete(path);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00009DEA File Offset: 0x00007FEA
		public static bool Exists(string path)
		{
			return NativeMethods.IU_DirectoryExists(path);
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00009DF4 File Offset: 0x00007FF4
		public static FileAttributes GetAttributes(string path)
		{
			FileAttributes attributes = LongPathCommon.GetAttributes(path);
			if (!attributes.HasFlag(FileAttributes.Directory))
			{
				throw LongPathCommon.GetExceptionFromWin32Error(267);
			}
			return attributes;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00009E28 File Offset: 0x00008028
		public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOptions)
		{
			return LongPathDirectory.GetDirectories(path, searchPattern, searchOptions);
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x00009E32 File Offset: 0x00008032
		public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
		{
			return LongPathDirectory.EnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		// Token: 0x060001FA RID: 506 RVA: 0x00009E3C File Offset: 0x0000803C
		public static IEnumerable<string> EnumerateDirectories(string path)
		{
			return LongPathDirectory.EnumerateDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00009E4C File Offset: 0x0000804C
		public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOptions)
		{
			if (searchOptions != SearchOption.AllDirectories && searchOptions != SearchOption.TopDirectoryOnly)
			{
				throw new NotImplementedException("Unknown search option: " + searchOptions);
			}
			bool fRecursive = searchOptions == SearchOption.AllDirectories;
			IntPtr zero = IntPtr.Zero;
			int num = 0;
			string strFolder = Path.Combine(path, Path.GetDirectoryName(searchPattern));
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
				num2 = NativeMethods.IU_FreeStringList(zero, num);
				if (num2 != 0)
				{
					throw LongPathCommon.GetExceptionFromWin32Error(num2);
				}
			}
			return result;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00009EE4 File Offset: 0x000080E4
		public static string[] GetDirectories(string path, string searchPattern)
		{
			return LongPathDirectory.GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00009EEE File Offset: 0x000080EE
		public static string[] GetDirectories(string path)
		{
			return LongPathDirectory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00009EFC File Offset: 0x000080FC
		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOptions)
		{
			return LongPathDirectory.GetFiles(path, searchPattern, searchOptions);
		}

		// Token: 0x060001FF RID: 511 RVA: 0x00009F06 File Offset: 0x00008106
		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
		{
			return LongPathDirectory.EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00009F10 File Offset: 0x00008110
		public static IEnumerable<string> EnumerateFiles(string path)
		{
			return LongPathDirectory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly);
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00009F20 File Offset: 0x00008120
		public static string[] GetFiles(string path, string searchPattern, SearchOption searchOptions)
		{
			if (searchOptions != SearchOption.AllDirectories && searchOptions != SearchOption.TopDirectoryOnly)
			{
				throw new NotImplementedException("Unknown search option: " + searchOptions);
			}
			bool fRecursive = searchOptions == SearchOption.AllDirectories;
			IntPtr zero = IntPtr.Zero;
			int num = 0;
			string strFolder = Path.Combine(path, Path.GetDirectoryName(searchPattern));
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
				num2 = NativeMethods.IU_FreeStringList(zero, num);
				if (num2 != 0)
				{
					throw LongPathCommon.GetExceptionFromWin32Error(num2);
				}
			}
			return result;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00009FB8 File Offset: 0x000081B8
		public static string[] GetFiles(string path, string searchPattern)
		{
			return LongPathDirectory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00009FC2 File Offset: 0x000081C2
		public static string[] GetFiles(string path)
		{
			return LongPathDirectory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
		}

		// Token: 0x040000FE RID: 254
		public const string ALL_FILE_PATTERN = "*.*";
	}
}
