using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200000B RID: 11
	[Export(typeof(IPkgPlugin))]
	internal class File : PkgPlugin
	{
		// Token: 0x0600001E RID: 30 RVA: 0x00002F88 File Offset: 0x00001188
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement content = ((FileConverter)enviorn.arg).WmFile(ToWm.Name.Namespace, FromPkg);
			ToWm.Add(content);
		}
	}
}
