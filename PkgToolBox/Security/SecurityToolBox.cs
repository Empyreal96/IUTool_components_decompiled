using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Composition.ToolBox.IO;

namespace Microsoft.Composition.ToolBox.Security
{
	// Token: 0x02000010 RID: 16
	public class SecurityToolBox
	{
		// Token: 0x0600004B RID: 75 RVA: 0x00002C0C File Offset: 0x00000E0C
		public static void CreateCatalog(string catPath, IEnumerable<string> filesToSign, string packageName)
		{
			string text = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(Path.GetDirectoryName(catPath));
			using (TextWriter textWriter = new StreamWriter(text))
			{
				textWriter.WriteLine("[CatalogHeader]");
				textWriter.WriteLine("Name={0}", Path.GetFileName(catPath));
				textWriter.WriteLine("ResultDir={0}", Path.GetDirectoryName(catPath));
				textWriter.WriteLine("PublicVersion=0x00000001");
				textWriter.WriteLine("EncodingType=0x00010001");
				textWriter.WriteLine("PageHashes=false");
				textWriter.WriteLine("CatalogVersion=2");
				textWriter.WriteLine("HashAlgorithms=SHA256");
				textWriter.WriteLine("CATATTR1=0x00010001:OSAttr:2:6.1,2:6.0,2:5.2,2:5.1");
				textWriter.WriteLine("CATATTR2=0x00010001:PackageName:{0}\r\n", packageName);
				textWriter.WriteLine();
				textWriter.WriteLine("[CatalogFiles]");
				foreach (string path in filesToSign)
				{
					string text2 = PathToolBox.LongPath(path);
					if (FileToolBox.Size(text2) != 0L)
					{
						textWriter.WriteLine(string.Format("<HASH>{0}={1}", text2, text2));
					}
				}
			}
			if (FileToolBox.Exists(catPath))
			{
				FileToolBox.Delete(catPath);
			}
			Process process = new Process();
			process.StartInfo.FileName = "makecat.exe";
			process.StartInfo.Arguments = string.Format("/v \"{0}\"", text);
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.Start();
			string arg = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			if (process.ExitCode != 0)
			{
				throw new Exception(string.Format("SecurityToolBox::CreateCatalog: makecat.exe failed to resign {0}.\nErr: {1}\nOutput: {2}", catPath, process.ExitCode, arg));
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002DF4 File Offset: 0x00000FF4
		public static void SignFile(string path)
		{
			string text = "ntsign.cmd";
			if (!FileToolBox.GlobalExists(text))
			{
				text = "sign.cmd";
			}
			for (int i = 0; i < SecurityToolBox.IntRetryCount; i++)
			{
				Process process = new Process();
				process.StartInfo.FileName = "cmd.exe";
				process.StartInfo.Arguments = string.Concat(new string[]
				{
					"/c ",
					text,
					" \"",
					path,
					"\""
				});
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.Start();
				string text2 = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				if (process.ExitCode == 0)
				{
					break;
				}
				if (i == SecurityToolBox.IntRetryCount - 1)
				{
					throw new Exception(string.Format("SecurityToolBox::SignFile {0} failed to resign {1}.\nErr: {2}\nOutput: {3}", new object[]
					{
						text,
						path,
						process.ExitCode,
						text2
					}));
				}
				Thread.Sleep(SecurityToolBox.IntSleepTimeMs);
			}
		}

		// Token: 0x04000052 RID: 82
		public static readonly int IntRetryCount = 20;

		// Token: 0x04000053 RID: 83
		public static readonly int IntSleepTimeMs = 500;
	}
}
