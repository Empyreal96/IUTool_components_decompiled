using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000002 RID: 2
	[Export(typeof(IPkgPlugin))]
	internal class Capabilities : PkgPlugin
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = PkgBldrHelpers.AddIfNotFound(ToWm, "capabilities");
			string attributeValue = PkgBldrHelpers.GetAttributeValue(FromPkg, "buildFilter");
			if (attributeValue != null)
			{
				string value = Helpers.ConvertBuildFilter(attributeValue);
				XAttribute content = new XAttribute("buildFilter", value);
				xelement.Add(content);
			}
			base.ConvertEntries(xelement, plugins, enviorn, FromPkg);
			if (!xelement.HasElements)
			{
				xelement.Remove();
			}
		}
	}
}
