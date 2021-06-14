using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000031 RID: 49
	[InheritedExport(typeof(ConfigActionBase))]
	public abstract class ConfigActionBase
	{
		// Token: 0x0600023C RID: 572
		public abstract List<ConfigCommand> GetConfigCommand(HashSet<string> deployedPackages, string outputPath);

		// Token: 0x040000F8 RID: 248
		public static readonly string RelativeConfigFolder = "Files\\ConfigActions";

		// Token: 0x040000F9 RID: 249
		public static readonly string RelativeFilesFolder = "Files";
	}
}
