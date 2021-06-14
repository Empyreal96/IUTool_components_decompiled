using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200001D RID: 29
	[Export(typeof(IPkgPlugin))]
	internal class CustomMetadata : PkgPlugin
	{
		// Token: 0x06000056 RID: 86 RVA: 0x00005CF1 File Offset: 0x00003EF1
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			enviorn.Logger.LogWarning("<Package> <CustomMetadata> not converted", new object[0]);
		}
	}
}
