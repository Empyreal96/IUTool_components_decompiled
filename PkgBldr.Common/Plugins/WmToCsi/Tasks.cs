using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000023 RID: 35
	[Export(typeof(IPkgPlugin))]
	internal class Tasks : PkgPlugin
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00006A2A File Offset: 0x00004C2A
		public override string XmlSchemaPath
		{
			get
			{
				return "PkgBldr.WM.Xsd\\Task.xsd";
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00006A34 File Offset: 0x00004C34
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement xelement = PkgBldrHelpers.AddIfNotFound(ToCsi, "taskScheduler");
			foreach (XElement xelement2 in FromWm.Descendants())
			{
				string localName = xelement2.Name.LocalName;
				XName name = localName.Equals("uri", StringComparison.OrdinalIgnoreCase) ? (xelement2.GetDefaultNamespace() + "URI") : (xelement2.GetDefaultNamespace() + string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
				{
					char.ToUpperInvariant(localName[0]),
					localName.Substring(1)
				}));
				xelement2.Name = name;
				if (xelement2.Attribute("context") != null)
				{
					List<XAttribute> list = xelement2.Attributes().ToList<XAttribute>();
					XAttribute xattribute = (from x in list
					where x.Name.LocalName.Equals("context")
					select x).First<XAttribute>();
					list.Add(new XAttribute("Context", xattribute.Value));
					list.Remove(xattribute);
					xelement2.ReplaceAttributes(list);
				}
			}
			foreach (XAttribute other in FromWm.Attributes())
			{
				XAttribute content = new XAttribute(other);
				xelement.Add(content);
			}
			base.ConvertEntries(xelement, plugins, enviorn, FromWm);
		}
	}
}
