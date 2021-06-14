using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000011 RID: 17
	[Export(typeof(IPkgPlugin))]
	internal class Macro : Macros
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00005A5F File Offset: 0x00003C5F
		public override void ConvertEntries(XElement parent, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement component)
		{
			enviorn.Macros.Register(component.Attribute("id").Value, component.Attribute("value").Value);
		}
	}
}
