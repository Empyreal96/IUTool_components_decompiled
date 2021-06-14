using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x02000007 RID: 7
	[Export(typeof(IPkgPlugin))]
	internal class RegistryKey : PkgPlugin
	{
		// Token: 0x06000012 RID: 18 RVA: 0x00002980 File Offset: 0x00000B80
		public override void ConvertEntries(XElement ToPkg, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromCsi)
		{
			string attributeValue = PkgBldrHelpers.GetAttributeValue(FromCsi, "keyName");
			MyContainter myContainter = (MyContainter)enviorn.arg;
			XElement regKeys = myContainter.RegKeys;
			string text = attributeValue.TrimEnd("\\".ToCharArray());
			text = enviorn.Macros.Resolve(text);
			text = RegHelpers.RegKeyNameToMacro(text);
			if (text == null)
			{
				Console.WriteLine("warning: ignoring key", attributeValue);
				return;
			}
			XElement xelement = new XElement(ToPkg.Name.Namespace + "RegKey");
			xelement.Add(new XAttribute("KeyName", text));
			base.ConvertEntries(xelement, plugins, enviorn, FromCsi);
			if (xelement.Elements().Count<XElement>() > 0)
			{
				Share.MergeNewPkgRegKey(regKeys, xelement);
			}
			XElement xelement2 = FromCsi.Element(FromCsi.Name.Namespace + "securityDescriptor");
			if (xelement2 != null)
			{
				string name = xelement2.Attribute("name").Value.ToUpperInvariant();
				SDDL sddl = myContainter.Security.Lookup(name);
				if (sddl == null)
				{
					Console.WriteLine("error: cant find matching ACE in lookup table");
					return;
				}
				string path = RegHelpers.RegMacroToKeyName(text);
				myContainter.Security.AddRegAce(path, sddl);
			}
		}
	}
}
