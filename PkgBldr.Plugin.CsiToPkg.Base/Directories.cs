using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x0200000B RID: 11
	[Export(typeof(IPkgPlugin))]
	internal class Directories : PkgPlugin
	{
		// Token: 0x0600001A RID: 26 RVA: 0x00003468 File Offset: 0x00001668
		public override void ConvertEntries(XElement ToPkg, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromCsi)
		{
			MyContainter myContainter = (MyContainter)enviorn.arg;
			foreach (XElement xelement in FromCsi.Elements(FromCsi.Name.Namespace + "directory"))
			{
				string text = PkgBldrHelpers.GetAttributeValue(xelement, "destinationPath");
				text = enviorn.Macros.Resolve(text);
				if (text.StartsWith("$(ERROR)", StringComparison.OrdinalIgnoreCase))
				{
					Console.WriteLine("warning: can't resolve {0}", text);
				}
				else
				{
					XElement xelement2 = xelement.Element(FromCsi.Name.Namespace + "securityDescriptor");
					if (xelement2 != null)
					{
						string attributeValue = PkgBldrHelpers.GetAttributeValue(xelement2, "name");
						SDDL sddl = myContainter.Security.Lookup(attributeValue);
						if (sddl == null)
						{
							Console.WriteLine("error: cant find matching ACE in lookup table");
							break;
						}
						text = myContainter.Security.Macros.Resolve(text);
						myContainter.Security.AddDirAce(text, sddl);
					}
					else
					{
						string sddl2 = enviorn.Macros.Resolve("$(build.wrpDirSddl)");
						SDDL sddl3 = new SDDL();
						sddl3.Owner = SddlHelpers.GetSddlOwner(sddl2);
						sddl3.Group = SddlHelpers.GetSddlGroup(sddl2);
						sddl3.Dacl = SddlHelpers.GetSddlDacl(sddl2);
						sddl3.Sacl = SddlHelpers.GetSddlSacl(sddl2);
						text = myContainter.Security.Macros.Resolve(text);
						myContainter.Security.AddDirAce(text, sddl3);
					}
				}
			}
		}
	}
}
