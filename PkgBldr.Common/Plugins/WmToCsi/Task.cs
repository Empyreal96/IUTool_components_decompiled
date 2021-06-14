using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000024 RID: 36
	[Export(typeof(IPkgPlugin))]
	internal class Task : PkgPlugin
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00006BE8 File Offset: 0x00004DE8
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.WM.Xsd\\Task.xsd";
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00006BEF File Offset: 0x00004DEF
		public override string XmlSchemaNameSpace
		{
			get
			{
				return "urn:Microsoft.CompPlat/ManifestSchema.v1.00";
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00006BF8 File Offset: 0x00004DF8
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement xelement = new XElement(FromWm);
			XNamespace xnamespace = "http://schemas.microsoft.com/windows/2004/02/mit/task";
			PkgBldrHelpers.ReplaceDefaultNameSpace(ref xelement, FromWm.Name.Namespace, xnamespace);
			base.ConvertEntries(ToCsi, plugins, enviorn, FromWm);
			foreach (XElement xelement2 in xelement.Descendants(xnamespace + "privateResources").ToList<XElement>())
			{
				xelement2.Remove();
			}
			ToCsi.Add(xelement);
		}
	}
}
