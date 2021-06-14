using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000029 RID: 41
	[Export(typeof(IPkgPlugin))]
	internal class PrivateResources : PkgPlugin
	{
		// Token: 0x06000088 RID: 136 RVA: 0x00007AD8 File Offset: 0x00005CD8
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = new XElement(ToWm.Name.Namespace + "privateResources");
			foreach (XElement xelement2 in FromPkg.Elements())
			{
				XElement xelement3 = new XElement(ToWm.Name.Namespace + Helpers.lowerCamel(xelement2.Name.LocalName));
				foreach (XAttribute xattribute in xelement2.Attributes())
				{
					if (xattribute.Name.LocalName == "Path")
					{
						xattribute.Value = enviorn.Macros.Resolve(xattribute.Value);
					}
					xelement3.Add(new XAttribute(Helpers.lowerCamel(xattribute.Name.LocalName), xattribute.Value));
				}
				xelement.Add(xelement3);
			}
			if (xelement.HasElements)
			{
				ToWm.Add(xelement);
			}
		}
	}
}
