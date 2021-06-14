using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000016 RID: 22
	[Export(typeof(IPkgPlugin))]
	internal class Driver : PkgPlugin
	{
		// Token: 0x06000047 RID: 71 RVA: 0x00005228 File Offset: 0x00003428
		public override void ConvertEntries(XElement toWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromPkg)
		{
			this._env = enviorn;
			string text = fromPkg.Attribute("InfSource").Value;
			text = enviorn.Macros.Resolve(text);
			XElement xelement = new XElement(toWm.Name.Namespace + "driver");
			xelement.Add(new XElement(toWm.Name.Namespace + "inf", new XAttribute("source", text)));
			XElement xelement2 = new XElement(toWm.Name.Namespace + "files");
			foreach (XElement component in fromPkg.Elements(fromPkg.Name.Namespace + "Files"))
			{
				base.ConvertEntries(xelement2, plugins, enviorn, component);
			}
			foreach (XElement pkgDriverReference in fromPkg.Elements(fromPkg.Name.Namespace + "Reference"))
			{
				XElement xelement3 = this.MergeDriverFile(xelement2, pkgDriverReference);
				if (xelement3 != null)
				{
					xelement2.Add(xelement3);
				}
			}
			if (xelement2.HasElements)
			{
				xelement.Add(xelement2);
			}
			XElement xelement4 = fromPkg.Element(fromPkg.Name.Namespace + "Security");
			if (xelement4 != null)
			{
				List<XElement> list = new List<XElement>();
				foreach (XElement xelement5 in xelement4.Elements())
				{
					string localName = xelement5.Name.LocalName;
					if (localName == "AccessedByCapability")
					{
						list.Add(new XElement(toWm.Name.Namespace + "accessedByCapability", new object[]
						{
							new XAttribute("id", xelement5.Attribute("Id").Value),
							new XAttribute("rights", xelement5.Attribute("Rights").Value)
						}));
					}
					else
					{
						enviorn.Logger.LogWarning(string.Format(CultureInfo.InvariantCulture, "<Package> <{0}> not converted", new object[]
						{
							xelement5.Name.LocalName
						}), new object[0]);
					}
				}
				if (list.Any<XElement>())
				{
					xelement.Add(new XElement(toWm.Name.Namespace + "security", new object[]
					{
						new XAttribute("infSectionName", xelement4.Attribute("InfSectionName").Value),
						list
					}));
				}
			}
			PkgBldrHelpers.AddIfNotFound(toWm, "drivers").Add(xelement);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00005544 File Offset: 0x00003744
		private XElement MergeDriverFile(XElement wmDriverFiles, XElement pkgDriverReference)
		{
			string text = pkgDriverReference.Attribute("Source").Value;
			text = this._env.Macros.Resolve(text);
			string value = text.ToLowerInvariant();
			using (IEnumerator<XElement> enumerator = wmDriverFiles.Elements().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Attribute("source").Value.ToLowerInvariant().Equals(value))
					{
						return null;
					}
				}
			}
			XElement xelement = new XElement(wmDriverFiles.Name.Namespace + "file");
			xelement.Add(new XAttribute("source", text));
			xelement.Add(new XAttribute("destinationDir", "$(runtime.drivers)"));
			return xelement;
		}

		// Token: 0x04000009 RID: 9
		private Config _env;
	}
}
