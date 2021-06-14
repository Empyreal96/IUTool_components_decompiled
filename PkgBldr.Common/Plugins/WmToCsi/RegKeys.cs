using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000014 RID: 20
	[Export(typeof(IPkgPlugin))]
	internal class RegKeys : PkgPlugin
	{
		// Token: 0x0600005A RID: 90 RVA: 0x0000607C File Offset: 0x0000427C
		public override void ConvertEntries(XElement toCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromWm)
		{
			string text = PkgBldrHelpers.GetAttributeValue(fromWm, "resolution");
			if (text != null)
			{
				bool flag = false;
				text = text.ToLowerInvariant();
				try
				{
					flag = (text.Equals("*", StringComparison.InvariantCultureIgnoreCase) || enviorn.ExpressionEvaluator.Evaluate(text));
				}
				catch
				{
					throw new PkgGenException("Invalid resolution filter {0} on regKeys", new object[]
					{
						text
					});
				}
				if (!flag)
				{
					return;
				}
			}
			XElement parent = PkgBldrHelpers.AddIfNotFound(toCsi, "registryKeys");
			base.ConvertEntries(parent, plugins, enviorn, fromWm);
		}
	}
}
