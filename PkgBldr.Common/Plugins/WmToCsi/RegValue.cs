using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000016 RID: 22
	[Export(typeof(IPkgPlugin))]
	internal class RegValue : PkgPlugin
	{
		// Token: 0x0600005E RID: 94 RVA: 0x00006354 File Offset: 0x00004554
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement xelement = new XElement(ToCsi.Name.Namespace + "registryValue");
			foreach (XAttribute xattribute in FromWm.Attributes())
			{
				string localName = xattribute.Name.LocalName;
				if (!(localName == "name"))
				{
					if (!(localName == "value"))
					{
						if (!(localName == "type"))
						{
							if (!(localName == "buildFilter"))
							{
								if (localName == "operationHint")
								{
									xelement.Add(new XAttribute("operationHint", xattribute.Value));
								}
							}
							else
							{
								xelement.Add(new XAttribute("buildFilter", xattribute.Value));
							}
						}
						else
						{
							string value = xattribute.Value;
							xelement.Add(new XAttribute("valueType", value));
						}
					}
					else
					{
						string value2 = enviorn.Macros.Resolve(xattribute.Value);
						xelement.Add(new XAttribute("value", value2));
					}
				}
				else
				{
					string value3 = xattribute.Value;
					xelement.Add(new XAttribute("name", value3));
				}
			}
			ToCsi.Add(xelement);
		}
	}
}
