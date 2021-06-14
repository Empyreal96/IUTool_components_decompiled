using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x0200000B RID: 11
	[Export(typeof(IPkgPlugin))]
	internal class Files : PkgPlugin
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00004844 File Offset: 0x00002A44
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromWm)
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
					throw new PkgGenException("Invalid resolution filter {0} on files", new object[]
					{
						text
					});
				}
				if (!flag)
				{
					return;
				}
			}
			base.ConvertEntries(ToCsi, plugins, enviorn, fromWm);
		}
	}
}
