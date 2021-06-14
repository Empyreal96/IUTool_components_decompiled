using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x0200001D RID: 29
	[Export(typeof(IPkgPlugin))]
	internal class FailureActions : PkgPlugin
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600006D RID: 109 RVA: 0x000068BD File Offset: 0x00004ABD
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.Shared.Xsd\\Service.xsd";
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600006E RID: 110 RVA: 0x000068C4 File Offset: 0x00004AC4
		public override string XmlSchemaNameSpace
		{
			get
			{
				return "urn:Microsoft.CompPlat/ManifestSchema.v1.00";
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000068CC File Offset: 0x00004ACC
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement xelement = new XElement(FromWm);
			PkgBldrHelpers.SetDefaultNameSpace(xelement, ToCsi.Name.Namespace);
			ToCsi.Add(xelement);
		}
	}
}
