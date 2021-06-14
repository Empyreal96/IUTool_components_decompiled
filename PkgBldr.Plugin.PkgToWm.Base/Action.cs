using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000026 RID: 38
	[Export(typeof(IPkgPlugin))]
	internal class Action : PkgPlugin
	{
		// Token: 0x06000082 RID: 130 RVA: 0x00007758 File Offset: 0x00005958
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = new XElement(ToWm.Name.Namespace + "action");
			foreach (XAttribute xattribute in FromPkg.Attributes())
			{
				xattribute.Value = enviorn.Macros.Resolve(xattribute.Value);
				string localName = xattribute.Name.LocalName;
				if (localName == "Type" || localName == "Delay")
				{
					string expandedName = Helpers.lowerCamel(xattribute.Name.LocalName);
					string value = Helpers.lowerCamel(xattribute.Value);
					xelement.Add(new XAttribute(expandedName, value));
				}
			}
			ToWm.Add(xelement);
		}
	}
}
