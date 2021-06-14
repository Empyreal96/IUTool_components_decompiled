using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000006 RID: 6
	[Export(typeof(IPkgPlugin))]
	internal class Configuration : PkgPlugin
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600000E RID: 14 RVA: 0x0000209D File Offset: 0x0000029D
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.Shared.Xsd\\Configuration.xsd";
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000020A4 File Offset: 0x000002A4
		public override string XmlSchemaNameSpace
		{
			get
			{
				return "urn:Microsoft.CompPlat/ManifestSchema.v1.00";
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000020AC File Offset: 0x000002AC
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement content = new XElement(FromWm);
			PkgBldrHelpers.ReplaceDefaultNameSpace(ref content, FromWm.Name.Namespace, ToCsi.Name.Namespace);
			ToCsi.Add(content);
		}
	}
}
