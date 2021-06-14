using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using BuildFilterExpressionEvaluator;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x0200000E RID: 14
	[Export(typeof(IPkgPlugin))]
	public class Identity : PkgPlugin
	{
		// Token: 0x0600003F RID: 63 RVA: 0x00004FFC File Offset: 0x000031FC
		public static string WmIdentityNameToCsiAssemblyName(string owner, string nameSpace, string name, string legacyName)
		{
			string text;
			if (legacyName == null)
			{
				text = owner;
				if (nameSpace != null)
				{
					text = text + "-" + nameSpace;
				}
				text = text + "-" + name;
			}
			else
			{
				text = legacyName;
			}
			return text;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00005032 File Offset: 0x00003232
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.WM.Xsd\\Common.xsd";
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000503C File Offset: 0x0000323C
		protected static string UpdateOutputPath(string outputPath, Identity.ManifestType manType)
		{
			string directoryName = LongPath.GetDirectoryName(outputPath);
			string text = LongPath.GetFileName(outputPath);
			if (text[2].Equals('_'))
			{
				text = text.Remove(0, 3);
			}
			text = Identity._table[manType] + text;
			switch (manType)
			{
			case Identity.ManifestType.HostLang:
			case Identity.ManifestType.GuestLang:
			case Identity.ManifestType.HostMultiLang:
			case Identity.ManifestType.GuestMultiLang:
				if (!text.EndsWith(".Resources.man", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Replace(".man", ".Resources.man");
				}
				break;
			}
			return LongPath.Combine(directoryName, text);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000050C8 File Offset: 0x000032C8
		private static void DoResolutionFiltering(XElement fromWm, Config enviorn)
		{
			bool flag = enviorn.build.satellite.Type == SatelliteType.Resolution;
			foreach (XElement xelement in fromWm.Elements().ToList<XElement>())
			{
				if (xelement.Name.LocalName == "regKeys" || xelement.Name.LocalName == "files")
				{
					if (PkgBldrHelpers.GetAttributeValue(xelement, "resolution") != null)
					{
						if (!flag)
						{
							xelement.Remove();
						}
					}
					else if (flag)
					{
						xelement.Remove();
					}
				}
				else if (flag)
				{
					xelement.Remove();
				}
			}
			if (flag && !fromWm.HasElements)
			{
				enviorn.ExitStatus = ExitStatus.SKIPPED;
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x0000519C File Offset: 0x0000339C
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			enviorn.ExitStatus = ExitStatus.SUCCESS;
			Identity.DoResolutionFiltering(FromWm, enviorn);
			if (enviorn.ExitStatus == ExitStatus.SKIPPED)
			{
				return;
			}
			enviorn.ExpressionEvaluator = new BuildFilterExpressionEvaluator();
			if (enviorn.build.satellite.Type != SatelliteType.Resolution)
			{
				enviorn.ExpressionEvaluator.SetVariable("build.arch", enviorn.Bld.ArchAsString);
				enviorn.ExpressionEvaluator.SetVariable("build.product", enviorn.Bld.Product);
				enviorn.ExpressionEvaluator.SetVariable("build.isWow", enviorn.build.wow == Build.WowType.guest);
			}
			else if (enviorn.Bld.Resolution != null)
			{
				foreach (SatelliteId satelliteId in enviorn.Bld.AllResolutions)
				{
					enviorn.ExpressionEvaluator.SetVariable(satelliteId.Id, false);
				}
				enviorn.ExpressionEvaluator.SetVariable(enviorn.Bld.Resolution, true);
			}
			this.ConvertWmXmlBuildFiltersToCsiFormat(FromWm);
			enviorn.Macros = new MacroResolver();
			enviorn.Macros.Load(XmlReader.Create(PkgGenResources.GetResourceStream("Macros_WmToCsi.xml")));
			if (enviorn.build.satellite.Type == SatelliteType.Resolution)
			{
				enviorn.Macros.Register("resId", enviorn.Bld.Resolution);
			}
			if (enviorn.Bld.BuildMacros != null)
			{
				Dictionary<string, Macro> macroTable = enviorn.Bld.BuildMacros.GetMacroTable();
				enviorn.Macros.Register(macroTable, true);
			}
			if (enviorn.Bld.Lang != "neutral")
			{
				if (enviorn.Bld.Lang == "*")
				{
					enviorn.Bld.Lang = "en-us";
				}
				if (FromWm.Element(FromWm.Name.Namespace + "language") == null)
				{
					enviorn.ExitStatus = ExitStatus.SKIPPED;
					return;
				}
			}
			if (enviorn.build.wow == Build.WowType.guest)
			{
				string attributeValue = PkgBldrHelpers.GetAttributeValue(FromWm, "buildWow");
				if (attributeValue == null)
				{
					enviorn.ExitStatus = ExitStatus.SKIPPED;
					return;
				}
				if (!attributeValue.Equals("true", StringComparison.OrdinalIgnoreCase))
				{
					enviorn.ExitStatus = ExitStatus.SKIPPED;
					return;
				}
			}
			XNamespace ns = "urn:schemas-microsoft-com:asm.v3";
			ToCsi.Name = ns + ToCsi.Name.LocalName;
			ToCsi.Add(new XAttribute("xmlns", "urn:schemas-microsoft-com:asm.v3"));
			ToCsi.Add(new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"));
			ToCsi.Add(new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"));
			ToCsi.Add(new XAttribute("manifestVersion", "1.0"));
			enviorn.Bld.CSI.Root = ToCsi;
			enviorn.Bld.WM.Root = FromWm;
			XElement xelement = new XElement(ToCsi.Name.Namespace + "assemblyIdentity");
			string value = "$(build.buildType)";
			string value2 = "$(build.arch)";
			string value3 = "$(build.WindowsPublicKeyToken)";
			string value4 = "$(build.version)";
			string attributeValue2 = PkgBldrHelpers.GetAttributeValue(FromWm, "owner");
			string attributeValue3 = PkgBldrHelpers.GetAttributeValue(FromWm, "namespace");
			string attributeValue4 = PkgBldrHelpers.GetAttributeValue(FromWm, "name");
			string attributeValue5 = PkgBldrHelpers.GetAttributeValue(FromWm, "legacyName");
			string text = Identity.WmIdentityNameToCsiAssemblyName(attributeValue2, attributeValue3, attributeValue4, attributeValue5);
			if (enviorn.build.satellite.Type == SatelliteType.Resolution)
			{
				text = text + "_" + enviorn.build.satellite.FileSuffix;
			}
			enviorn.Bld.CSI.Name = text;
			xelement.Add(new XAttribute("name", text));
			xelement.Add(new XAttribute("language", "neutral"));
			xelement.Add(new XAttribute("buildType", value));
			xelement.Add(new XAttribute("processorArchitecture", value2));
			xelement.Add(new XAttribute("publicKeyToken", value3));
			xelement.Add(new XAttribute("version", value4));
			xelement.Add(new XAttribute("versionScope", "nonSxS"));
			if (enviorn.AutoGenerateOutput)
			{
				enviorn.Output = LongPath.Combine(enviorn.Output, text + ".man");
			}
			Build.WowType wow = enviorn.build.wow;
			if (wow != Build.WowType.host)
			{
				if (wow == Build.WowType.guest)
				{
					SatelliteType type = enviorn.build.satellite.Type;
					if (type != SatelliteType.Neutral)
					{
						if (type == SatelliteType.Language)
						{
							enviorn.Output = Identity.UpdateOutputPath(enviorn.Output, Identity.ManifestType.GuestLang);
						}
					}
					else
					{
						enviorn.Output = Identity.UpdateOutputPath(enviorn.Output, Identity.ManifestType.GuestNeutral);
					}
				}
			}
			else
			{
				switch (enviorn.build.satellite.Type)
				{
				case SatelliteType.Neutral:
				case SatelliteType.Resolution:
					enviorn.Output = Identity.UpdateOutputPath(enviorn.Output, Identity.ManifestType.HostNeutral);
					break;
				case SatelliteType.Language:
					enviorn.Output = Identity.UpdateOutputPath(enviorn.Output, Identity.ManifestType.HostLang);
					break;
				}
			}
			if (enviorn.Bld.Product != "windows")
			{
				xelement.Add(new XAttribute("product", "$(build.product)"));
			}
			ToCsi.Add(xelement);
			if (enviorn.build.satellite.Type == SatelliteType.Language)
			{
				this.RemoveNeutralContent(enviorn.Bld.WM.Root);
			}
			foreach (BuildPass pass in (BuildPass[])Enum.GetValues(typeof(BuildPass)))
			{
				enviorn.Pass = pass;
				this.ProcessDesendents(ToCsi, plugins, enviorn, FromWm);
			}
			if (enviorn.build.satellite.Type == SatelliteType.Resolution && ToCsi.Elements().Count<XElement>() == 1)
			{
				enviorn.ExitStatus = ExitStatus.SKIPPED;
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000057A4 File Offset: 0x000039A4
		protected void ProcessDesendents(XElement toCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromWm)
		{
			base.ConvertEntries(toCsi, plugins, enviorn, fromWm);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000057B4 File Offset: 0x000039B4
		private void RemoveNeutralContent(XElement wmRoot)
		{
			List<XElement> list = new List<XElement>();
			foreach (XElement xelement in wmRoot.Elements())
			{
				string localName = xelement.Name.LocalName;
				if (!(localName == "macros") && !(localName == "language"))
				{
					list.Add(xelement);
				}
			}
			foreach (XElement xelement2 in list)
			{
				xelement2.Remove();
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00005868 File Offset: 0x00003A68
		private void ConvertWmXmlBuildFiltersToCsiFormat(XElement windowsManifest)
		{
			foreach (XElement xelement in from el in windowsManifest.Descendants()
			where el.Attribute("buildFilter") != null
			select el)
			{
				string value = xelement.Attribute("buildFilter").Value;
				xelement.Attribute("buildFilter").Value = Identity.ConvertBuildFilterToCSI(value);
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00005904 File Offset: 0x00003B04
		private static string ConvertBuildFilterToCSI(string wmBuildFilter)
		{
			string text = wmBuildFilter;
			foreach (object obj in new Regex("\\([^\\)]+\\)", RegexOptions.IgnoreCase).Matches(text))
			{
				Match match = (Match)obj;
				if (!match.Value.ToLowerInvariant().Contains(" or ") && !match.Value.ToLowerInvariant().Contains(" and "))
				{
					string newValue = match.Value.Trim("()".ToCharArray());
					text = text.Replace(match.Value, newValue);
				}
			}
			return text;
		}

		// Token: 0x04000013 RID: 19
		protected static Dictionary<Identity.ManifestType, string> _table = new Dictionary<Identity.ManifestType, string>
		{
			{
				Identity.ManifestType.GuestLang,
				"GL_"
			},
			{
				Identity.ManifestType.GuestMultiLang,
				"GM_"
			},
			{
				Identity.ManifestType.GuestNeutral,
				"GN_"
			},
			{
				Identity.ManifestType.HostLang,
				"HL_"
			},
			{
				Identity.ManifestType.HostMultiLang,
				"HM_"
			},
			{
				Identity.ManifestType.HostNeutral,
				"HN_"
			}
		};

		// Token: 0x02000060 RID: 96
		protected enum ManifestType
		{
			// Token: 0x04000154 RID: 340
			HostNeutral,
			// Token: 0x04000155 RID: 341
			GuestNeutral,
			// Token: 0x04000156 RID: 342
			HostLang,
			// Token: 0x04000157 RID: 343
			GuestLang,
			// Token: 0x04000158 RID: 344
			HostMultiLang,
			// Token: 0x04000159 RID: 345
			GuestMultiLang
		}
	}
}
