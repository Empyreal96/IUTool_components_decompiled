using System;
using System.IO;
using System.Text;

namespace Microsoft.Composition.ToolBox.IO
{
	// Token: 0x02000017 RID: 23
	public class LongPathIO
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00003360 File Offset: 0x00001560
		internal static string UNCPath(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				return filename;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int fullPathName = NativeMethods.GetFullPathName(filename, 0, stringBuilder, null);
			stringBuilder.EnsureCapacity(fullPathName);
			fullPathName = NativeMethods.GetFullPathName(filename, stringBuilder.Capacity, stringBuilder, null);
			string text = stringBuilder.ToString();
			if (!text.StartsWith("\\\\?\\", StringComparison.InvariantCultureIgnoreCase))
			{
				if (text.StartsWith("\\\\", StringComparison.InvariantCultureIgnoreCase))
				{
					text = "\\\\?\\UNC\\" + text.TrimStart(new char[]
					{
						'\\'
					});
				}
				else
				{
					text = "\\\\?\\" + text;
				}
			}
			return text;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000033EC File Offset: 0x000015EC
		internal static bool Exists(string filepath, LongPathIO.SystemType type)
		{
			string lpFileName = LongPathIO.UNCPath(filepath);
			uint fileAttributes;
			try
			{
				fileAttributes = NativeMethods.GetFileAttributes(lpFileName);
			}
			catch
			{
				throw new FormatException(string.Format("LongPathIO::Exists: Could not get the attributes of file '{0}'", filepath));
			}
			if (type == LongPathIO.SystemType.File)
			{
				return LongPathIO.IsFile((FileAttributes)fileAttributes);
			}
			return LongPathIO.IsDirectory((FileAttributes)fileAttributes);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000343C File Offset: 0x0000163C
		internal static bool IsDirectory(string filepath)
		{
			string lpFileName = LongPathIO.UNCPath(filepath);
			uint fileAttributes;
			try
			{
				fileAttributes = NativeMethods.GetFileAttributes(lpFileName);
			}
			catch
			{
				throw new FormatException(string.Format("LongPathIO::IsDirectory: Could not get the attributes of file '{0}'", filepath));
			}
			return LongPathIO.IsDirectory((FileAttributes)fileAttributes);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00003484 File Offset: 0x00001684
		internal static bool IsDirectory(FileAttributes attributes)
		{
			return (attributes & FileAttributes.Directory) == FileAttributes.Directory && attributes != (FileAttributes)(-1);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00003497 File Offset: 0x00001697
		internal static bool IsFile(FileAttributes attributes)
		{
			return (attributes & FileAttributes.Directory) != FileAttributes.Directory && attributes != (FileAttributes)(-1);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000034AA File Offset: 0x000016AA
		internal static bool ValidHandle(IntPtr handle)
		{
			return handle != LongPathIO.NullHandleValue && handle != LongPathIO.InvalidHandleValue;
		}

		// Token: 0x0400005E RID: 94
		internal const string UncPrefix = "\\\\?\\";

		// Token: 0x0400005F RID: 95
		internal const string UncNetworkPrefix = "\\\\?\\UNC\\";

		// Token: 0x04000060 RID: 96
		internal static IntPtr InvalidHandleValue = new IntPtr(-1);

		// Token: 0x04000061 RID: 97
		internal static IntPtr NullHandleValue = new IntPtr(0);

		// Token: 0x02000020 RID: 32
		internal enum SystemType
		{
			// Token: 0x04000074 RID: 116
			File,
			// Token: 0x04000075 RID: 117
			Directory
		}
	}
}
