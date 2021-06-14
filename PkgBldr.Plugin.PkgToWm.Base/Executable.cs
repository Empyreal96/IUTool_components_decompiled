using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000027 RID: 39
	[Export(typeof(IPkgPlugin))]
	internal class Executable : PkgPlugin
	{
		// Token: 0x06000084 RID: 132 RVA: 0x00007834 File Offset: 0x00005A34
		public override void ConvertEntries(XElement toWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromPkg)
		{
			XElement xelement = new XElement(fromPkg);
			XAttribute xattribute = xelement.Attribute("ImagePath");
			XAttribute xattribute2 = xelement.Attribute("BinaryInOneCorePkg");
			if (xattribute2 != null && (xattribute2.Value.Equals("1") || xattribute2.Value.Equals("true")))
			{
				enviorn.Logger.LogWarning("<Executable> Not converted because binary is in OneCore", new object[0]);
				return;
			}
			string attributeValue = PkgBldrHelpers.GetAttributeValue(toWm, "imagePath");
			if (attributeValue != null)
			{
				enviorn.Logger.LogWarning(string.Format("<Executable> Not converted because imagePath is already defined by SvcHostGroupName as {0}", attributeValue), new object[0]);
				return;
			}
			string text = null;
			if (xattribute == null)
			{
				string attributeValue2 = PkgBldrHelpers.GetAttributeValue(xelement, "Name");
				string attributeValue3 = PkgBldrHelpers.GetAttributeValue(xelement, "Source");
				if (attributeValue2 != null)
				{
					text = attributeValue2;
				}
				else if (attributeValue3 != null)
				{
					string[] array = attributeValue3.Split(new char[]
					{
						'\\'
					});
					if (array.Length == 0)
					{
						text = attributeValue3;
					}
					else
					{
						string[] array2 = array;
						text = array2[array2.Length - 1];
					}
				}
				text = "$(runtime.system32)\\" + text;
			}
			else
			{
				text = enviorn.Macros.Resolve(xattribute.Value);
				xattribute.Remove();
			}
			if (text.StartsWith("$(runtime.system32)", StringComparison.InvariantCulture))
			{
				text = text.Replace("$(runtime.system32)", "%SystemRoot%\\System32");
			}
			if (!text.StartsWith("%SystemRoot%\\System32", StringComparison.InvariantCultureIgnoreCase))
			{
				enviorn.Logger.LogWarning(string.Format("<Executable> Not converted because ImagePath does not start with %SystemRoot%\\System32", new object[0]), new object[0]);
				return;
			}
			toWm.Add(new XAttribute("imagePath", text));
			XElement xelement2 = new XElement(toWm.Name.Namespace + "files");
			XElement content = new FileConverter(enviorn).WmFile(toWm.Name.Namespace, xelement);
			xelement2.Add(content);
			toWm.Add(xelement2);
		}
	}
}
