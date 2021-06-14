using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000019 RID: 25
	[Export(typeof(IPkgPlugin))]
	internal class ServiceGroup : PkgPlugin
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00006754 File Offset: 0x00004954
		public override void ConvertEntries(XElement toCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromWm)
		{
			string attributeValue = PkgBldrHelpers.GetAttributeValue(fromWm, "serviceName");
			string attributeValue2 = PkgBldrHelpers.GetAttributeValue(fromWm, "groupName");
			string attributeValue3 = PkgBldrHelpers.GetAttributeValue(fromWm, "buildFilter");
			string attributeValue4 = PkgBldrHelpers.GetAttributeValue(fromWm, "position");
			XContainer xcontainer = Membership.Add(toCsi, attributeValue3, attributeValue2, "Microsoft.Windows.Categories", "1.0.0.0", "365143bb27e7ac8b", "SvcHost");
			XElement xelement = new XElement(toCsi.Name.Namespace + "serviceGroup");
			xcontainer.Add(xelement);
			xelement.Add(new XAttribute("serviceName", attributeValue));
			if (attributeValue4 != null)
			{
				xelement.Add(new XAttribute("position", attributeValue4));
			}
		}
	}
}
