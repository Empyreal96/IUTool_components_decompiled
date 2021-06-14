using System;
using System.IO;
using System.Reflection;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x02000008 RID: 8
	public static class PkgGenResources
	{
		// Token: 0x06000024 RID: 36 RVA: 0x0000254C File Offset: 0x0000074C
		public static Stream GetProjSchemaStream()
		{
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(PkgGenResources).Namespace + ".ProjSchema.xsd");
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002571 File Offset: 0x00000771
		public static Stream GetGlobalMacroStream()
		{
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(PkgGenResources).Namespace + ".PkgGen.cfg.xml");
		}
	}
}
