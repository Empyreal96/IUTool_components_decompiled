using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins
{
	// Token: 0x02000003 RID: 3
	[Export(typeof(IPkgPlugin))]
	internal class Capability : Capabilities
	{
		// Token: 0x06000005 RID: 5 RVA: 0x000020C0 File Offset: 0x000002C0
		public override void ConvertEntries(XElement parent, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement component)
		{
			MacroResolver macros = enviorn.Macros;
			string attributeValue = PkgBldrHelpers.GetAttributeValue(component, "buildFilter");
			if (attributeValue != null && !enviorn.ExpressionEvaluator.Evaluate(attributeValue))
			{
				return;
			}
			base.ConvertEntries(parent, plugins, enviorn, component);
			if (macros.Resolve(base.GetAttributeValue(component, "adminOnMultiSession")) == "Yes")
			{
				string attributeValue2 = base.GetAttributeValue(component, "id");
				XElement xelement = new XElement(parent.Name.Namespace + "registryKeys");
				XElement xelement2 = new XElement(parent.Name.Namespace + "registryKey");
				xelement2.Add(new XAttribute("keyName", Capability.adminCapabilitiesKey));
				XElement xelement3 = new XElement(parent.Name.Namespace + "registryValue");
				xelement3.Add(new XAttribute("name", attributeValue2));
				xelement3.Add(new XAttribute("value", 1));
				xelement3.Add(new XAttribute("valueType", "REG_DWORD"));
				xelement2.Add(xelement3);
				xelement.Add(xelement2);
				parent.Add(xelement);
			}
		}

		// Token: 0x04000001 RID: 1
		protected static string adminCapabilitiesKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\SecurityManager\\AdminCapabilities";
	}
}
