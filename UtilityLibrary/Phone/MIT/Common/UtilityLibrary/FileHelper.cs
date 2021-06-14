using System;
using System.IO;

namespace Microsoft.Phone.MIT.Common.UtilityLibrary
{
	// Token: 0x02000018 RID: 24
	public class FileHelper
	{
		// Token: 0x06000078 RID: 120 RVA: 0x00004510 File Offset: 0x00002710
		public static bool FileCompare(string file1, string file2)
		{
			byte[] array = new byte[64];
			byte[] array2 = new byte[64];
			bool result;
			if (string.Compare(Path.GetFullPath(file1), Path.GetFullPath(file2), StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = true;
			}
			else
			{
				using (FileStream fileStream = new FileStream(file1, FileMode.Open, FileAccess.Read, FileShare.Read, 65536))
				{
					using (FileStream fileStream2 = new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.Read, 65536))
					{
						if (fileStream.Length != fileStream2.Length)
						{
							return false;
						}
						for (;;)
						{
							int num = fileStream.ReadReally(array, 64);
							num = fileStream2.ReadReally(array2, 64);
							if (num == 0)
							{
								break;
							}
							for (int i = 0; i < num; i++)
							{
								if (array[i] != array2[i])
								{
									goto Block_8;
								}
							}
						}
						goto IL_E8;
						Block_8:
						return false;
					}
					IL_E8:;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00004640 File Offset: 0x00002840
		public static bool FileCompareByLine(string file1, string file2, Func<int, string, string, bool> comparer = null)
		{
			int num = 0;
			bool result;
			using (StreamReader streamReader = new StreamReader(new FileStream(file1, FileMode.Open, FileAccess.Read, FileShare.Read, 65536)))
			{
				using (StreamReader streamReader2 = new StreamReader(new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.Read, 65536)))
				{
					for (;;)
					{
						string text = streamReader.EndOfStream ? null : streamReader.ReadLine();
						string text2 = streamReader2.EndOfStream ? null : streamReader2.ReadLine();
						num++;
						if (text == null && text2 == null)
						{
							break;
						}
						bool flag;
						if (comparer != null)
						{
							flag = comparer(num, text, text2);
						}
						else
						{
							flag = (string.Compare(text, text2, StringComparison.OrdinalIgnoreCase) != 0);
						}
						if (flag)
						{
							goto Block_10;
						}
					}
					return true;
					Block_10:
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00004750 File Offset: 0x00002950
		public static bool CopyFileIfNecessary(string source, string destination)
		{
			FileInfo fileInfo = new FileInfo(source);
			FileInfo fileInfo2 = new FileInfo(destination);
			bool result;
			if (fileInfo2.Exists && fileInfo.Length == fileInfo2.Length && fileInfo.LastWriteTime == fileInfo2.LastWriteTime)
			{
				result = false;
			}
			else
			{
				string directoryName = Path.GetDirectoryName(destination);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				File.Copy(source, destination, true);
				result = true;
			}
			return result;
		}
	}
}
