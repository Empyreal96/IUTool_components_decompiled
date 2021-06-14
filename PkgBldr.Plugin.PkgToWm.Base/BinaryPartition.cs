using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000015 RID: 21
	[Export(typeof(IPkgPlugin))]
	internal class BinaryPartition : PkgPlugin
	{
		// Token: 0x06000045 RID: 69 RVA: 0x0000520D File Offset: 0x0000340D
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			enviorn.Logger.LogWarning("<Package> <BinaryPartition> not converted", new object[0]);
		}
	}
}
