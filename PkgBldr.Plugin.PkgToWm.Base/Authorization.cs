using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200001E RID: 30
	[Export(typeof(IPkgPlugin))]
	internal class Authorization : PkgPlugin
	{
		// Token: 0x06000058 RID: 88 RVA: 0x00005D09 File Offset: 0x00003F09
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			enviorn.Logger.LogWarning("<Package> <Authorization> not converted", new object[0]);
		}
	}
}
