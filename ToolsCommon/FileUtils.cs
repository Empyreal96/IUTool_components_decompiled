using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000023 RID: 35
	public static class FileUtils
	{
		// Token: 0x06000120 RID: 288 RVA: 0x00007455 File Offset: 0x00005655
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

		// Token: 0x06000121 RID: 289 RVA: 0x0000748D File Offset: 0x0000568D
		public static string GetTempFile()
		{
			return FileUtils.GetTempFile(Path.GetTempPath());
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00007499 File Offset: 0x00005699
		public static string GetTempFile(string dir)
		{
			return Path.Combine(dir, Path.GetRandomFileName());
		}

		// Token: 0x06000123 RID: 291 RVA: 0x000074A6 File Offset: 0x000056A6
		public static void DeleteTree(string dirPath)
		{
			if (string.IsNullOrEmpty(dirPath))
			{
				throw new ArgumentException("Empty directory path");
			}
			if (LongPathFile.Exists(dirPath))
			{
				throw new IOException(string.Format("Cannot delete directory {0}, it's a file", dirPath));
			}
			if (!LongPathDirectory.Exists(dirPath))
			{
				return;
			}
			LongPathDirectory.Delete(dirPath, true);
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000074E4 File Offset: 0x000056E4
		public static void DeleteFile(string filePath)
		{
			if (!LongPathFile.Exists(filePath))
			{
				return;
			}
			LongPathFile.SetAttributes(filePath, FileAttributes.Normal);
			LongPathFile.Delete(filePath);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00007500 File Offset: 0x00005700
		public static void CleanDirectory(string dirPath)
		{
			if (string.IsNullOrEmpty(dirPath))
			{
				throw new ArgumentException("Empty directory path");
			}
			if (LongPathFile.Exists(dirPath))
			{
				throw new IOException(string.Format("Cannot create directory {0}, a file with same name exists", dirPath));
			}
			NativeMethods.IU_CleanDirectory(dirPath, false);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00007538 File Offset: 0x00005738
		public static string GetTempDirectory()
		{
			string text;
			do
			{
				text = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			}
			while (LongPathDirectory.Exists(text));
			LongPathDirectory.CreateDirectory(text);
			return text;
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00007564 File Offset: 0x00005764
		public static bool IsTargetUpToDate(string inputFile, string targetFile)
		{
			if (!LongPathFile.Exists(targetFile))
			{
				return false;
			}
			DateTime lastWriteTimeUtc = new FileInfo(targetFile).LastWriteTimeUtc;
			return !(new FileInfo(inputFile).LastWriteTimeUtc > lastWriteTimeUtc);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x000075A0 File Offset: 0x000057A0
		public static string GetFileVersion(string filepath)
		{
			string result = string.Empty;
			if (LongPathFile.Exists(filepath))
			{
				result = FileVersionInfo.GetVersionInfo(filepath).FileVersion;
			}
			return result;
		}

		// Token: 0x06000129 RID: 297 RVA: 0x000075C8 File Offset: 0x000057C8
		public static string GetCurrentAssemblyFileVersion()
		{
			return FileUtils.GetFileVersion(Process.GetCurrentProcess().MainModule.FileName);
		}

		// Token: 0x0600012A RID: 298
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern uint GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string lpszLongPath, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszShortPath, uint cchBuffer);

		// Token: 0x0600012B RID: 299 RVA: 0x000075E0 File Offset: 0x000057E0
		public static string GetShortPathName(string dirPath)
		{
			StringBuilder stringBuilder = new StringBuilder(260);
			if (FileUtils.GetShortPathName(dirPath, stringBuilder, 260U) == 0U)
			{
				return dirPath;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00007610 File Offset: 0x00005810
		public static void CopyDirectory(string source, string destination)
		{
			LongPathDirectory.CreateDirectory(destination);
			foreach (string text in LongPathDirectory.GetFiles(source))
			{
				LongPathFile.Copy(text, Path.Combine(destination, Path.GetFileName(text)));
			}
			foreach (string text2 in LongPathDirectory.GetDirectories(source))
			{
				FileUtils.CopyDirectory(text2, Path.Combine(destination, Path.GetFileName(text2)));
			}
		}

		// Token: 0x04000064 RID: 100
		public const int MAX_PATH = 260;
	}
}
