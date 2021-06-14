using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins
{
	// Token: 0x02000002 RID: 2
	[Export(typeof(IPkgPlugin))]
	internal class Accounts : PkgPlugin
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override string XmlSchemaPath
		{
			get
			{
				return Accounts.securityComponentSchemaPath;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
		public override string XmlElementName
		{
			get
			{
				return "accounts";
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002060 File Offset: 0x00000260
		public override void ConvertEntries(XElement root, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement component)
		{
			XElement xelement = new XElement("trustInfo");
			XElement xelement2 = new XElement("security");
			XElement xelement3 = new XElement("accessControl");
			XElement xelement4 = new XElement("trustees");
			base.ConvertEntries(xelement4, plugins, environ, component);
			xelement3.Add(xelement4);
			xelement2.Add(xelement3);
			xelement.Add(xelement2);
			PkgBldrHelpers.SetDefaultNameSpace(xelement, root.Name.Namespace);
			environ.Bld.CSI.Root.Add(xelement);
		}

		// Token: 0x04000001 RID: 1
		private static string securityComponentSchemaPath = "PkgBldr.WM.Xsd\\SecurityPlugin.xsd";
	}
}
