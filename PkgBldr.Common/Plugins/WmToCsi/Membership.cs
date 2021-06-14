using System;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x02000017 RID: 23
	public static class Membership
	{
		// Token: 0x06000060 RID: 96 RVA: 0x000064E4 File Offset: 0x000046E4
		public static XElement Add(XElement toCsi, string buildFilter, string subCategory, string name, string version, string publicKeyToken, string typeName)
		{
			XContainer xcontainer = PkgBldrHelpers.AddIfNotFound(toCsi, "memberships");
			XElement xelement = new XElement(toCsi.Name.Namespace + "categoryMembership");
			xcontainer.Add(xelement);
			if (buildFilter != null)
			{
				xelement.Add(new XAttribute("buildFilter", buildFilter));
			}
			XElement xelement2 = new XElement(toCsi.Name.Namespace + "id");
			xelement.Add(xelement2);
			xelement2.Add(new XAttribute("name", name));
			xelement2.Add(new XAttribute("version", version));
			xelement2.Add(new XAttribute("publicKeyToken", publicKeyToken));
			xelement2.Add(new XAttribute("typeName", typeName));
			XElement xelement3 = new XElement(toCsi.Name.Namespace + "categoryInstance");
			if (subCategory != null)
			{
				xelement3.Add(new XAttribute("subcategory", subCategory));
			}
			xelement.Add(xelement3);
			return xelement3;
		}
	}
}
