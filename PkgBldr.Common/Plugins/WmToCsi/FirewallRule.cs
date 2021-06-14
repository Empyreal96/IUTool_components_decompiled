using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000020 RID: 32
	[Export(typeof(IPkgPlugin))]
	internal class FirewallRule : PkgPlugin
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00006971 File Offset: 0x00004B71
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.Shared.Xsd\\SharedTypes.xsd";
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00006978 File Offset: 0x00004B78
		public override string XmlSchemaNameSpace
		{
			get
			{
				return "urn:Microsoft.CompPlat/ManifestSchema.v1.00";
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00006980 File Offset: 0x00004B80
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement xelement = new XElement(FromWm);
			PkgBldrHelpers.SetDefaultNameSpace(xelement, ToCsi.Name.Namespace);
			ToCsi.Add(xelement);
		}
	}
}
