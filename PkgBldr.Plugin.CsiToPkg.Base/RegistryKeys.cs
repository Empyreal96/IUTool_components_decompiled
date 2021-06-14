using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x02000006 RID: 6
	[Export(typeof(IPkgPlugin))]
	internal class RegistryKeys : PkgPlugin
	{
		// Token: 0x06000010 RID: 16 RVA: 0x00002970 File Offset: 0x00000B70
		public override void ConvertEntries(XElement ToPkg, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromCsi)
		{
			base.ConvertEntries(ToPkg, plugins, enviorn, FromCsi);
		}
	}
}
