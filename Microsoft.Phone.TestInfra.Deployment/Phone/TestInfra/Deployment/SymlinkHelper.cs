using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Tools.IO;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000012 RID: 18
	public static class SymlinkHelper
	{
		// Token: 0x060000EB RID: 235 RVA: 0x0000649C File Offset: 0x0000469C
		public static void CreateSymlink(string source, string target, bool overwrite)
		{
			bool flag = string.IsNullOrEmpty(source);
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = string.IsNullOrEmpty(target);
			if (flag2)
			{
				throw new ArgumentNullException("target");
			}
			bool flag3 = string.Equals(source, target, StringComparison.OrdinalIgnoreCase);
			if (flag3)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Source file is equal to target: {0}", new object[]
				{
					source
				}));
			}
			bool flag4 = !LongPathFile.Exists(source);
			if (flag4)
			{
				throw new FileNotFoundException("Source file does not exist", source);
			}
			bool flag5 = LongPathFile.Exists(target);
			bool flag6 = flag5 && overwrite;
			if (flag6)
			{
				LongPathFile.Delete(target);
				flag5 = false;
			}
			bool flag7 = !flag5;
			if (flag7)
			{
				LongPathDirectory.Create(LongPathPath.GetDirectoryName(target));
				bool flag8 = !NativeMethods.CreateSymbolicLink(target, source, NativeMethods.SymbolicLinkFlag.File);
				if (flag8)
				{
					Exception exceptionForHR = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to create symbolic link: {0} -> {1}", new object[]
					{
						target,
						source
					}), exceptionForHR);
				}
				SymlinkHelper.SetSymlinkTimestamps(target, source);
			}
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000659C File Offset: 0x0000479C
		public static void CreateSymlinks(string sourceDirectory, string targetDirectory, bool overwrite)
		{
			SymlinkHelper.CreateSymlinks(sourceDirectory, targetDirectory, overwrite, null);
		}

		// Token: 0x060000ED RID: 237 RVA: 0x000065AC File Offset: 0x000047AC
		public static void CreateSymlinks(string sourceDirectory, string targetDirectory, bool overwrite, IEnumerable<string> filesToSkip)
		{
			bool flag = string.IsNullOrEmpty(sourceDirectory);
			if (flag)
			{
				throw new ArgumentNullException("sourceDirectory");
			}
			bool flag2 = string.IsNullOrEmpty(targetDirectory);
			if (flag2)
			{
				throw new ArgumentNullException("targetDirectory");
			}
			foreach (string text in LongPathDirectory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories))
			{
				bool flag3 = filesToSkip != null && filesToSkip.Contains(LongPathPath.GetFileName(text), StringComparer.OrdinalIgnoreCase);
				if (!flag3)
				{
					string target = PathHelper.ChangeParent(text, sourceDirectory, targetDirectory);
					SymlinkHelper.CreateSymlink(text, target, overwrite);
				}
			}
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00006660 File Offset: 0x00004860
		private static void SetSymlinkTimestamps(string link, string source)
		{
			using (SafeFileHandle safeFileHandle = NativeMethods.CreateFile(link, 256U, 3U, IntPtr.Zero, 3U, 2097152U, IntPtr.Zero))
			{
				long num = LongPathFile.GetCreationTimeUtc(source).ToFileTimeUtc();
				long num2 = LongPathFile.GetLastAccessTimeUtc(source).ToFileTimeUtc();
				long num3 = LongPathFile.GetLastWriteTimeUtc(source).ToFileTimeUtc();
				bool flag = !NativeMethods.SetFileTime(safeFileHandle, ref num, ref num2, ref num3);
				if (flag)
				{
					Exception exceptionForHR = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
					throw new InvalidOperationException("Unable to update symbolic link timestamps", exceptionForHR);
				}
			}
		}
	}
}
