using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary;
using Microsoft.Tools.IO;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200000F RID: 15
	public static class FileCopyHelper
	{
		// Token: 0x060000D1 RID: 209 RVA: 0x00004E94 File Offset: 0x00003094
		public static bool FilesMatch(string file1, string file2)
		{
			bool flag = string.IsNullOrEmpty(file1);
			if (flag)
			{
				throw new ArgumentNullException("file1");
			}
			bool flag2 = !LongPathFile.Exists(file1);
			if (flag2)
			{
				throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "File file does not exist: {0}", new object[]
				{
					file1
				}));
			}
			bool flag3 = string.IsNullOrEmpty(file2);
			if (flag3)
			{
				throw new ArgumentNullException("file2");
			}
			return LongPathFile.Exists(file2) && ((LongPathFile.GetFileLengthBytes(file1) == LongPathFile.GetFileLengthBytes(file2) && LongPathFile.GetLastWriteTimeUtc(file1) == LongPathFile.GetLastWriteTimeUtc(file2)) || LongPathFile.GetAttributes(file2).HasFlag(FileAttributes.ReparsePoint));
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00004F48 File Offset: 0x00003148
		public static bool CopyFile(string sourceFile, string targetFile, int retryCount, TimeSpan retryDelay)
		{
			bool flag = string.IsNullOrEmpty(sourceFile);
			if (flag)
			{
				throw new ArgumentNullException("sourceFile");
			}
			bool flag2 = string.IsNullOrEmpty(targetFile);
			if (flag2)
			{
				throw new ArgumentNullException("targetFile");
			}
			bool flag3 = retryCount < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("retryCount", retryCount, "Retry count is negative");
			}
			bool flag4 = retryDelay < TimeSpan.Zero;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("retryDelay", retryDelay, "Retry delay is negative");
			}
			return FileCopyHelper.InternalLongCopyFile(sourceFile, targetFile, retryCount, retryDelay);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00004FD8 File Offset: 0x000031D8
		public static IEnumerable<string> CopyFiles(string source, string destination, string pattern, bool recursive, int retryCount, TimeSpan retryDelay)
		{
			IEnumerable<string> enumerable;
			return FileCopyHelper.CopyFiles(source, destination, pattern, recursive, retryCount, retryDelay, out enumerable);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00004FFC File Offset: 0x000031FC
		public static IEnumerable<string> CopyFiles(string source, string destination, string pattern, bool recursive, int retryCount, TimeSpan retryDelay, out IEnumerable<string> skippedFiles)
		{
			bool flag = string.IsNullOrEmpty(source);
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = string.IsNullOrEmpty(destination);
			if (flag2)
			{
				throw new ArgumentNullException("destination");
			}
			bool flag3 = string.IsNullOrEmpty(pattern);
			if (flag3)
			{
				throw new ArgumentNullException("pattern");
			}
			bool flag4 = retryCount < 0;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("retryCount", retryCount, "Retry count is negative");
			}
			bool flag5 = retryDelay < TimeSpan.Zero;
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("retryDelay", retryDelay, "Retry delay is negative");
			}
			LongPathDirectory.Create(destination);
			object syncRoot = new object();
			List<string> affected = new List<string>();
			List<string> skipped = new List<string>();
			IEnumerable<string> source2 = RetryHelper.Retry<IEnumerable<string>>(() => LongPathDirectory.EnumerateFiles(source, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly), retryCount, retryDelay);
			Parallel.ForEach<string>(source2, delegate(string sourceFile)
			{
				string text = PathHelper.ChangeParent(sourceFile, source, destination);
				bool flag6 = FileCopyHelper.CopyFile(sourceFile, text, retryCount, retryDelay);
				object syncRoot = syncRoot;
				lock (syncRoot)
				{
					bool flag8 = !flag6;
					if (flag8)
					{
						skipped.Add(text);
					}
					affected.Add(text);
				}
			});
			skippedFiles = skipped;
			return affected;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00005160 File Offset: 0x00003360
		internal static bool InternalLongCopyFile(string sourceFile, string targetFile, int retryCount, TimeSpan retryDelay)
		{
			bool flag = FileCopyHelper.FilesMatch(sourceFile, targetFile);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				LongPathDirectory.Create(LongPathPath.GetDirectoryName(targetFile));
				RetryHelper.Retry(delegate()
				{
					LongPathFile.Copy(sourceFile, targetFile, true);
				}, retryCount, retryDelay, new Type[]
				{
					typeof(UnauthorizedAccessException),
					typeof(IOException)
				});
				LongPathFile.SetAttributes(targetFile, FileAttributes.Normal);
				LongPathFile.SetCreationTimeUtc(targetFile, LongPathFile.GetCreationTimeUtc(sourceFile));
				LongPathFile.SetLastWriteTimeUtc(targetFile, LongPathFile.GetLastWriteTimeUtc(sourceFile));
				result = true;
			}
			return result;
		}
	}
}
