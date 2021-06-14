using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000018 RID: 24
	[Export(typeof(IPkgPlugin))]
	internal class Service : PkgPlugin
	{
		// Token: 0x06000061 RID: 97 RVA: 0x000065F0 File Offset: 0x000047F0
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			string attributeValue = PkgBldrHelpers.GetAttributeValue(FromWm, "buildFilter");
			string attributeValue2 = PkgBldrHelpers.GetAttributeValue(FromWm, "subCategory");
			XContainer xcontainer = Membership.Add(ToCsi, attributeValue, attributeValue2, "Microsoft.Windows.Categories.Services", "$(build.version)", "$(build.WindowsPublicKeyToken)", "Service");
			XElement xelement = new XElement(ToCsi.Name.Namespace + "serviceData");
			xcontainer.Add(xelement);
			string text = null;
			foreach (XAttribute xattribute in FromWm.Attributes())
			{
				string localName = xattribute.Name.LocalName;
				if (!(localName == "subCategory") && !(localName == "buildFilter"))
				{
					if (localName == "securityDescriptor")
					{
						text = enviorn.Macros.Resolve(xattribute.Value);
					}
					else
					{
						xelement.Add(new XAttribute(xattribute.Name.LocalName, xattribute.Value));
					}
				}
			}
			if (text != null)
			{
				XElement xelement2 = new XElement(ToCsi.Name.Namespace + "securityDescriptor");
				xelement2.Add(new XAttribute("name", text));
				xelement.Add(xelement2);
			}
			base.ConvertEntries(xelement, plugins, enviorn, FromWm);
		}
	}
}
