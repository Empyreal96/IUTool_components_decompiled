using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins
{
	// Token: 0x02000005 RID: 5
	[Export(typeof(IPkgPlugin))]
	internal class AccountPrivileges : Account
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000E RID: 14 RVA: 0x0000226C File Offset: 0x0000046C
		public override string XmlElementName
		{
			get
			{
				return "accountPrivileges";
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002274 File Offset: 0x00000474
		public override void ConvertEntries(XElement groupTrustee, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement component)
		{
			string text = null;
			bool flag = true;
			foreach (XElement xelement in component.Descendants())
			{
				text += (flag ? null : " ");
				text += xelement.Attribute("name").Value;
				flag = false;
			}
			groupTrustee.Add(new XElement("privileges", text));
		}
	}
}
