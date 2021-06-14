using System;

namespace Microsoft.CompPlat.PkgBldr.Interfaces
{
	// Token: 0x02000003 RID: 3
	public interface IPackageGenerator
	{
		// Token: 0x0600001B RID: 27
		void Build(string projPath, string outputDir, bool compress);
	}
}
