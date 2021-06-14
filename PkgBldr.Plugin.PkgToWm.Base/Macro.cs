using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200000F RID: 15
	[Export(typeof(IPkgPlugin))]
	internal class Macro : Macros
	{
		// Token: 0x0600002C RID: 44 RVA: 0x000034D4 File Offset: 0x000016D4
		public override void ConvertEntries(XElement toWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromPkg)
		{
			string attributeValue = PkgBldrHelpers.GetAttributeValue(fromPkg, "Id");
			string attributeValue2 = PkgBldrHelpers.GetAttributeValue(fromPkg, "Value");
			XElement xelement = new XElement(toWm.Name.Namespace + "macro");
			xelement.Add(new XAttribute("id", attributeValue));
			xelement.Add(new XAttribute("value", attributeValue2));
			enviorn.Macros.Register(attributeValue, string.Format("$({0})", attributeValue));
			enviorn.PolicyMacros.Register(attributeValue, attributeValue2);
			toWm.Add(xelement);
		}
	}
}
