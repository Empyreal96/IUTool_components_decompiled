using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000025 RID: 37
	[Export(typeof(IPkgPlugin))]
	internal class FailureActions : PkgPlugin
	{
		// Token: 0x06000080 RID: 128 RVA: 0x00007644 File Offset: 0x00005844
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = new XElement(ToWm.Name.Namespace + "failureActions");
			foreach (XAttribute xattribute in FromPkg.Attributes())
			{
				xattribute.Value = enviorn.Macros.Resolve(xattribute.Value);
				string localName = xattribute.Name.LocalName;
				if (localName == "Command" || localName == "ResetPeriod" || localName == "RebootMessage")
				{
					string expandedName = Helpers.lowerCamel(xattribute.Name.LocalName);
					xelement.Add(new XAttribute(expandedName, xattribute.Value));
				}
			}
			XElement xelement2 = new XElement(ToWm.Name.Namespace + "actions");
			xelement.Add(xelement2);
			base.ConvertEntries(xelement2, plugins, enviorn, FromPkg);
			ToWm.Add(xelement);
		}
	}
}
