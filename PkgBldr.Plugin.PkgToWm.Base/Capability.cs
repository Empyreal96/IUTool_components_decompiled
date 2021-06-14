using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000003 RID: 3
	[Export(typeof(IPkgPlugin))]
	internal class Capability : PkgPlugin
	{
		// Token: 0x06000003 RID: 3 RVA: 0x000020BC File Offset: 0x000002BC
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = new XElement(ToWm.Name.Namespace + "capability");
			foreach (XAttribute xattribute in FromPkg.Attributes())
			{
				string localName = xattribute.Name.LocalName;
				if (!(localName == "Id"))
				{
					if (!(localName == "FriendlyName"))
					{
						if (!(localName == "Visibility"))
						{
							if (!(localName == "buildFilter"))
							{
								enviorn.Logger.LogWarning("<Capability> attribute not handled {0}", new object[]
								{
									xattribute.Name.LocalName
								});
							}
							else
							{
								string value = Helpers.ConvertBuildFilter(xattribute.Value);
								xelement.Add(new XAttribute("buildFilter", value));
							}
						}
						else
						{
							xelement.Add(new XAttribute("visibility", xattribute.Value));
						}
					}
					else
					{
						xelement.Add(new XAttribute("friendlyName", xattribute.Value));
					}
				}
				else
				{
					xelement.Add(new XAttribute("id", xattribute.Value));
				}
			}
			ToWm.Add(xelement);
			base.ConvertEntries(xelement, plugins, enviorn, FromPkg);
		}
	}
}
