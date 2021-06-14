using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins
{
	// Token: 0x02000009 RID: 9
	[Export(typeof(IPkgPlugin))]
	internal class CapabilityRule : Authorization
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002994 File Offset: 0x00000B94
		public override string XmlElementName
		{
			get
			{
				return "capabilityRule";
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000299C File Offset: 0x00000B9C
		public override void ConvertEntries(XElement authorizationRegKeys, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement component)
		{
			XElement xelement = new XElement(authorizationRegKeys.Name.Namespace + "registryKey");
			xelement.Add(new XAttribute("keyName", Authorization.authorizationRegKeyRoot + CapabilityRule.capabilityClassRegKeyName + component.Attribute("name").Value));
			XElement xelement2 = new XElement(authorizationRegKeys.Name.Namespace + "registryValue");
			XElement xelement3 = new XElement(authorizationRegKeys.Name.Namespace + "registryValue");
			xelement2.Add(new XAttribute("name", "CapabilityClass"));
			xelement2.Add(new XAttribute("value", component.Attribute("capabilityClass").Value));
			xelement2.Add(new XAttribute("valueType", "REG_SZ"));
			xelement3.Add(new XAttribute("name", "PrincipalClass"));
			xelement3.Add(new XAttribute("value", component.Attribute("principalClass").Value));
			xelement3.Add(new XAttribute("valueType", "REG_SZ"));
			xelement.Add(xelement2);
			xelement.Add(xelement3);
			XElement xelement4 = environ.Bld.CSI.Root.Element(environ.Bld.CSI.Root.Name.Namespace + "registryKeys");
			if (xelement4 == null)
			{
				xelement4 = new XElement(environ.Bld.CSI.Root.Name.Namespace + "registryKeys");
				environ.Bld.CSI.Root.Add(xelement4);
			}
			xelement4.Add(xelement);
		}

		// Token: 0x04000007 RID: 7
		private static string capabilityClassRegKeyName = "AuthorizationRules\\Capability\\";
	}
}
