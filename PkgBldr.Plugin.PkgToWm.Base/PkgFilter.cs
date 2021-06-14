using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200001F RID: 31
	[Export(typeof(IPkgPlugin))]
	internal class PkgFilter : PkgPlugin
	{
		// Token: 0x0600005A RID: 90 RVA: 0x00005D24 File Offset: 0x00003F24
		public override void ConvertEntries(XElement toFilteredPkg, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromUnFilteredPkg)
		{
			enviorn.ExitStatus = ExitStatus.SUCCESS;
			if (enviorn.build.wow == Build.WowType.guest)
			{
				PkgFilter.AppendGuestToSubCompName(fromUnFilteredPkg);
			}
			toFilteredPkg.Name = fromUnFilteredPkg.Name;
			toFilteredPkg.Add(fromUnFilteredPkg.Attributes());
			XElement xelement = new XElement(fromUnFilteredPkg);
			enviorn.Bld.PKG.Root = xelement;
			this.DoWowFiltering(xelement, enviorn);
			toFilteredPkg.Attribute("BuildWow").Remove();
			toFilteredPkg.Add(xelement.Elements());
			if (enviorn.build.wow == Build.WowType.guest && toFilteredPkg.Descendants(toFilteredPkg.Name.Namespace + "SettingsGroup").Count<XElement>() > 0)
			{
				throw new PkgGenException("<SettingsGroup> must be filtered out of guest packages using buildFilter=\"not wow\"");
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00005DE8 File Offset: 0x00003FE8
		private void DoWowFiltering(XElement filteredPkg, Config environ)
		{
			PkgFilter.m_filterList.Clear();
			PkgFilter.m_langFilterList.Clear();
			PkgFilter.m_resFilterList.Clear();
			PkgFilter.m_RegKeys.Clear();
			this.PopulateWowFilterList(filteredPkg);
			foreach (XElement xelement in PkgFilter.m_filterList)
			{
				string archAsString = environ.Bld.ArchAsString;
				bool state = false;
				if (environ.build.wow == Build.WowType.guest)
				{
					state = true;
				}
				string attributeValue = PkgBldrHelpers.GetAttributeValue(xelement, "buildFilter");
				BooleanExpressionEvaluator booleanExpressionEvaluator = new BooleanExpressionEvaluator();
				string expressionPattern = "^([archarmx86amd64notandorwow=\\)\\(\\s]+)$";
				booleanExpressionEvaluator.expressionPattern = expressionPattern;
				booleanExpressionEvaluator.Set("arch", true);
				booleanExpressionEvaluator.Set("wow", state);
				booleanExpressionEvaluator.Set(archAsString, true);
				string text = booleanExpressionEvaluator.Evaluate(attributeValue);
				if (text == null)
				{
					throw new PkgGenException("Invalid build filter {0}", new object[]
					{
						attributeValue
					});
				}
				xelement.Attribute("buildFilter").Remove();
				if (text == "false")
				{
					foreach (XElement element in PkgFilter.GetLangResElements(xelement))
					{
						string attributeValue2 = PkgBldrHelpers.GetAttributeValue(element, "Resolution");
						string attributeValue3 = PkgBldrHelpers.GetAttributeValue(element, "Language");
						if (attributeValue2 != null)
						{
							this.AddRootRegResElement(attributeValue2, environ.Bld.PKG.Root);
						}
						if (attributeValue3 != null)
						{
							this.AddRootRegLangElement(attributeValue3, environ.Bld.PKG.Root);
						}
					}
					xelement.Remove();
				}
				this.Prune(filteredPkg);
			}
			if (PkgFilter.m_RegKeys.Count > 0)
			{
				XElement xelement2 = PkgBldrHelpers.AddIfNotFound(PkgBldrHelpers.AddIfNotFound(filteredPkg, "Components"), "OSComponent");
				foreach (XElement content in PkgFilter.m_RegKeys)
				{
					xelement2.Add(content);
				}
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00006034 File Offset: 0x00004234
		private static IEnumerable<XElement> GetLangResElements(XElement element)
		{
			return from ele in element.DescendantsAndSelf(element.Name.Namespace + "Files").Concat(element.DescendantsAndSelf(element.Name.Namespace + "RegKeys"))
			where ele.Attribute("Language") != null || ele.Attribute("Resolution") != null
			select ele;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000060A0 File Offset: 0x000042A0
		private static void AppendGuestToSubCompName(XElement Pkg)
		{
			XAttribute xattribute = Pkg.Attribute("SubComponent");
			if (xattribute == null)
			{
				Pkg.Add(new XAttribute("SubComponent", "Guest"));
				return;
			}
			if (xattribute.Value.Trim().Equals(string.Empty))
			{
				xattribute.Value = "Guest";
				return;
			}
			XAttribute xattribute2 = xattribute;
			xattribute2.Value += ".Guest";
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00006118 File Offset: 0x00004318
		private void AddRootRegLangElement(string languageFilter, XElement pkgRoot)
		{
			if (!PkgFilter.m_langFilterList.Contains(languageFilter.ToLowerInvariant()))
			{
				PkgFilter.m_langFilterList.Add(languageFilter.ToLowerInvariant());
				XElement xelement = new XElement(pkgRoot.Name.Namespace + "RegKeys");
				xelement.Add(new XAttribute("Language", languageFilter));
				PkgFilter.m_RegKeys.Add(xelement);
				XElement xelement2 = new XElement(pkgRoot.Name.Namespace + "RegKey");
				string str = this.GenerateUniqueKey(pkgRoot, languageFilter);
				xelement2.Add(new XAttribute("KeyName", "$(hklm.microsoft)\\PkgGen\\Lang\\$(langid)\\" + str));
				xelement.Add(xelement2);
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000061D4 File Offset: 0x000043D4
		private void AddRootRegResElement(string resFilter, XElement pkgRoot)
		{
			if (!PkgFilter.m_resFilterList.Contains(resFilter.ToLowerInvariant()))
			{
				PkgFilter.m_resFilterList.Add(resFilter.ToLowerInvariant());
				XElement xelement = new XElement(pkgRoot.Name.Namespace + "RegKeys");
				xelement.Add(new XAttribute("Resolution", resFilter));
				PkgFilter.m_RegKeys.Add(xelement);
				XElement xelement2 = new XElement(pkgRoot.Name.Namespace + "RegKey");
				string str = this.GenerateUniqueKey(pkgRoot, resFilter);
				xelement2.Add(new XAttribute("KeyName", "$(hklm.microsoft)\\PkgGen\\Res\\$(resid)\\" + str));
				xelement.Add(xelement2);
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00006290 File Offset: 0x00004490
		private string GenerateUniqueKey(XElement pkgElement, string filter)
		{
			string text = filter;
			foreach (XAttribute xattribute in pkgElement.Attributes())
			{
				string localName = xattribute.Name.LocalName;
				if (localName == "Component" || localName == "Owner" || localName == "OwnerType" || localName == "ReleaseType" || localName == "SubComponent")
				{
					text += xattribute.Value.ToLowerInvariant();
				}
			}
			text = HashCalculator.CalculateSha1Hash(text);
			return text;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00006340 File Offset: 0x00004540
		private void PopulateWowFilterList(XElement parent)
		{
			string localName = parent.Name.LocalName;
			if (PkgBldrHelpers.GetAttributeValue(parent, "buildFilter") != null)
			{
				PkgFilter.m_filterList.Add(parent);
			}
			foreach (XElement parent2 in parent.Elements())
			{
				this.PopulateWowFilterList(parent2);
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000063B4 File Offset: 0x000045B4
		private void Prune(XElement root)
		{
			foreach (XElement root2 in root.Elements())
			{
				this.Prune(root2);
			}
			if (!root.HasElements && !root.HasAttributes)
			{
				root.Remove();
			}
		}

		// Token: 0x0400000A RID: 10
		private static List<XElement> m_filterList = new List<XElement>();

		// Token: 0x0400000B RID: 11
		private static List<string> m_langFilterList = new List<string>();

		// Token: 0x0400000C RID: 12
		private static List<string> m_resFilterList = new List<string>();

		// Token: 0x0400000D RID: 13
		private static List<XElement> m_RegKeys = new List<XElement>();
	}
}
