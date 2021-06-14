using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000028 RID: 40
	[Export(typeof(IPkgPlugin))]
	internal class ServiceDll : PkgPlugin
	{
		// Token: 0x06000086 RID: 134 RVA: 0x00007A08 File Offset: 0x00005C08
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = new XElement(FromPkg);
			XAttribute xattribute = xelement.Attribute("ServiceMain");
			XAttribute xattribute2 = xelement.Attribute("UnloadOnStop");
			XAttribute xattribute3 = xelement.Attribute("BinaryInOneCorePkg");
			if (xattribute3 != null)
			{
				if (xattribute3.Value.Equals("1") || xattribute3.Value.Equals("true"))
				{
					return;
				}
				xattribute3.Remove();
			}
			XElement xelement2 = new XElement(ToWm.Name.Namespace + "files");
			if (xattribute != null)
			{
				xattribute.Remove();
			}
			if (xattribute2 != null)
			{
				xattribute2.Remove();
			}
			XElement content = new FileConverter(enviorn).WmFile(ToWm.Name.Namespace, xelement);
			xelement2.Add(content);
			ToWm.Add(xelement2);
		}
	}
}
