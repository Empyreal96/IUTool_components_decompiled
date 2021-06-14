using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000021 RID: 33
	[Export(typeof(IPkgPlugin))]
	internal class RegKeys : PkgPlugin
	{
		// Token: 0x06000073 RID: 115 RVA: 0x000067C0 File Offset: 0x000049C0
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = new XElement(ToWm.Name.Namespace + "regKeys");
			base.ConvertEntries(xelement, plugins, enviorn, FromPkg);
			if (xelement.HasElements)
			{
				string text = Helpers.GenerateWmBuildFilter(FromPkg, enviorn.Logger);
				string text2 = Helpers.GenerateWmResolutionFilter(FromPkg);
				if (text2 != null)
				{
					xelement.Add(new XAttribute("resolution", text2));
				}
				if (text != null)
				{
					if (text2 != null)
					{
						enviorn.Logger.LogWarning("Ignoring buildFilter because it can't be used with Resolution", new object[0]);
					}
					else
					{
						xelement.Add(new XAttribute("buildFilter", text));
					}
				}
				ToWm.Add(xelement);
			}
		}
	}
}
