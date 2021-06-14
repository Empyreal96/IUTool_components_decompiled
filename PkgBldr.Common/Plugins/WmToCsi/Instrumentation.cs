using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000022 RID: 34
	[Export(typeof(IPkgPlugin))]
	internal class Instrumentation : PkgPlugin
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000081 RID: 129 RVA: 0x000069E9 File Offset: 0x00004BE9
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.Shared.Xsd\\SharedTypes.xsd";
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000069F0 File Offset: 0x00004BF0
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement content = new XElement(FromWm);
			PkgBldrHelpers.ReplaceDefaultNameSpace(ref content, FromWm.Name.Namespace, ToCsi.Name.Namespace);
			ToCsi.Add(content);
		}
	}
}
