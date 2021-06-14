using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x0200000D RID: 13
	[Export(typeof(IPkgPlugin))]
	internal class Language : Identity
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00004DA0 File Offset: 0x00002FA0
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.WM.Xsd\\Common.xsd";
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00004DA8 File Offset: 0x00002FA8
		public override void ConvertEntries(XElement toCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromWm)
		{
			XElement xelement = toCsi.Element(toCsi.Name.Namespace + "assemblyIdentity");
			XAttribute xattribute;
			if (enviorn.build.satellite.Type == SatelliteType.Neutral)
			{
				XElement xelement2 = new XElement(xelement);
				xattribute = xelement2.Attribute("name");
				xattribute.Value += ".Resources";
				xelement2.Attribute("language").Value = "*";
				XElement xelement3 = new XElement(toCsi.Name.Namespace + "dependentAssembly");
				xelement3.Add(new XAttribute("dependencyType", "prerequisite"));
				xelement3.Add(xelement2);
				XElement xelement4 = new XElement(toCsi.Name.Namespace + "dependency");
				xelement4.Add(new XAttribute("discoverable", "false"));
				xelement4.Add(new XAttribute("optional", "false"));
				xelement4.Add(new XAttribute("resourceType", "Resources"));
				xelement4.Add(xelement3);
				toCsi.Add(xelement4);
				string attributeValue = PkgBldrHelpers.GetAttributeValue(fromWm, "buildFilter");
				if (attributeValue != null)
				{
					xelement4.Add(new XAttribute("buildFilter", attributeValue));
				}
				return;
			}
			if (enviorn.build.satellite.Type != SatelliteType.Language)
			{
				return;
			}
			XElement xelement5 = new XElement(xelement);
			xattribute = xelement5.Attribute("name");
			xattribute.Value += ".Resources";
			xelement5.Attribute("language").Value = enviorn.Bld.Lang;
			xelement.Remove();
			toCsi.Add(xelement5);
			string attributeValue2 = PkgBldrHelpers.GetAttributeValue(fromWm, "multilingual");
			if (attributeValue2 != null && attributeValue2.Equals("true", StringComparison.OrdinalIgnoreCase))
			{
				Build.WowType wow = enviorn.build.wow;
				if (wow != Build.WowType.host)
				{
					if (wow == Build.WowType.guest)
					{
						enviorn.Output = Identity.UpdateOutputPath(enviorn.Output, Identity.ManifestType.GuestMultiLang);
					}
				}
				else
				{
					enviorn.Output = Identity.UpdateOutputPath(enviorn.Output, Identity.ManifestType.HostMultiLang);
				}
			}
			base.ProcessDesendents(toCsi, plugins, enviorn, fromWm);
		}
	}
}
