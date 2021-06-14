using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x0200000C RID: 12
	[Export(typeof(IPkgPlugin))]
	internal class Configuration : PkgPlugin
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00003600 File Offset: 0x00001800
		public override void ConvertEntries(XElement ToPkg, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromCsi)
		{
			PkgBldrHelpers.SetDefaultNameSpace(FromCsi, FromCsi.Name.Namespace);
			IEnumerable<XElement> enumerable = FromCsi.Descendants(FromCsi.Name.Namespace + "element");
			MyContainter myContainter = (MyContainter)enviorn.arg;
			foreach (XElement xelement in enumerable)
			{
				IEnumerable<XAttribute> enumerable2 = xelement.Attributes();
				string text = null;
				string value = null;
				string text2 = null;
				string text3 = null;
				foreach (XAttribute xattribute in enumerable2)
				{
					string localName = xattribute.Name.LocalName;
					if (!(localName == "default"))
					{
						if (!(localName == "name"))
						{
							if (!(localName == "type"))
							{
								if (localName == "handler")
								{
									text3 = xattribute.Value;
								}
							}
							else
							{
								text2 = xattribute.Value;
							}
						}
						else
						{
							value = xattribute.Value;
						}
					}
					else
					{
						text = xattribute.Value;
					}
				}
				if (text3 != null && text3.StartsWith("regkey", StringComparison.OrdinalIgnoreCase) && text != null)
				{
					bool flag = false;
					text3 = text3.Split(new char[]
					{
						'\''
					})[1];
					if (!(text2 == "xsd:boolean"))
					{
						if (!(text2 == "xsd:string"))
						{
							if (!(text2 == "xsd:unsignedInt"))
							{
								Console.WriteLine("");
							}
							else
							{
								text2 = "REG_DWORD";
								text = Convert.ToUInt32(text, CultureInfo.InvariantCulture).ToString("X8", CultureInfo.InvariantCulture);
								flag = true;
							}
						}
						else
						{
							text2 = "REG_SZ";
							flag = true;
						}
					}
					else
					{
						text2 = "REG_DWORD";
						if (text.ToLowerInvariant().Equals("false"))
						{
							text = "00000000";
						}
						else
						{
							text = "00000001";
						}
						flag = true;
					}
					if (flag)
					{
						text3 = RegHelpers.RegKeyNameToMacro(text3);
						if (!string.IsNullOrEmpty(text3))
						{
							XElement xelement2 = new XElement(myContainter.RegKeys.Name.Namespace + "RegKey");
							xelement2.Add(new XAttribute("KeyName", text3));
							XElement xelement3 = new XElement(myContainter.RegKeys.Name.Namespace + "RegValue");
							xelement3.Add(new XAttribute("Name", value));
							xelement3.Add(new XAttribute("Type", text2));
							xelement3.Add(new XAttribute("Value", text));
							xelement2.Add(xelement3);
							Share.MergeNewPkgRegKey(myContainter.RegKeys, xelement2);
						}
					}
				}
			}
		}
	}
}
