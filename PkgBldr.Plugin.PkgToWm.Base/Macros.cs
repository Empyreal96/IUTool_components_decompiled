using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200000E RID: 14
	[Export(typeof(IPkgPlugin))]
	internal class Macros : PkgPlugin
	{
		// Token: 0x06000029 RID: 41 RVA: 0x00003488 File Offset: 0x00001688
		public override bool Pass(BuildPass pass)
		{
			return pass == BuildPass.MACRO_PASS;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003490 File Offset: 0x00001690
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = new XElement(ToWm.Name.Namespace + "macros");
			base.ConvertEntries(xelement, plugins, enviorn, FromPkg);
			if (xelement.HasElements)
			{
				ToWm.Add(xelement);
			}
		}
	}
}
