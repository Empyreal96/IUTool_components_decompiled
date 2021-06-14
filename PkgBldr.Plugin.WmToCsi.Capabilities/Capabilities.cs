using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins
{
	// Token: 0x02000002 RID: 2
	[Export(typeof(IPkgPlugin))]
	internal class Capabilities : PkgPlugin
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.WM.Xsd\\SecurityPlugin.xsd";
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public override void ConvertEntries(XElement parent, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement component)
		{
			string attributeValue = PkgBldrHelpers.GetAttributeValue(component, "buildFilter");
			if (attributeValue != null && !enviorn.ExpressionEvaluator.Evaluate(attributeValue))
			{
				return;
			}
			base.ConvertEntries(parent, plugins, enviorn, component);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002090 File Offset: 0x00000290
		protected string GetAttributeValue(XElement element, string attributeName)
		{
			XAttribute xattribute = element.Attribute(attributeName);
			if (xattribute == null)
			{
				return null;
			}
			return xattribute.Value;
		}
	}
}
