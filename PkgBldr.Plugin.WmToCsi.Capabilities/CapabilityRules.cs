using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Base.Security;
using Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins
{
	// Token: 0x02000004 RID: 4
	[Export(typeof(IPkgPlugin))]
	internal class CapabilityRules : Capabilities
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002218 File Offset: 0x00000418
		public override void ConvertEntries(XElement parent, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement component)
		{
			GlobalSecurity globalSecurity = enviorn.GlobalSecurity;
			MacroResolver macros = enviorn.Macros;
			string attributeValue = base.GetAttributeValue(component.Parent, "id");
			bool adminOnMultiSession = macros.Resolve(base.GetAttributeValue(component.Parent, "adminOnMultiSession")) == "Yes";
			foreach (XElement xelement in component.Elements())
			{
				bool flag = false;
				string localName = xelement.Name.LocalName;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(localName);
				if (num <= 2548070366U)
				{
					if (num <= 300321706U)
					{
						if (num != 294729096U)
						{
							if (num == 300321706U)
							{
								if (localName == "wnf")
								{
									string text = base.GetAttributeValue(xelement, "name");
									string attributeValue2 = base.GetAttributeValue(xelement, "tag");
									string attributeValue3 = base.GetAttributeValue(xelement, "scope");
									string attributeValue4 = base.GetAttributeValue(xelement, "sequence");
									string rights = macros.Resolve(base.GetAttributeValue(xelement, "rights"));
									WnfValue wnfValue = new WnfValue(text, attributeValue2, attributeValue3, attributeValue4);
									globalSecurity.AddCapability(attributeValue, wnfValue, rights, adminOnMultiSession);
								}
							}
						}
						else if (localName == "directory")
						{
							string text2 = macros.Resolve(base.GetAttributeValue(xelement, "path"));
							if (text2.StartsWith("$(runtime.userProfile)", StringComparison.OrdinalIgnoreCase))
							{
								flag = true;
							}
							string rights = macros.Resolve(base.GetAttributeValue(xelement, "rights"));
							globalSecurity.AddCapability(attributeValue, text2, ResourceType.Directory, rights, adminOnMultiSession, flag, null);
						}
					}
					else if (num != 1339017052U)
					{
						if (num != 2088392601U)
						{
							if (num == 2548070366U)
							{
								if (localName == "serviceAccess")
								{
									string text = macros.Resolve(base.GetAttributeValue(xelement, "name"));
									string rights = macros.Resolve(base.GetAttributeValue(xelement, "rights"));
									globalSecurity.AddCapability(attributeValue, text, ResourceType.ServiceAccess, rights, adminOnMultiSession);
								}
							}
						}
						else if (localName == "sdRegValue")
						{
							string text2 = macros.Resolve(base.GetAttributeValue(xelement, "path"));
							bool isString = macros.Resolve(base.GetAttributeValue(xelement, "saveAsString")) == "Yes";
							string rights = macros.Resolve(base.GetAttributeValue(xelement, "rights"));
							SdRegValue sdRegValue = new SdRegValue(SdRegType.Generic, text2, null, isString);
							globalSecurity.AddCapability(attributeValue, sdRegValue, ResourceType.SdReg, rights, adminOnMultiSession);
						}
					}
					else if (localName == "regKey")
					{
						string text2 = macros.Resolve(base.GetAttributeValue(xelement, "path"));
						if (text2.StartsWith("HKEY_CURRENT_USER", StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
						}
						string rights = macros.Resolve(base.GetAttributeValue(xelement, "rights"));
						globalSecurity.AddCapability(attributeValue, text2, ResourceType.Registry, rights, adminOnMultiSession, flag, null);
					}
				}
				else if (num <= 3775006546U)
				{
					if (num != 2867484483U)
					{
						if (num == 3775006546U)
						{
							if (localName == "transientObject")
							{
								string text2 = base.GetAttributeValue(xelement, "path");
								string qualifyingType = macros.Resolve(base.GetAttributeValue(xelement, "type"));
								string rights = macros.Resolve(base.GetAttributeValue(xelement, "rights"));
								flag = (macros.Resolve(base.GetAttributeValue(xelement, "protectToUser")) == "Yes");
								SdRegValue sdRegValue2 = new SdRegValue(SdRegType.TransientObject, text2, qualifyingType, flag);
								globalSecurity.AddCapability(attributeValue, sdRegValue2, ResourceType.TransientObject, rights, adminOnMultiSession, flag);
							}
						}
					}
					else if (localName == "file")
					{
						string text2 = macros.Resolve(base.GetAttributeValue(xelement, "path"));
						if (text2.StartsWith("$(runtime.userProfile)", StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
						}
						string rights = macros.Resolve(base.GetAttributeValue(xelement, "rights"));
						globalSecurity.AddCapability(attributeValue, text2, ResourceType.File, rights, adminOnMultiSession, flag, null);
					}
				}
				else if (num != 3950710239U)
				{
					if (num != 4052603614U)
					{
						if (num == 4162518264U)
						{
							if (localName == "etwProvider")
							{
								string attributeValue5 = base.GetAttributeValue(xelement, "guid");
								string rights = macros.Resolve(base.GetAttributeValue(xelement, "rights"));
								SdRegValue sdRegValue3 = new SdRegValue(SdRegType.EtwProvider, attributeValue5);
								globalSecurity.AddCapability(attributeValue, sdRegValue3, ResourceType.EtwProvider, rights, adminOnMultiSession);
							}
						}
					}
					else if (localName == "com")
					{
						string attributeValue6 = base.GetAttributeValue(xelement, "appId");
						string text3 = macros.Resolve(base.GetAttributeValue(xelement, "accessPermission"));
						string text4 = macros.Resolve(base.GetAttributeValue(xelement, "launchPermission"));
						flag = (macros.Resolve(base.GetAttributeValue(xelement, "protectToUser")) == "Yes");
						SdRegValue sdRegValue4 = new SdRegValue(SdRegType.Com, attributeValue6);
						if (text3 != null)
						{
							globalSecurity.AddCapability(attributeValue, sdRegValue4, ResourceType.ComAccess, text3, adminOnMultiSession, flag);
						}
						if (text4 != null)
						{
							globalSecurity.AddCapability(attributeValue, sdRegValue4, ResourceType.ComLaunch, text4, adminOnMultiSession, flag);
						}
					}
				}
				else if (localName == "winRT")
				{
					string attributeValue7 = base.GetAttributeValue(xelement, "serverName");
					string text4 = macros.Resolve(base.GetAttributeValue(xelement, "launchPermission"));
					string text3 = macros.Resolve(base.GetAttributeValue(xelement, "accessPermission"));
					flag = (macros.Resolve(base.GetAttributeValue(xelement, "protectToUser")) == "Yes");
					SdRegValue sdRegValue5 = new SdRegValue(SdRegType.WinRt, attributeValue7);
					if (text4 != null)
					{
						globalSecurity.AddCapability(attributeValue, sdRegValue5, ResourceType.WinRt, text4, adminOnMultiSession, flag);
					}
					else if (text3 != null)
					{
						globalSecurity.AddCapability(attributeValue, sdRegValue5, ResourceType.WinRt, text3, adminOnMultiSession, flag);
					}
				}
			}
		}
	}
}
