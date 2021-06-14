using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins
{
	// Token: 0x02000004 RID: 4
	[Export(typeof(IPkgPlugin))]
	internal class AccountCapabilities : Account
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000021C3 File Offset: 0x000003C3
		public override string XmlElementName
		{
			get
			{
				return "accountCapabilities";
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000021CC File Offset: 0x000003CC
		public override void ConvertEntries(XElement groupTrustee, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement component)
		{
			string text = null;
			bool flag = true;
			foreach (XElement xelement in component.Descendants())
			{
				text += (flag ? null : " ");
				text += SidBuilder.BuildServiceCapabilitySidString(xelement.Attribute("id").Value);
				flag = false;
			}
			groupTrustee.Add(new XElement("capabilities", text));
		}
	}
}
