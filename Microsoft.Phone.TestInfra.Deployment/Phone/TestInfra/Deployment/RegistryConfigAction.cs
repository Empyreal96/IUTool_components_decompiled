using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000032 RID: 50
	public class RegistryConfigAction : ConfigActionBase
	{
		// Token: 0x0600023F RID: 575 RVA: 0x0000E738 File Offset: 0x0000C938
		public override List<ConfigCommand> GetConfigCommand(HashSet<string> deployedPackages, string outputPath)
		{
			bool flag = deployedPackages == null || deployedPackages.Count == 0;
			if (flag)
			{
				throw new ArgumentNullException("deployedPackages");
			}
			bool flag2 = string.IsNullOrWhiteSpace(outputPath);
			if (flag2)
			{
				throw new ArgumentNullException("outputPath");
			}
			List<ConfigCommand> list = new List<ConfigCommand>();
			string path = Path.Combine(outputPath, ConfigActionBase.RelativeConfigFolder);
			string path2 = Path.Combine(outputPath, ConfigActionBase.RelativeFilesFolder);
			bool flag3 = !Directory.Exists(path2);
			if (flag3)
			{
				throw new InvalidDataException("Could not find the files folder in the deployment output.");
			}
			path2 = Path.GetFullPath(path2);
			string[] files = Directory.GetFiles(path2, "*.reg", SearchOption.AllDirectories);
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string regFile = array[i];
				string fileName = Path.GetFileName(regFile);
				bool flag4 = deployedPackages.Any((string x) => string.Compare(PathHelper.GetFileNameWithoutExtension(x, ".spkg"), Path.GetFileNameWithoutExtension(regFile), true) == 0);
				if (flag4)
				{
					string text = Path.Combine(path, fileName);
					bool flag5 = string.Compare(regFile, text, true) != 0;
					if (flag5)
					{
						File.Copy(regFile, text, true);
					}
					string commandLine = string.Format("reg.exe import \"{0}\"", text);
					list.Add(new ConfigCommand
					{
						CommandLine = commandLine
					});
					Logger.Info("Added command line to import {0}.", new object[]
					{
						fileName
					});
				}
			}
			return list;
		}
	}
}
