using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000019 RID: 25
	[Export(typeof(IPkgPlugin))]
	internal class FullTrust : PkgPlugin
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00005CB7 File Offset: 0x00003EB7
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			enviorn.Logger.LogWarning("<Package> <FullTrust> not converted", new object[0]);
		}
	}
}
