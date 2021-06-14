using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000014 RID: 20
	[Export(typeof(IPkgPlugin))]
	internal class BCDStore : PkgPlugin
	{
		// Token: 0x06000043 RID: 67 RVA: 0x0000518C File Offset: 0x0000338C
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = new XElement(ToWm.Name.Namespace + "bcdStore", new XAttribute("source", FromPkg.Attribute("Source").Value));
			string attributeValue = PkgBldrHelpers.GetAttributeValue(FromPkg, "buildFilter");
			if (attributeValue != null)
			{
				string value = Helpers.ConvertBuildFilter(attributeValue);
				xelement.Add(new XAttribute("buildFilter", value));
			}
			ToWm.Add(xelement);
		}
	}
}
