using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000021 RID: 33
	[Export(typeof(IPkgPlugin))]
	internal class GenericCommands : PkgPlugin
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600007D RID: 125 RVA: 0x000069AD File Offset: 0x00004BAD
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.Shared.Xsd\\SharedTypes.xsd";
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600007E RID: 126 RVA: 0x000069B4 File Offset: 0x00004BB4
		public override string XmlSchemaNameSpace
		{
			get
			{
				return "urn:Microsoft.CompPlat/ManifestSchema.v1.00";
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000069BC File Offset: 0x00004BBC
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement xelement = new XElement(FromWm);
			PkgBldrHelpers.SetDefaultNameSpace(xelement, ToCsi.Name.Namespace);
			ToCsi.Add(xelement);
		}
	}
}
