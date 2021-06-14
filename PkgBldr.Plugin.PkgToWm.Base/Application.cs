using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000013 RID: 19
	[Export(typeof(IPkgPlugin))]
	internal class Application : PkgPlugin
	{
		// Token: 0x06000041 RID: 65 RVA: 0x00005173 File Offset: 0x00003373
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			enviorn.Logger.LogWarning("<Package> <Application> not converted", new object[0]);
		}
	}
}
