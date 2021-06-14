using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins
{
	// Token: 0x02000007 RID: 7
	[Export(typeof(IPkgPlugin))]
	internal class PrincipalClass : Authorization
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00002368 File Offset: 0x00000568
		public override string XmlElementName
		{
			get
			{
				return "principalClass";
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002370 File Offset: 0x00000570
		public override void ConvertEntries(XElement authorizationRegKeys, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement component)
		{
			string value = component.Attribute("name").Value;
			string text = Authorization.authorizationRegKeyRoot + PrincipalClass.principalClassRegKeyName + value;
			uint num = 1U;
			XElement xelement = new XElement(authorizationRegKeys.Name.Namespace + "registryKey");
			xelement.Add(new XAttribute("keyName", text));
			foreach (XElement xelement2 in component.Descendants().Descendants<XElement>())
			{
				XElement xelement3 = new XElement(authorizationRegKeys.Name.Namespace + "registryKey");
				xelement3.Add(new XAttribute("keyName", text + "\\Certificate" + num.ToString(CultureInfo.InvariantCulture)));
				foreach (XAttribute xattribute in xelement2.Attributes())
				{
					XElement xelement4 = new XElement(authorizationRegKeys.Name.Namespace + "registryValue");
					xelement4.Add(new XAttribute("name", xattribute.Name.ToString()));
					xelement4.Add(new XAttribute("value", xattribute.Value));
					xelement4.Add(new XAttribute("valueType", "REG_SZ"));
					xelement3.Add(xelement4);
				}
				authorizationRegKeys.Add(xelement3);
				num += 1U;
			}
			authorizationRegKeys.Add(xelement);
		}

		// Token: 0x04000005 RID: 5
		private static string principalClassRegKeyName = "PrincipalClasses\\";
	}
}
