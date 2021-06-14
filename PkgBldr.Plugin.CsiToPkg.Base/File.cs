using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x02000004 RID: 4
	[Export(typeof(IPkgPlugin))]
	internal class File : PkgPlugin
	{
		// Token: 0x0600000B RID: 11 RVA: 0x00002670 File Offset: 0x00000870
		public override void ConvertEntries(XElement ToPkg, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromCsi)
		{
			MyContainter myContainter = (MyContainter)enviorn.arg;
			XElement files = myContainter.Files;
			XNamespace @namespace = ToPkg.Name.Namespace;
			string attributeValue = PkgBldrHelpers.GetAttributeValue(FromCsi, "name");
			string attributeValue2 = PkgBldrHelpers.GetAttributeValue(FromCsi, "destinationPath");
			string attributeValue3 = PkgBldrHelpers.GetAttributeValue(FromCsi, "sourceName");
			PkgBldrHelpers.GetAttributeValue(FromCsi, "sourcePath");
			string attributeValue4 = PkgBldrHelpers.GetAttributeValue(FromCsi, "importPath");
			PkgBldrHelpers.GetAttributeValue(FromCsi, "buildType");
			XElement xelement = new XElement(files.Name.Namespace + "File");
			string text = attributeValue4.TrimEnd("\\".ToCharArray()) + "\\" + attributeValue3;
			text = enviorn.Macros.Resolve(text);
			xelement.Add(new XAttribute("Source", text));
			string text2 = null;
			string text3 = null;
			if (attributeValue2 != null)
			{
				text2 = attributeValue2.TrimEnd("\\".ToCharArray());
				text2 = enviorn.Macros.Resolve(text2);
				if (text2.StartsWith("$(ERROR)", StringComparison.OrdinalIgnoreCase))
				{
					Console.WriteLine("warning: can't resolve {0}", attributeValue2);
					return;
				}
				xelement.Add(new XAttribute("DestinationDir", text2));
			}
			if (attributeValue != null)
			{
				text3 = attributeValue.TrimEnd("\\".ToCharArray());
				xelement.Add(new XAttribute("Name", text3));
			}
			files.Add(xelement);
			XElement firstDecendant = PkgBldrHelpers.GetFirstDecendant(FromCsi, FromCsi.Name.Namespace + "securityDescriptor");
			if (firstDecendant != null)
			{
				string name = firstDecendant.Attribute("name").Value.ToUpperInvariant();
				SDDL sddl = myContainter.Security.Lookup(name);
				if (sddl == null)
				{
					Console.WriteLine("error: cant find matching ACE in lookup table");
					return;
				}
				text2 = myContainter.Security.Macros.Resolve(text2);
				string path = (text2 + "\\" + text3).ToUpperInvariant();
				myContainter.Security.AddFileAce(path, sddl);
			}
		}
	}
}
