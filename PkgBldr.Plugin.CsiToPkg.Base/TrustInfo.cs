using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x02000005 RID: 5
	[Export(typeof(IPkgPlugin))]
	internal class TrustInfo : PkgPlugin
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002562 File Offset: 0x00000762
		public override bool Pass(BuildPass pass)
		{
			return pass == BuildPass.MACRO_PASS;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002874 File Offset: 0x00000A74
		public override void ConvertEntries(XElement ToPkg, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromCsi)
		{
			MyContainter myContainter = (MyContainter)enviorn.arg;
			foreach (XElement element in FromCsi.Descendants(FromCsi.Name.Namespace + "securityDescriptorDefinition"))
			{
				string attributeValue = PkgBldrHelpers.GetAttributeValue(element, "name");
				string text = PkgBldrHelpers.GetAttributeValue(element, "sddl");
				text = enviorn.Macros.Resolve(text);
				if (enviorn.Macros.PassThrough(text))
				{
					Console.WriteLine("error: can't pass through SDDL macro {0}", text);
				}
				else
				{
					SDDL sddl = new SDDL();
					sddl.Owner = SddlHelpers.GetSddlOwner(text);
					sddl.Group = SddlHelpers.GetSddlGroup(text);
					sddl.Dacl = SddlHelpers.GetSddlDacl(text);
					sddl.Sacl = SddlHelpers.GetSddlSacl(text);
					myContainter.Security.AddToLookupTable(attributeValue.ToUpperInvariant(), sddl);
				}
			}
		}
	}
}
