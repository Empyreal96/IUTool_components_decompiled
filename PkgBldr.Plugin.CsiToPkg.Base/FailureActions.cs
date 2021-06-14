using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x0200000A RID: 10
	[Export(typeof(IPkgPlugin))]
	internal class FailureActions : PkgPlugin
	{
		// Token: 0x06000018 RID: 24 RVA: 0x000032B0 File Offset: 0x000014B0
		public override void ConvertEntries(XElement ToPkg, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromCsi)
		{
			IEnumerable<XElement> enumerable = FromCsi.Descendants(FromCsi.Name.Namespace + "action");
			if (enumerable.Count<XElement>() == 0)
			{
				return;
			}
			string attributeValue = PkgBldrHelpers.GetAttributeValue(FromCsi, "resetPeriod");
			List<byte> list = new List<byte>();
			int value = 0;
			int value2 = 0;
			int value3 = Convert.ToInt32(attributeValue, CultureInfo.InvariantCulture);
			list.AddRange(BitConverter.GetBytes(value3));
			list.AddRange(BitConverter.GetBytes(value));
			list.AddRange(BitConverter.GetBytes(value2));
			list.AddRange(BitConverter.GetBytes(enumerable.Count<XElement>()));
			list.AddRange(BitConverter.GetBytes(20));
			foreach (XElement element in enumerable)
			{
				string attributeValue2 = PkgBldrHelpers.GetAttributeValue(element, "type");
				string attributeValue3 = PkgBldrHelpers.GetAttributeValue(element, "delay");
				string a = attributeValue2.ToLowerInvariant();
				int value4;
				if (!(a == "none"))
				{
					if (!(a == "rebootmachine"))
					{
						if (!(a == "restartservice"))
						{
							if (!(a == "runcommand"))
							{
								Console.WriteLine("warning: unknow service action type {0}", attributeValue2);
								continue;
							}
							value4 = 3;
						}
						else
						{
							value4 = 2;
						}
					}
					else
					{
						value4 = 1;
					}
				}
				else
				{
					value4 = 0;
				}
				list.AddRange(BitConverter.GetBytes(value4));
				uint value5 = Convert.ToUInt32(attributeValue3, CultureInfo.InvariantCulture);
				list.AddRange(BitConverter.GetBytes(value5));
			}
			string value6 = BitConverter.ToString(list.ToArray()).Replace('-', ',');
			XElement content = RegHelpers.PkgRegValue("FailureActions", "REG_BINARY", value6);
			ToPkg.Add(content);
		}
	}
}
