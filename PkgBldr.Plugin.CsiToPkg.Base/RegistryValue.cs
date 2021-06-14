using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x02000008 RID: 8
	[Export(typeof(IPkgPlugin))]
	internal class RegistryValue : PkgPlugin
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00002AAC File Offset: 0x00000CAC
		public override void ConvertEntries(XElement ToPkg, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromCsi)
		{
			string attributeValue = PkgBldrHelpers.GetAttributeValue(FromCsi, "name");
			string attributeValue2 = PkgBldrHelpers.GetAttributeValue(FromCsi, "valueType");
			string text = PkgBldrHelpers.GetAttributeValue(FromCsi, "value");
			text = enviorn.Macros.Resolve(text);
			if (text.StartsWith("$(ERROR)", StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine("warning: skipping key {0}", text);
				return;
			}
			XElement xelement = RegHelpers.PkgRegValue(attributeValue, attributeValue2, text);
			if (xelement != null)
			{
				ToPkg.Add(xelement);
			}
		}
	}
}
