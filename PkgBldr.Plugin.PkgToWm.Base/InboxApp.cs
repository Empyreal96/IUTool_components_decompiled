using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200001A RID: 26
	[Export(typeof(IPkgPlugin))]
	internal class InboxApp : PkgPlugin
	{
		// Token: 0x06000050 RID: 80 RVA: 0x00005CCF File Offset: 0x00003ECF
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			enviorn.Logger.LogWarning("<Package> <InboxApp> not converted", new object[0]);
		}
	}
}
