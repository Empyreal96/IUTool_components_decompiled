using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000023 RID: 35
	[Export(typeof(IPkgPlugin))]
	internal class RegValue : PkgPlugin
	{
		// Token: 0x0600007B RID: 123 RVA: 0x00006DC4 File Offset: 0x00004FC4
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = new XElement(ToWm.Name.Namespace + "regValue");
			string attributeValue = PkgBldrHelpers.GetAttributeValue(FromPkg, "buildFilter");
			if (attributeValue != null)
			{
				string value = Helpers.ConvertBuildFilter(attributeValue);
				xelement.Add(new XAttribute("buildFilter", value));
			}
			string text = null;
			string text2 = null;
			foreach (XAttribute xattribute in FromPkg.Attributes())
			{
				xattribute.Value = enviorn.Macros.Resolve(xattribute.Value);
				string localName = xattribute.Name.LocalName;
				if (!(localName == "Name"))
				{
					if (!(localName == "Type"))
					{
						if (localName == "Value")
						{
							text = xattribute.Value;
						}
					}
					else
					{
						text2 = xattribute.Value;
						if (text2.Equals("REG_HEX", StringComparison.InvariantCultureIgnoreCase))
						{
							text2 = "REG_BINARY";
						}
						xelement.Add(new XAttribute("type", text2));
					}
				}
				else if (!xattribute.Value.Equals("@"))
				{
					xelement.Add(new XAttribute("name", xattribute.Value));
				}
			}
			text = this.ConvertValue(text, text2, enviorn.Logger);
			if (text != null)
			{
				xelement.Add(new XAttribute("value", text));
				ToWm.Add(xelement);
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00006F54 File Offset: 0x00005154
		private string ConvertValue(string pkgValue, string wmType, IDeploymentLogger logger)
		{
			string text = null;
			if (string.IsNullOrEmpty(pkgValue))
			{
				return null;
			}
			if (!(wmType == "REG_BINARY"))
			{
				if (!(wmType == "REG_DWORD"))
				{
					if (!(wmType == "REG_QWORD"))
					{
						if (!(wmType == "REG_MULTI_SZ"))
						{
							text = pkgValue;
						}
						else
						{
							text = Helpers.ConvertMulitSz(pkgValue);
						}
					}
					else
					{
						text = pkgValue.TrimStart("0x".ToCharArray()).PadLeft(16, '0');
					}
				}
				else
				{
					text = pkgValue.TrimStart("0x".ToCharArray()).PadLeft(8, '0');
					text = "0x" + text;
				}
			}
			else
			{
				if (pkgValue.Contains(':'))
				{
					pkgValue = pkgValue.Split(new char[]
					{
						':'
					})[1];
				}
				foreach (string text2 in pkgValue.Split(new char[]
				{
					','
				}))
				{
					if (text2.Length == 1)
					{
						text += string.Format(CultureInfo.InvariantCulture, "0{0}", new object[]
						{
							text2
						});
					}
					else
					{
						text += text2;
					}
				}
				if (text == null)
				{
					text = pkgValue;
				}
				text = text.ToUpperInvariant();
			}
			return text;
		}
	}
}
