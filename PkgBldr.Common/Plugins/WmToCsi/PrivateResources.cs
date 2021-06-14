using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Base.Security;
using Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000013 RID: 19
	[Export(typeof(IPkgPlugin))]
	internal class PrivateResources : PkgPlugin
	{
		// Token: 0x06000057 RID: 87 RVA: 0x00005AEC File Offset: 0x00003CEC
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config environ, XElement FromWm)
		{
			string attributeValue = PkgBldrHelpers.GetAttributeValue(FromWm, "buildFilter");
			if (attributeValue != null && !environ.ExpressionEvaluator.Evaluate(attributeValue))
			{
				return;
			}
			GlobalSecurity globalSecurity = environ.GlobalSecurity;
			MacroResolver macros = environ.Macros;
			string text = this.GetAttributeValue(FromWm.Parent, "name");
			PrivateResourceClaimerType resourceClaimerType;
			if (text != null)
			{
				resourceClaimerType = PrivateResourceClaimerType.Service;
			}
			else
			{
				text = FromWm.Parent.Element(FromWm.Parent.Name.Namespace + "PackageId").Value;
				if (text == null)
				{
					throw new PkgGenException("Private resources can only be specified for services and tasks");
				}
				resourceClaimerType = PrivateResourceClaimerType.Task;
			}
			foreach (XElement xelement in FromWm.Elements())
			{
				bool readOnly = macros.Resolve(this.GetAttributeValue(xelement, "readOnly")) == "Yes";
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
									string text2 = this.GetAttributeValue(xelement, "name");
									string attributeValue2 = this.GetAttributeValue(xelement, "tag");
									string attributeValue3 = this.GetAttributeValue(xelement, "scope");
									string attributeValue4 = this.GetAttributeValue(xelement, "sequence");
									WnfValue wnfValue = new WnfValue(text2, attributeValue2, attributeValue3, attributeValue4);
									globalSecurity.AddPrivateResource(wnfValue, text, resourceClaimerType, readOnly);
								}
							}
						}
						else if (localName == "directory")
						{
							string text3 = macros.Resolve(this.GetAttributeValue(xelement, "path"));
							if (text3.StartsWith("$(runtime.userProfile)", StringComparison.OrdinalIgnoreCase))
							{
								flag = true;
							}
							globalSecurity.AddPrivateResource(text3, ResourceType.Directory, text, resourceClaimerType, readOnly, flag);
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
									string text2 = macros.Resolve(this.GetAttributeValue(xelement, "name"));
									globalSecurity.AddPrivateResource(text2, ResourceType.ServiceAccess, text, resourceClaimerType, readOnly);
								}
							}
						}
						else if (localName == "sdRegValue")
						{
							string text3 = macros.Resolve(this.GetAttributeValue(xelement, "path"));
							bool isString = macros.Resolve(this.GetAttributeValue(xelement, "saveAsString")) == "Yes";
							SdRegValue sdRegValue = new SdRegValue(SdRegType.Generic, text3, null, isString);
							globalSecurity.AddPrivateResource(sdRegValue, ResourceType.SdReg, text, resourceClaimerType, readOnly);
						}
					}
					else if (localName == "regKey")
					{
						string text3 = macros.Resolve(this.GetAttributeValue(xelement, "path"));
						if (text3.StartsWith("HKEY_CURRENT_USER", StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
						}
						globalSecurity.AddPrivateResource(text3, ResourceType.Registry, text, resourceClaimerType, readOnly, flag);
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
								string text3 = this.GetAttributeValue(xelement, "path");
								string qualifyingType = macros.Resolve(this.GetAttributeValue(xelement, "type"));
								flag = (macros.Resolve(this.GetAttributeValue(xelement, "protectToUser")) == "Yes");
								SdRegValue sdRegValue2 = new SdRegValue(SdRegType.TransientObject, text3, qualifyingType, flag);
								globalSecurity.AddPrivateResource(sdRegValue2, ResourceType.TransientObject, text, resourceClaimerType, readOnly, flag);
							}
						}
					}
					else if (localName == "file")
					{
						string text3 = macros.Resolve(this.GetAttributeValue(xelement, "path"));
						if (text3.StartsWith("$(runtime.userProfile)", StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
						}
						globalSecurity.AddPrivateResource(text3, ResourceType.File, text, resourceClaimerType, readOnly, flag);
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
								string attributeValue5 = this.GetAttributeValue(xelement, "guid");
								SdRegValue sdRegValue3 = new SdRegValue(SdRegType.EtwProvider, attributeValue5);
								globalSecurity.AddPrivateResource(sdRegValue3, ResourceType.EtwProvider, text, resourceClaimerType, readOnly);
							}
						}
					}
					else if (localName == "com")
					{
						string attributeValue6 = this.GetAttributeValue(xelement, "appId");
						flag = (macros.Resolve(this.GetAttributeValue(xelement, "protectToUser")) == "Yes");
						SdRegValue sdRegValue4 = new SdRegValue(SdRegType.Com, attributeValue6);
						globalSecurity.AddPrivateResource(sdRegValue4, ResourceType.ComAccess, text, resourceClaimerType, readOnly, flag);
						globalSecurity.AddPrivateResource(sdRegValue4, ResourceType.ComLaunch, text, resourceClaimerType, readOnly, flag);
					}
				}
				else if (localName == "winRT")
				{
					string attributeValue7 = this.GetAttributeValue(xelement, "serverName");
					flag = (macros.Resolve(this.GetAttributeValue(xelement, "protectToUser")) == "Yes");
					SdRegValue sdRegValue5 = new SdRegValue(SdRegType.WinRt, attributeValue7);
					globalSecurity.AddPrivateResource(sdRegValue5, ResourceType.WinRt, text, resourceClaimerType, readOnly, flag);
				}
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00006050 File Offset: 0x00004250
		private string GetAttributeValue(XElement element, string attributeName)
		{
			XAttribute xattribute = element.Attribute(attributeName);
			string result;
			if (xattribute == null)
			{
				result = null;
			}
			else
			{
				result = xattribute.Value;
			}
			return result;
		}
	}
}
