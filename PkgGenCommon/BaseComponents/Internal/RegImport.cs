using System;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x0200006A RID: 106
	public class RegImport
	{
		// Token: 0x060001E8 RID: 488 RVA: 0x00003E08 File Offset: 0x00002008
		public RegImport()
		{
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x000077BE File Offset: 0x000059BE
		public RegImport(string source)
		{
			this.Source = source;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x000077CD File Offset: 0x000059CD
		public void Preprocess(IMacroResolver macroResolver)
		{
			this.Source = macroResolver.Resolve(this.Source);
		}

		// Token: 0x060001EB RID: 491 RVA: 0x000077E1 File Offset: 0x000059E1
		public void Build(IPackageGenerator pkgGen)
		{
			pkgGen.ImportRegistry(this.Source).Build(pkgGen);
		}

		// Token: 0x0400016C RID: 364
		[XmlAttribute("Source")]
		public string Source;
	}
}
