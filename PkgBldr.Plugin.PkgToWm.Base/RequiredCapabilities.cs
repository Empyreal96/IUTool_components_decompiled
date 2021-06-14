using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200002A RID: 42
	[Export(typeof(IPkgPlugin))]
	internal class RequiredCapabilities : PkgPlugin
	{
		// Token: 0x0600008A RID: 138 RVA: 0x00007C10 File Offset: 0x00005E10
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = new XElement(ToWm.Name.Namespace + "requiredCapabilities");
			foreach (XElement element in FromPkg.Elements())
			{
				XElement xelement2 = new XElement(ToWm.Name.Namespace + "requiredCapability");
				string attributeValue = PkgBldrHelpers.GetAttributeValue(element, "CapId");
				xelement2.Add(new XAttribute("id", attributeValue));
				xelement.Add(xelement2);
			}
			if (xelement.HasElements)
			{
				ToWm.Add(xelement);
			}
		}
	}
}
