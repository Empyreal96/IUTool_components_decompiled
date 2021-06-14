using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x0200001E RID: 30
	[Export(typeof(IPkgPlugin))]
	internal class SystemProtection : PkgPlugin
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000071 RID: 113 RVA: 0x000068F9 File Offset: 0x00004AF9
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.Shared.Xsd\\Service.xsd";
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00006900 File Offset: 0x00004B00
		public override string XmlSchemaNameSpace
		{
			get
			{
				return "urn:Microsoft.CompPlat/ManifestSchema.v1.00";
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00006908 File Offset: 0x00004B08
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement xelement = new XElement(FromWm);
			PkgBldrHelpers.SetDefaultNameSpace(xelement, ToCsi.Name.Namespace);
			ToCsi.Add(xelement);
		}
	}
}
