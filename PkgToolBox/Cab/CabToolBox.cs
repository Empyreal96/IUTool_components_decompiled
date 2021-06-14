using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Composition.ToolBox.IO;

namespace Microsoft.Composition.ToolBox.Cab
{
	// Token: 0x0200001E RID: 30
	public class CabToolBox
	{
		// Token: 0x060000A8 RID: 168 RVA: 0x000054CC File Offset: 0x000036CC
		public static void CreateCab(string output, List<string> files, List<string> cabPaths)
		{
			CabToolBox.CreateCab(output, files, cabPaths, CabToolBox.CompressionType.FastLZX);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000054D8 File Offset: 0x000036D8
		public static void CreateCab(string output, List<string> files, List<string> cabPaths, CabToolBox.CompressionType compressionType)
		{
			if (files.Count != cabPaths.Count)
			{
				throw new ArgumentException(string.Format("CabToolBox::CreateCab: The list of provide cabPaths is not the same length as the number of provided files.\nFile Count: {0}\nCab Path Count: {1}", files.Count, cabPaths.Count));
			}
			string text = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			DirectoryToolBox.Create(text);
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			foreach (string text2 in files)
			{
				if (!FileToolBox.Exists(text2))
				{
					list2.Add(text2);
				}
				else
				{
					list.Add(LongPathIO.UNCPath(text2));
				}
			}
			if (list2.Count != 0)
			{
				string text3 = string.Empty;
				foreach (string str in list2)
				{
					text3 = text3 + str + Environment.NewLine;
				}
				throw new FileNotFoundException(string.Format("CabToolBox::CreateCab: Cab creation failed due to missing files. The following files are missing:\n{0}", text3));
			}
			if (FileToolBox.Exists(output))
			{
				FileToolBox.Delete(output);
			}
			uint num = NativeMethods.Cab_CreateCabSelected(output, list.ToArray(), (uint)list.Count, cabPaths.ToArray(), (uint)cabPaths.Count, text, null, compressionType);
			if (num != 0U)
			{
				throw new Exception(string.Format("CabToolBox::CreateCab: CreateCabSelected Failed with error code '{0}' for package '{1}'.", num, output));
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x0000565C File Offset: 0x0000385C
		public static void ExtractSelected(string fileName, string outputDir, List<string> filesToExtract)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName", "CabToolBox::ExtractSelected: fileName cannot be null");
			}
			fileName = LongPathIO.UNCPath(fileName);
			if (string.IsNullOrEmpty(outputDir))
			{
				throw new ArgumentNullException("outputDir", "CabToolBox::ExtractSelected: outputDir cannot be null");
			}
			outputDir = LongPathIO.UNCPath(outputDir);
			if (filesToExtract == null)
			{
				throw new ArgumentNullException("filesToExtract", "CabToolBox::ExtractSelected: filesToExtract cannot be null");
			}
			string[] array = filesToExtract.ToArray();
			uint num = (uint)array.Length;
			if (num == 0U)
			{
				throw new ArgumentException("CabToolBox::ExtractSelected: Parameter 'filesToExtract' cannot be empty");
			}
			if (!FileToolBox.Exists(fileName))
			{
				throw new FileNotFoundException(string.Format("CabToolBox::ExtractSelected: CAB file {0} not found", fileName), fileName);
			}
			for (int i = 0; i < CabToolBox.IntRetryCount; i++)
			{
				uint num2 = NativeMethods.Cab_ExtractSelected(fileName, outputDir, array, num);
				if (num2 == 0U)
				{
					break;
				}
				if (i == CabToolBox.IntRetryCount - 1)
				{
					throw new Exception(string.Format("CabToolBox::ExtractSelected: Cab extract Failed with error code '{0}' for package '{1}'.", num2, fileName));
				}
				Thread.Sleep(CabToolBox.IntSleepTimeMs);
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00005738 File Offset: 0x00003938
		public static void ExtractCab(string fileName, string targetLoc)
		{
			uint num = NativeMethods.Cab_Extract(LongPathIO.UNCPath(fileName), LongPathIO.UNCPath(targetLoc));
			if (num != 0U)
			{
				throw new Exception(string.Format("CabToolBox::ExtractCab: Cab extract Failed with error code '{0}' for package '{1}'.", num, fileName));
			}
		}

		// Token: 0x0400006D RID: 109
		public static readonly int IntRetryCount = 20;

		// Token: 0x0400006E RID: 110
		public static readonly int IntSleepTimeMs = 100;

		// Token: 0x02000027 RID: 39
		public enum CompressionType
		{
			// Token: 0x040000AC RID: 172
			None,
			// Token: 0x040000AD RID: 173
			MSZip,
			// Token: 0x040000AE RID: 174
			LZX,
			// Token: 0x040000AF RID: 175
			FastLZX
		}
	}
}
