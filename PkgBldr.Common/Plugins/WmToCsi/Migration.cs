using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000012 RID: 18
	[Export(typeof(IPkgPlugin))]
	internal class Migration : PkgPlugin
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00005AA0 File Offset: 0x00003CA0
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.Shared.Xsd\\Migration.xsd";
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00005AA7 File Offset: 0x00003CA7
		public override string XmlSchemaNameSpace
		{
			get
			{
				return "urn:Microsoft.CompPlat/ManifestSchema.v1.00";
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00005AB0 File Offset: 0x00003CB0
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement content = new XElement(FromWm);
			PkgBldrHelpers.ReplaceDefaultNameSpace(ref content, FromWm.Name.Namespace, ToCsi.Name.Namespace);
			ToCsi.Add(content);
		}
	}
}
