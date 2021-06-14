using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000018 RID: 24
	[Export(typeof(IPkgPlugin))]
	internal class WinRTHost : PkgPlugin
	{
		// Token: 0x0600004C RID: 76 RVA: 0x00005C9F File Offset: 0x00003E9F
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			enviorn.Logger.LogWarning("<Package> <WinRTHost> not converted", new object[0]);
		}
	}
}
