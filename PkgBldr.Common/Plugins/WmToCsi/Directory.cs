using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000008 RID: 8
	[Export(typeof(IPkgPlugin))]
	internal class Directory : PkgPlugin
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00002134 File Offset: 0x00000334
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement xelement = new XElement(ToCsi.Name.Namespace + "directory");
			string text = null;
			foreach (XAttribute xattribute in FromWm.Attributes())
			{
				string localName = xattribute.Name.LocalName;
				if (!(localName == "path"))
				{
					if (!(localName == "securityDescriptor"))
					{
						if (!(localName == "attributes"))
						{
							if (!(localName == "owner"))
							{
								throw new PkgGenException("Unknow directory attribute {0}", new object[]
								{
									xattribute.Name.LocalName
								});
							}
							xelement.Add(new XAttribute("owner", xattribute.Value));
						}
						else
						{
							string value = enviorn.Macros.Resolve(xattribute.Value);
							xelement.Add(new XAttribute("attributes", value));
						}
					}
					else
					{
						text = enviorn.Macros.Resolve(xattribute.Value);
					}
				}
				else
				{
					string value2 = enviorn.Macros.Resolve(xattribute.Value).TrimEnd(new char[]
					{
						'\\'
					});
					xelement.Add(new XAttribute("destinationPath", value2));
				}
			}
			if (text != null)
			{
				XElement xelement2 = new XElement(ToCsi.Name.Namespace + "securityDescriptor");
				xelement2.Add(new XAttribute("name", text));
				xelement.Add(xelement2);
			}
			ToCsi.Add(xelement);
		}
	}
}
