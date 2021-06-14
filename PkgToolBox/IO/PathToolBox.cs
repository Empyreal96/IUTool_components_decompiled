using System;
using System.IO;

namespace Microsoft.Composition.ToolBox.IO
{
	// Token: 0x02000018 RID: 24
	public class PathToolBox
	{
		// Token: 0x06000076 RID: 118 RVA: 0x000034DE File Offset: 0x000016DE
		public static string LongPath(string path)
		{
			return LongPathIO.UNCPath(path);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000034E8 File Offset: 0x000016E8
		public static string RelativePath(string filePath, string referencePath)
		{
			Uri uri = new Uri(filePath, UriKind.Absolute);
			if (!referencePath.EndsWith("\\", StringComparison.InvariantCultureIgnoreCase))
			{
				referencePath += "\\";
			}
			return new Uri(referencePath, UriKind.Absolute).MakeRelativeUri(uri).ToString().Replace('/', '\\');
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00003533 File Offset: 0x00001733
		public static string Combine(string path1, string path2)
		{
			path1 = path1.TrimEnd(new char[]
			{
				'\\'
			});
			path2 = path2.TrimStart(new char[]
			{
				'\\'
			});
			return path1 + "\\" + path2;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00003568 File Offset: 0x00001768
		public static string GetTemporaryPath()
		{
			return PathToolBox.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00003592 File Offset: 0x00001792
		public static string GetFileNameFromPath(string path)
		{
			string[] array = path.Split(new char[]
			{
				'\\'
			});
			return array[(int)(checked((IntPtr)(unchecked((long)array.Length - 1L))))];
		}
	}
}
