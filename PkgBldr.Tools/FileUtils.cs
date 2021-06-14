using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000031 RID: 49
	public static class FileUtils
	{
		// Token: 0x0600019C RID: 412 RVA: 0x000077F7 File Offset: 0x000059F7
		public static string RerootPath(string path, string oldRoot, string newRoot)
		{
			if (oldRoot.Last<char>() != '\\')
			{
				oldRoot += "\\";
			}
			if (newRoot.Last<char>() != '\\')
			{
				newRoot += "\\";
			}
			return path.Replace(oldRoot, newRoot);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000782F File Offset: 0x00005A2F
		public static string GetTempFile()
		{
			return FileUtils.GetTempFile(Path.GetTempPath());
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000783B File Offset: 0x00005A3B
		public static string GetTempFile(string dir)
		{
			return LongPath.Combine(dir, Path.GetRandomFileName());
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00007848 File Offset: 0x00005A48
		public static void DeleteTree(string dirPath)
		{
			if (string.IsNullOrEmpty(dirPath))
			{
				throw new ArgumentException("Empty directory path");
			}
			if (LongPathFile.Exists(dirPath))
			{
				throw new IOException(string.Format(CultureInfo.InvariantCulture, "Cannot delete directory {0}, it's a file", new object[]
				{
					dirPath
				}));
			}
			if (!LongPathDirectory.Exists(dirPath))
			{
				return;
			}
			LongPathDirectory.Delete(dirPath, true);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000789F File Offset: 0x00005A9F
		public static void DeleteFile(string filePath)
		{
			if (!LongPathFile.Exists(filePath))
			{
				return;
			}
			LongPathFile.SetAttributes(filePath, FileAttributes.Normal);
			LongPathFile.Delete(filePath);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x000078BC File Offset: 0x00005ABC
		public static void CleanDirectory(string dirPath)
		{
			if (string.IsNullOrEmpty(dirPath))
			{
				throw new ArgumentException("Empty directory path");
			}
			if (LongPathFile.Exists(dirPath))
			{
				throw new IOException(string.Format(CultureInfo.InvariantCulture, "Cannot create directory {0}, a file with same name exists", new object[]
				{
					dirPath
				}));
			}
			NativeMethods.IU_CleanDirectory(dirPath, false);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000790C File Offset: 0x00005B0C
		public static string GetTempDirectory()
		{
			string text;
			do
			{
				text = LongPath.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			}
			while (LongPathDirectory.Exists(text));
			LongPathDirectory.CreateDirectory(text);
			return text;
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00007938 File Offset: 0x00005B38
		public static string GetFileVersion(string filepath)
		{
			string result = string.Empty;
			if (LongPathFile.Exists(filepath))
			{
				result = FileVersionInfo.GetVersionInfo(filepath).FileVersion;
			}
			return result;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00007960 File Offset: 0x00005B60
		public static string GetCurrentAssemblyFileVersion()
		{
			return FileUtils.GetFileVersion(Process.GetCurrentProcess().MainModule.FileName);
		}

		// Token: 0x060001A5 RID: 421
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern uint GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string lpszLongPath, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszShortPath, uint cchBuffer);

		// Token: 0x060001A6 RID: 422 RVA: 0x00007978 File Offset: 0x00005B78
		public static string GetShortPathName(string dirPath)
		{
			StringBuilder stringBuilder = new StringBuilder(260);
			if (FileUtils.GetShortPathName(dirPath, stringBuilder, 260U) == 0U || stringBuilder.Length == 0)
			{
				return dirPath;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x000079B0 File Offset: 0x00005BB0
		public static void CopyDirectory(string source, string destination)
		{
			LongPathDirectory.CreateDirectory(destination);
			foreach (string text in LongPathDirectory.GetFiles(source))
			{
				LongPathFile.Copy(text, LongPath.Combine(destination, LongPath.GetFileName(text)));
			}
			foreach (string text2 in LongPathDirectory.GetDirectories(source))
			{
				FileUtils.CopyDirectory(text2, LongPath.Combine(destination, LongPath.GetFileName(text2)));
			}
		}

		// Token: 0x0400007E RID: 126
		public const int MAX_PATH = 260;
	}
}
