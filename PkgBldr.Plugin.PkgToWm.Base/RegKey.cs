using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000022 RID: 34
	[Export(typeof(IPkgPlugin))]
	internal class RegKey : PkgPlugin
	{
		// Token: 0x06000075 RID: 117 RVA: 0x00006868 File Offset: 0x00004A68
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			this.m_logger = enviorn.Logger;
			string text = enviorn.Macros.Resolve(PkgBldrHelpers.GetAttributeValue(FromPkg, "KeyName"));
			string attributeValue = PkgBldrHelpers.GetAttributeValue(FromPkg, "buildFilter");
			string attributeValue2 = PkgBldrHelpers.GetAttributeValue(FromPkg.Parent, "buildFilter");
			if (!Regex.IsMatch(text, ".+\\\\WINEVT\\\\Publishers\\\\\\{.+\\}$", RegexOptions.IgnoreCase))
			{
				XElement xelement = new XElement(ToWm.Name.Namespace + "regKey");
				if (attributeValue != null)
				{
					string value = Helpers.ConvertBuildFilter(attributeValue);
					xelement.Add(new XAttribute("buildFilter", value));
				}
				xelement.Add(new XAttribute("keyName", text));
				this.AddTrustIfNeeded(text, xelement, enviorn.Bld.WM.Root);
				ToWm.Add(xelement);
				base.ConvertEntries(xelement, plugins, enviorn, FromPkg);
				return;
			}
			if (attributeValue != null || attributeValue2 != null)
			{
				string arg = attributeValue;
				if (attributeValue2 != null)
				{
					arg = attributeValue2;
				}
				this.m_logger.LogWarning("Can't use buildFilter on ETW intrumentation elements until wm.xml build filter pre-proccessing is supported", new object[0]);
				this.m_logger.LogWarning(string.Format("Removing the buildFilter {0} from {1}", arg, text), new object[0]);
			}
			string value2 = Regex.Match(text, "\\{.+\\}$").Value;
			FromPkg.Descendants(FromPkg.Name.Namespace + "RegValue");
			string text2 = null;
			string text3 = null;
			XElement xelement2 = PkgBldrHelpers.FindMatchingAttribute(FromPkg, "RegValue", "Name", "@");
			if (xelement2 == null)
			{
				enviorn.Logger.LogWarning(string.Format("Can't convert {0}, provider name is required", text), new object[0]);
				return;
			}
			string value3 = xelement2.Attribute("Value").Value;
			XElement xelement3 = PkgBldrHelpers.FindMatchingAttribute(FromPkg, "RegValue", "Name", "ResourceFileName");
			if (xelement3 != null)
			{
				text2 = xelement3.Attribute("Value").Value;
				text2 = this.RemoveAndReplaceBuildMacros(text2, enviorn);
			}
			XElement xelement4 = PkgBldrHelpers.FindMatchingAttribute(FromPkg, "RegValue", "Name", "MessageFileName");
			if (xelement4 != null)
			{
				text3 = xelement4.Attribute("Value").Value;
				text3 = this.RemoveAndReplaceBuildMacros(text3, enviorn);
			}
			"http://manifests.microsoft.com/win/2004/08/windows/events";
			XElement xelement5 = XElement.Parse(string.Format(CultureInfo.InvariantCulture, "\r\n                    <instrumentation\r\n                        xmlns:win=\"http://manifests.microsoft.com/win/2004/08/windows/events\"\r\n                        xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n                        xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                      >\r\n                      <events xmlns=\"http://schemas.microsoft.com/win/2004/08/events\">\r\n                        <provider>\r\n                          <channels/>\r\n                        </provider>\r\n                      </events>\r\n                    </instrumentation>", new object[0]));
			XElement xelement6 = xelement5.Elements().First<XElement>().Elements().First<XElement>();
			xelement6.Add(new XAttribute("name", value3));
			xelement6.Add(new XAttribute("guid", value2));
			xelement6.Add(new XAttribute("symbol", value3.ToUpperInvariant()));
			if (text2 != null)
			{
				xelement6.Add(new XAttribute("resourceFileName", text2));
			}
			if (text3 != null)
			{
				xelement6.Add(new XAttribute("messageFileName", text3));
			}
			PkgBldrHelpers.ReplaceDefaultNameSpace(ref xelement5, xelement5.Name.Namespace, ToWm.Name.Namespace);
			enviorn.Bld.WM.Root.Add(xelement5);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00006B90 File Offset: 0x00004D90
		private string RemoveAndReplaceBuildMacros(string etwFileName, Config enviorn)
		{
			bool flag = false;
			string text = null;
			etwFileName = enviorn.Macros.Resolve(etwFileName);
			if (!etwFileName.StartsWith("$", StringComparison.InvariantCultureIgnoreCase))
			{
				return etwFileName;
			}
			string[] array = etwFileName.Split(new char[]
			{
				'\\'
			});
			if (array.Length != 0)
			{
				text = array[0] + "\\";
				string a = text.ToLowerInvariant();
				if (!(a == "$(runtime.system32)\\"))
				{
					if (!(a == "$(runtime.bootdrive)\\"))
					{
						if (!(a == "$(runtime.drivers)\\"))
						{
							flag = true;
						}
						else
						{
							etwFileName = etwFileName.Replace(text, "%systemroot%\\system32\\drivers\\");
						}
					}
					else
					{
						etwFileName = etwFileName.Replace(text, "%systemdrive%\\");
					}
				}
				else
				{
					etwFileName = etwFileName.Replace(text, "%systemroot%\\system32\\");
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				this.m_logger.LogWarning(string.Format("Can't convert ETW messageFilename and/or resourceFileName from {0} to %systemroot%\\system32\\", etwFileName), new object[0]);
				if (text != null)
				{
					this.m_logger.LogWarning(string.Format("Replacing {0} with ****TBD**** in the output wm.xml", text), new object[0]);
					etwFileName = etwFileName.Replace(text, "****TBD****\\");
				}
			}
			return etwFileName;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00006C98 File Offset: 0x00004E98
		private void AddTrustIfNeeded(string keyName, XElement wmRegKey, XElement root)
		{
			bool flag = false;
			if (keyName.ToLowerInvariant().Contains("$(hkcr.root)\\interface\\"))
			{
				flag = true;
			}
			if (keyName.ToLowerInvariant().Contains("$(hkcr.classes)"))
			{
				flag = true;
			}
			if (keyName.ToLowerInvariant().Contains("$(hklm.software)\\classes"))
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			XElement xelement = PkgBldrHelpers.AddIfNotFound(root, "trustInfo");
			if ((from e in xelement.Descendants()
			where this.RegDefaultSddl(e)
			select e).Count<XElement>() == 0)
			{
				XElement xelement2 = PkgBldrHelpers.AddIfNotFound(PkgBldrHelpers.AddIfNotFound(PkgBldrHelpers.AddIfNotFound(PkgBldrHelpers.AddIfNotFound(xelement, "security"), "accessControl"), "securityDescriptorDefinitions"), "securityDescriptorDefinition");
				xelement2.Add(new XAttribute("name", "WRP_REGKEY_DEFAULT_SDDL"));
				xelement2.Add(new XAttribute("sddl", "$(build.wrpRegKeySddl)"));
			}
			wmRegKey.Add(new XAttribute("securityDescriptor", "WRP_REGKEY_DEFAULT_SDDL"));
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00006D87 File Offset: 0x00004F87
		private bool RegDefaultSddl(XElement e)
		{
			return e.Name.LocalName.Equals("securityDescriptorDefinition") && PkgBldrHelpers.GetAttributeValue(e, "name") == "WRP_REGKEY_DEFAULT_SDDL";
		}

		// Token: 0x04000014 RID: 20
		private IDeploymentLogger m_logger;
	}
}
