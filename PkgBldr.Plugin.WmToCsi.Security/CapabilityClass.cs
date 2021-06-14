using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins
{
	// Token: 0x02000008 RID: 8
	[Export(typeof(IPkgPlugin))]
	internal class CapabilityClass : Authorization
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002568 File Offset: 0x00000768
		public override string XmlElementName
		{
			get
			{
				return "capabilityClass";
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000256F File Offset: 0x0000076F
		public override void ConvertEntries(XElement authorizationRegKeys, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement component)
		{
			if (environ.Bld.Product.Equals("mobilecore", StringComparison.OrdinalIgnoreCase))
			{
				this.ConvertEntriesForMobileCore(authorizationRegKeys, plugins, environ, component);
				return;
			}
			this.ConvertEntriesForOneCore(authorizationRegKeys, plugins, environ, component);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000025A0 File Offset: 0x000007A0
		private void ConvertEntriesForOneCore(XElement authorizationRegKeys, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement component)
		{
			MacroResolver macros = environ.Macros;
			string value = component.Attribute("name").Value;
			XElement xelement = new XElement(authorizationRegKeys.Name.Namespace + "registryKey");
			xelement.Add(new XAttribute("keyName", Authorization.authorizationRegKeyRoot + CapabilityClass.capabilityClassRegKeyName + value));
			string text = "\"";
			string text2 = null;
			bool flag = true;
			string text3 = null;
			bool flag2 = true;
			foreach (XElement xelement2 in component.Descendants())
			{
				string localName = xelement2.Name.LocalName;
				if (!(localName == "memberCapability"))
				{
					if (localName == "memberCapabilityClass")
					{
						text3 += (flag2 ? null : ",");
						text3 = text3 + text + xelement2.Attribute("name").Value + text;
						flag2 = false;
					}
				}
				else
				{
					text2 += (flag ? null : ",");
					string str = macros.Resolve(xelement2.Attribute("id").Value);
					text2 = text2 + text + str + text;
					flag = false;
				}
			}
			if (text2 != null)
			{
				XElement xelement3 = new XElement(authorizationRegKeys.Name.Namespace + "registryValue");
				xelement3.Add(new XAttribute("name", "MemberCapability"));
				xelement3.Add(new XAttribute("value", text2));
				xelement3.Add(new XAttribute("valueType", "REG_MULTI_SZ"));
				xelement.Add(xelement3);
			}
			if (text3 != null)
			{
				XElement xelement4 = new XElement(authorizationRegKeys.Name.Namespace + "registryValue");
				xelement4.Add(new XAttribute("name", "MemberCapabilityClass"));
				xelement4.Add(new XAttribute("value", text3));
				xelement4.Add(new XAttribute("valueType", "REG_MULTI_SZ"));
				xelement.Add(xelement4);
			}
			authorizationRegKeys.Add(xelement);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002800 File Offset: 0x00000A00
		private void ConvertEntriesForMobileCore(XElement authorizationRegKeys, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement component)
		{
			MacroResolver macros = environ.Macros;
			string value = component.Attribute("name").Value;
			string str = "CAPABILITY_CLASS_";
			string text = value;
			string value2 = str + text.Substring(text.IndexOf('_') + 1).ToUpperInvariant();
			XElement xelement = new XElement(authorizationRegKeys.Name.Namespace + "registryKey");
			xelement.Add(new XAttribute("keyName", Authorization.authorizationRegKeyRoot + CapabilityClass.capabilityClassRegKeyName));
			foreach (XElement xelement2 in component.Descendants())
			{
				string localName = xelement2.Name.LocalName;
				if (localName == "memberCapability")
				{
					XElement xelement3 = new XElement(authorizationRegKeys.Name.Namespace + "registryValue");
					string value3 = SidBuilder.BuildApplicationCapabilitySidString(macros.Resolve(xelement2.Attribute("id").Value));
					xelement3.Add(new XAttribute("name", value3));
					xelement3.Add(new XAttribute("value", value2));
					xelement3.Add(new XAttribute("valueType", "REG_MULTI_SZ"));
					xelement.Add(xelement3);
				}
			}
			authorizationRegKeys.Add(xelement);
		}

		// Token: 0x04000006 RID: 6
		private static string capabilityClassRegKeyName = "CapabilityClasses\\";
	}
}
