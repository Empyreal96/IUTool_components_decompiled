using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200000A RID: 10
	[Export(typeof(IPkgPlugin))]
	internal class Files : PkgPlugin
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00002E54 File Offset: 0x00001054
		public override void ConvertEntries(XElement toWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromPkg)
		{
			FileConverter fileConverter = (FileConverter)enviorn.arg;
			string attributeValue = PkgBldrHelpers.GetAttributeValue(fromPkg, "Language");
			if (attributeValue != null)
			{
				if (!attributeValue.Equals("*"))
				{
					enviorn.Logger.LogWarning("setting Language={0} to language=*", new object[]
					{
						attributeValue
					});
				}
				toWm = PkgBldrHelpers.AddIfNotFound(toWm, "language");
				string attributeValue2 = PkgBldrHelpers.GetAttributeValue(fromPkg, "buildFilter");
				if (attributeValue2 != null)
				{
					string value = Helpers.ConvertBuildFilter(attributeValue2);
					toWm.Add(new XAttribute("buildFilter", value));
				}
				fileConverter.IsLangFile = true;
			}
			XElement xelement = new XElement(toWm.Name.Namespace + "files");
			base.ConvertEntries(xelement, plugins, enviorn, fromPkg);
			if (xelement.HasElements)
			{
				string text = Helpers.GenerateWmBuildFilter(fromPkg, enviorn.Logger);
				string text2 = Helpers.GenerateWmResolutionFilter(fromPkg);
				if (text2 != null)
				{
					xelement.Add(new XAttribute("resolution", text2));
				}
				if (text != null)
				{
					if (text2 != null)
					{
						enviorn.Logger.LogWarning("Ignoring buildFilter because it can't be used with Resolution", new object[0]);
					}
					else
					{
						xelement.Add(new XAttribute("buildFilter", text));
					}
				}
				toWm.Add(xelement);
			}
		}
	}
}
