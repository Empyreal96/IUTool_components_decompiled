using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000004 RID: 4
	[Export(typeof(IPkgPlugin))]
	internal class CapabilityRules : PkgPlugin
	{
		// Token: 0x06000005 RID: 5 RVA: 0x0000222C File Offset: 0x0000042C
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = new XElement(ToWm.Name.Namespace + "capabilityRules");
			foreach (XElement pkgCapRule in FromPkg.Elements())
			{
				XElement xelement2 = this.ConvertCapRule(xelement.Name.Namespace, pkgCapRule, enviorn.Macros, ref enviorn.ExitStatus, enviorn.Logger);
				if (xelement2 != null)
				{
					xelement.Add(xelement2);
				}
			}
			if (xelement.HasElements)
			{
				ToWm.Add(xelement);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000022CC File Offset: 0x000004CC
		private XElement ConvertCapRule(XNamespace wmNamespace, XElement PkgCapRule, MacroResolver macros, ref ExitStatus exitStatus, IDeploymentLogger logger)
		{
			string localName = PkgCapRule.Name.LocalName;
			string localName2 = PkgCapRule.Name.LocalName;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(localName2);
			if (num <= 1114269721U)
			{
				if (num <= 723007075U)
				{
					if (num != 439927768U)
					{
						if (num == 723007075U)
						{
							if (localName2 == "File")
							{
								localName = "file";
								goto IL_1A5;
							}
						}
					}
					else if (localName2 == "ETWProvider")
					{
						localName = "etwProvider";
						goto IL_1A5;
					}
				}
				else if (num != 879225342U)
				{
					if (num == 1114269721U)
					{
						if (localName2 == "SDRegValue")
						{
							localName = "sdRegValue";
							goto IL_1A5;
						}
					}
				}
				else if (localName2 == "COM")
				{
					localName = "com";
					goto IL_1A5;
				}
			}
			else if (num <= 1868227890U)
			{
				if (num != 1411464970U)
				{
					if (num == 1868227890U)
					{
						if (localName2 == "TransientObject")
						{
							localName = "transientObject";
							goto IL_1A5;
						}
					}
				}
				else if (localName2 == "WNF")
				{
					localName = "wnf";
					goto IL_1A5;
				}
			}
			else if (num != 2485283368U)
			{
				if (num != 3503166012U)
				{
					if (num == 4128555198U)
					{
						if (localName2 == "ServiceAccess")
						{
							localName = "serviceAccess";
							goto IL_1A5;
						}
					}
				}
				else if (localName2 == "RegKey")
				{
					localName = "regKey";
					goto IL_1A5;
				}
			}
			else if (localName2 == "Directory")
			{
				localName = "directory";
				goto IL_1A5;
			}
			logger.LogWarning("<CapabilityRule> {0} not converted", new object[]
			{
				PkgCapRule.Name.LocalName
			});
			return null;
			IL_1A5:
			XElement xelement = new XElement(wmNamespace + localName);
			foreach (XAttribute xattribute in PkgCapRule.Attributes())
			{
				if (xattribute.Name.LocalName.Equals("Path"))
				{
					xattribute.Value = macros.Resolve(xattribute.Value);
				}
				string localName3 = xattribute.Name.LocalName;
				XAttribute content = new XAttribute(Helpers.lowerCamel(xattribute.Name.LocalName), xattribute.Value);
				xelement.Add(content);
			}
			return xelement;
		}
	}
}
