using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Interfaces;
using Microsoft.CompPlat.PkgBldr.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.CompPlat.PkgBldr.Base.BasePlugins
{
	// Token: 0x0200005B RID: 91
	[Export(typeof(IPkgPlugin))]
	internal class BcdStore : PkgPlugin
	{
		// Token: 0x0600020C RID: 524 RVA: 0x0000AFB4 File Offset: 0x000091B4
		[SuppressMessage("Microsoft.Reliability", "CA2001")]
		public override void ConvertEntries(XElement toCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromWm)
		{
			if (enviorn.Convert.Equals(ConversionType.pkg2csi))
			{
				enviorn.Logger.LogInfo("Cannot convert bcdStore elements directly from pkg.xml to csi.", new object[0]);
				return;
			}
			BcdConverter bcdConverter = new BcdConverter(new IULogger());
			string directoryName = Microsoft.CompPlat.PkgBldr.Tools.LongPath.GetDirectoryName(enviorn.Input);
			string text = PkgBldrHelpers.GetAttributeValue(fromWm, "source");
			text = directoryName + "\\" + text;
			using (Stream manifestResourceStream = Assembly.LoadFrom(Assembly.GetAssembly(bcdConverter.GetType()).Location).GetManifestResourceStream("BcdLayout.xsd"))
			{
				bcdConverter.ProcessInputXml(text, manifestResourceStream);
			}
			BcdRegData bcdRegData = new BcdRegData();
			bcdConverter.SaveToRegData(bcdRegData);
			Dictionary<string, List<BcdRegValue>> dictionary = bcdRegData.RegKeys();
			XElement xelement = new XElement(toCsi.Name.Namespace + "registryKeys");
			foreach (KeyValuePair<string, List<BcdRegValue>> keyValuePair in dictionary)
			{
				XElement xelement2 = new XElement(toCsi.Name.Namespace + "registryKey");
				xelement2.Add(new XAttribute("keyName", keyValuePair.Key));
				foreach (BcdRegValue bcdRegValue in keyValuePair.Value)
				{
					XElement xelement3 = new XElement(toCsi.Name.Namespace + "registryValue");
					xelement3.Add(new XAttribute("name", bcdRegValue.Name));
					xelement3.Add(new XAttribute("value", bcdRegValue.Value));
					xelement3.Add(new XAttribute("valueType", bcdRegValue.Type));
					xelement3.Add(new XAttribute("mutable", "true"));
					string type = bcdRegValue.Type;
					if (!(type == "REG_BINARY") && !(type == "REG_DWORD") && !(type == "REG_SZ") && !(type == "REG_MULTI_SZ"))
					{
						throw new PkgGenException("BcdStore can't process invalid reg type {0}", new object[]
						{
							bcdRegValue.Type
						});
					}
					xelement2.Add(xelement3);
				}
				xelement.Add(xelement2);
			}
			string attributeValue = PkgBldrHelpers.GetAttributeValue(fromWm, "buildFilter");
			if (attributeValue != null)
			{
				xelement.Add(new XAttribute("buildFilter", attributeValue));
			}
			toCsi.Add(xelement);
		}
	}
}
