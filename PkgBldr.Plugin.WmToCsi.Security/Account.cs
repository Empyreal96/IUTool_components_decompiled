using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins
{
	// Token: 0x02000003 RID: 3
	[Export(typeof(IPkgPlugin))]
	internal class Account : PkgPlugin
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002108 File Offset: 0x00000308
		public override string XmlSchemaPath
		{
			get
			{
				return Account.securityComponentSchemaPath;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000007 RID: 7 RVA: 0x0000210F File Offset: 0x0000030F
		public override string XmlElementName
		{
			get
			{
				return "account";
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002118 File Offset: 0x00000318
		public override void ConvertEntries(XElement trustees, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement component)
		{
			XElement xelement = new XElement("groupTrustee");
			xelement.Add(new XAttribute("name", component.Attribute("name").Value));
			xelement.Add(new XAttribute("description", component.Attribute("description").Value));
			xelement.Add(new XAttribute("type", "VirtualAccount"));
			base.ConvertEntries(xelement, plugins, environ, component);
			trustees.Add(xelement);
		}

		// Token: 0x04000002 RID: 2
		private static string securityComponentSchemaPath = "PkgBldr.WM.Xsd\\SecurityPlugin.xsd";
	}
}
