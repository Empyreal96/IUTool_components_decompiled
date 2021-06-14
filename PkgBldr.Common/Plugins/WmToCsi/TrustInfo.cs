using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000027 RID: 39
	[Export(typeof(IPkgPlugin))]
	internal class TrustInfo : PkgPlugin
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00006C94 File Offset: 0x00004E94
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.Shared.Xsd\\TrustInfo.xsd";
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000090 RID: 144 RVA: 0x00006C9B File Offset: 0x00004E9B
		public override string XmlSchemaNameSpace
		{
			get
			{
				return "urn:Microsoft.CompPlat/ManifestSchema.v1.00";
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00006CA4 File Offset: 0x00004EA4
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement xelement = new XElement(FromWm);
			PkgBldrHelpers.SetDefaultNameSpace(xelement, ToCsi.Name.Namespace);
			ToCsi.Add(xelement);
		}
	}
}
