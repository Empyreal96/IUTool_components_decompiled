using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000030 RID: 48
	public class ConfigCommandAggregator
	{
		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000238 RID: 568 RVA: 0x0000E5F8 File Offset: 0x0000C7F8
		// (set) Token: 0x06000239 RID: 569 RVA: 0x0000E600 File Offset: 0x0000C800
		[ImportMany(typeof(ConfigActionBase))]
		public IEnumerable<Lazy<ConfigActionBase>> ConfigActionSet { get; set; }

		// Token: 0x0600023A RID: 570 RVA: 0x0000E60C File Offset: 0x0000C80C
		public List<ConfigCommand> GetConfigCommands(HashSet<string> deployedPackages, string outputPath)
		{
			bool flag = deployedPackages == null || deployedPackages.Count<string>() == 0;
			if (flag)
			{
				throw new ArgumentException("cannot be null or empty", "deployedPackages");
			}
			bool flag2 = string.IsNullOrEmpty(outputPath);
			if (flag2)
			{
				throw new ArgumentException("cannot be null or empty", "outputPath");
			}
			bool flag3 = !Directory.Exists(outputPath);
			if (flag3)
			{
				throw new DirectoryNotFoundException(outputPath);
			}
			List<ConfigCommand> list = new List<ConfigCommand>();
			string path = Path.Combine(outputPath, ConfigActionBase.RelativeConfigFolder);
			Directory.CreateDirectory(path);
			AssemblyCatalog catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
			CompositionContainer compositionService = new CompositionContainer(catalog, new ExportProvider[0]);
			compositionService.SatisfyImportsOnce(this);
			bool flag4 = this.ConfigActionSet != null;
			if (flag4)
			{
				foreach (Lazy<ConfigActionBase> lazy in this.ConfigActionSet)
				{
					list.AddRange(lazy.Value.GetConfigCommand(deployedPackages, outputPath));
				}
			}
			return list;
		}
	}
}
