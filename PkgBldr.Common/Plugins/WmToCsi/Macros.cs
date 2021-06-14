using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000010 RID: 16
	[Export(typeof(IPkgPlugin))]
	internal class Macros : PkgPlugin
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00005A59 File Offset: 0x00003C59
		public override bool Pass(BuildPass pass)
		{
			return pass == BuildPass.MACRO_PASS;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000057A4 File Offset: 0x000039A4
		public override void ConvertEntries(XElement parent, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement component)
		{
			base.ConvertEntries(parent, plugins, enviorn, component);
		}
	}
}
