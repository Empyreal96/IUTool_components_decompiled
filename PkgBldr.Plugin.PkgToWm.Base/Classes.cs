using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000007 RID: 7
	[Export(typeof(IPkgPlugin))]
	internal class Classes : PkgPlugin
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002A2E File Offset: 0x00000C2E
		public override void ConvertEntries(XElement toWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromPkg)
		{
			ComData comData = (ComData)enviorn.arg;
			base.ConvertEntries(toWm, plugins, enviorn, fromPkg);
		}
	}
}
