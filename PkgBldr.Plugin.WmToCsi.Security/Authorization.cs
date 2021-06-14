using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins
{
	// Token: 0x02000006 RID: 6
	[Export(typeof(IPkgPlugin))]
	internal class Authorization : PkgPlugin
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002308 File Offset: 0x00000508
		public override string XmlSchemaPath
		{
			get
			{
				return Authorization.securityComponentSchemaPath;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000012 RID: 18 RVA: 0x0000230F File Offset: 0x0000050F
		public override string XmlElementName
		{
			get
			{
				return "authorization";
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002318 File Offset: 0x00000518
		public override void ConvertEntries(XElement root, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement component)
		{
			XElement xelement = new XElement(root.Name.Namespace + "registryKeys");
			base.ConvertEntries(xelement, plugins, environ, component);
			root.Add(xelement);
		}

		// Token: 0x04000003 RID: 3
		private static string securityComponentSchemaPath = "PkgBldr.WM.Xsd\\SecurityPlugin.xsd";

		// Token: 0x04000004 RID: 4
		protected static string authorizationRegKeyRoot = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\SecurityManager\\";
	}
}
