using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000006 RID: 6
	[Export(typeof(IPkgPlugin))]
	internal class Dll : PkgPlugin
	{
		// Token: 0x0600000B RID: 11 RVA: 0x00002990 File Offset: 0x00000B90
		public override void ConvertEntries(XElement toWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromPkg)
		{
			ComData comData = (ComData)enviorn.arg;
			XElement xelement = new FileConverter(enviorn).WmFile(toWm.Name.Namespace, fromPkg);
			comData.Files.Add(xelement);
			string attributeValue = PkgBldrHelpers.GetAttributeValue(xelement, "destinationDir");
			string text = PkgBldrHelpers.GetAttributeValue(xelement, "name");
			string attributeValue2 = PkgBldrHelpers.GetAttributeValue(xelement, "source");
			if (attributeValue == null)
			{
				throw new PkgGenException("a destination dir is required for a COM inProcServer DLL");
			}
			if (text == null)
			{
				text = LongPath.GetFileName(attributeValue2);
			}
			comData.InProcServer.Add(new XAttribute("path", attributeValue + "\\" + text));
		}
	}
}
