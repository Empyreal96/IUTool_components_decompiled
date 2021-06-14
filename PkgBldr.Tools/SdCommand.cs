using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000030 RID: 48
	public static class SdCommand
	{
		// Token: 0x0600019B RID: 411 RVA: 0x00007740 File Offset: 0x00005940
		public static void Run(string cmd, string file)
		{
			string fileName = Environment.GetEnvironmentVariable("RAZZLETOOLPATH") + "\\x86\\sd.exe";
			Process process = new Process();
			process.StartInfo.FileName = fileName;
			process.StartInfo.Arguments = string.Format("{0} {1}", cmd, file);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.WorkingDirectory = Path.GetDirectoryName(file);
			process.Start();
			string value = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			if (process.ExitCode != 0)
			{
				Console.WriteLine(value);
				throw new Exception(string.Format("Failed to execute sd {0} {1}", cmd, file));
			}
		}
	}
}
