using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000012 RID: 18
	[Export(typeof(IPkgPlugin))]
	internal class AppResource : PkgPlugin
	{
		// Token: 0x0600003F RID: 63 RVA: 0x0000515B File Offset: 0x0000335B
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			enviorn.Logger.LogWarning("<Package> <AppResource> not converted", new object[0]);
		}
	}
}
