using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x0200000F RID: 15
	[Export(typeof(IPkgPlugin))]
	internal class Localization : PkgPlugin
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00005A1B File Offset: 0x00003C1B
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.Shared.Xsd\\Local.xsd";
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00005A22 File Offset: 0x00003C22
		public override string XmlSchemaNameSpace
		{
			get
			{
				return "urn:Microsoft.CompPlat/ManifestSchema.v1.00";
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00005A2C File Offset: 0x00003C2C
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement xelement = new XElement(FromWm);
			PkgBldrHelpers.SetDefaultNameSpace(xelement, ToCsi.Name.Namespace);
			ToCsi.Add(xelement);
		}
	}
}
