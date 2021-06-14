using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200001C RID: 28
	[Export(typeof(IPkgPlugin))]
	internal class Components : PkgPlugin
	{
		// Token: 0x06000054 RID: 84 RVA: 0x0000514E File Offset: 0x0000334E
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			base.ConvertEntries(ToWm, plugins, enviorn, FromPkg);
		}
	}
}
