using System;
using System.IO;
using System.Reflection;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000032 RID: 50
	public static class PkgGenResources
	{
		// Token: 0x060000D5 RID: 213 RVA: 0x00007A90 File Offset: 0x00005C90
		public static Stream GetResourceStream(string embeddedFileName)
		{
			string text = typeof(PkgGenResources).Namespace + ".Resources." + embeddedFileName;
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(text);
			Assembly.GetCallingAssembly().GetManifestResourceNames();
			if (manifestResourceStream == null)
			{
				throw new PkgGenException("Failed to load resource stream {0}", new object[]
				{
					text
				});
			}
			return manifestResourceStream;
		}
	}
}
