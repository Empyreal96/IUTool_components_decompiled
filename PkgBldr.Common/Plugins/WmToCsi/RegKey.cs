using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000015 RID: 21
	[Export(typeof(IPkgPlugin))]
	internal class RegKey : PkgPlugin
	{
		// Token: 0x0600005C RID: 92 RVA: 0x00006104 File Offset: 0x00004304
		public override void ConvertEntries(XElement toCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromWm)
		{
			string attributeValue = PkgBldrHelpers.GetAttributeValue(fromWm.Parent, "buildFilter");
			XElement xelement = new XElement(toCsi.Name.Namespace + "registryKey");
			if (attributeValue != null)
			{
				xelement.Add(new XAttribute("buildFilter", attributeValue));
			}
			XElement xelement2 = null;
			foreach (XAttribute xattribute in fromWm.Attributes())
			{
				string localName = xattribute.Name.LocalName;
				if (!(localName == "keyName"))
				{
					if (!(localName == "buildFilter"))
					{
						if (!(localName == "securityDescriptor"))
						{
							if (!(localName == "owner"))
							{
								throw new PkgGenException("Attribute not supported {0}", new object[]
								{
									xattribute.Name.LocalName
								});
							}
							xelement.Add(new XAttribute("owner", xattribute.Value));
						}
						else
						{
							xelement2 = new XElement(toCsi.Name.Namespace + "securityDescriptor");
							xelement2.Add(new XAttribute("name", enviorn.Macros.Resolve(xattribute.Value)));
						}
					}
					else if (attributeValue != null)
					{
						enviorn.Logger.LogWarning("TBD: Can't aggregate build filters from:", new object[0]);
						enviorn.Logger.LogWarning("   RegKeys = {0}", new object[]
						{
							attributeValue
						});
						enviorn.Logger.LogWarning("   RegKey  = {0}", new object[]
						{
							xattribute.Value
						});
						enviorn.Logger.LogWarning("Using {0}", new object[]
						{
							attributeValue
						});
					}
					else
					{
						xelement.Add(new XAttribute("buildFilter", xattribute.Value));
					}
				}
				else
				{
					xelement.Add(new XAttribute("keyName", enviorn.Macros.Resolve(xattribute.Value)));
				}
			}
			base.ConvertEntries(xelement, plugins, enviorn, fromWm);
			if (xelement2 != null)
			{
				xelement.Add(xelement2);
			}
			toCsi.Add(xelement);
		}
	}
}
