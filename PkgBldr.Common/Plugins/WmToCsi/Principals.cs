using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000025 RID: 37
	[Export(typeof(IPkgPlugin))]
	internal class Principals : PkgPlugin
	{
		// Token: 0x0600008B RID: 139 RVA: 0x000057A4 File Offset: 0x000039A4
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			base.ConvertEntries(ToCsi, plugins, enviorn, FromWm);
		}
	}
}
