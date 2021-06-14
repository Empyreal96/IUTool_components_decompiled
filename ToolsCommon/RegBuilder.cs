using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200004F RID: 79
	public static class RegBuilder
	{
		// Token: 0x06000236 RID: 566 RVA: 0x0000AA60 File Offset: 0x00008C60
		private static void CheckConflicts(IEnumerable<RegValueInfo> values)
		{
			Dictionary<string, RegValueInfo> dictionary = new Dictionary<string, RegValueInfo>();
			foreach (RegValueInfo regValueInfo in values)
			{
				if (regValueInfo.ValueName != null)
				{
					RegValueInfo regValueInfo2 = null;
					if (dictionary.TryGetValue(regValueInfo.ValueName, out regValueInfo2))
					{
						throw new IUException("Registry conflict discovered: keyName: {0}, valueName: {1}, oldValue: {2}, newValue: {3}", new object[]
						{
							regValueInfo.KeyName,
							regValueInfo.ValueName,
							regValueInfo2.Value,
							regValueInfo.Value
						});
					}
					dictionary.Add(regValueInfo.ValueName, regValueInfo);
				}
			}
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000AB04 File Offset: 0x00008D04
		private static void ConvertRegSz(StringBuilder output, string name, string value)
		{
			RegUtil.RegOutput(output, name, value, false);
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000AB0F File Offset: 0x00008D0F
		private static void ConvertRegExpandSz(StringBuilder output, string name, string value)
		{
			RegUtil.RegOutput(output, name, value, true);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000AB1A File Offset: 0x00008D1A
		private static void ConvertRegMultiSz(StringBuilder output, string name, string value)
		{
			RegUtil.RegOutput(output, name, value.Split(new char[]
			{
				';'
			}));
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000AB34 File Offset: 0x00008D34
		private static void ConvertRegDWord(StringBuilder output, string name, string value)
		{
			uint value2 = 0U;
			if (!uint.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture.NumberFormat, out value2))
			{
				throw new IUException("Invalid dword string: {0}", new object[]
				{
					value
				});
			}
			RegUtil.RegOutput(output, name, value2);
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000AB7C File Offset: 0x00008D7C
		private static void ConvertRegQWord(StringBuilder output, string name, string value)
		{
			ulong value2 = 0UL;
			if (!ulong.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture.NumberFormat, out value2))
			{
				throw new IUException("Invalid qword string: {0}", new object[]
				{
					value
				});
			}
			RegUtil.RegOutput(output, name, value2);
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000ABC2 File Offset: 0x00008DC2
		private static void ConvertRegBinary(StringBuilder output, string name, string value)
		{
			RegUtil.RegOutput(output, name, RegUtil.HexStringToByteArray(value));
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000ABD4 File Offset: 0x00008DD4
		private static void ConvertRegHex(StringBuilder output, string name, string value)
		{
			Match match = Regex.Match(value, "^hex\\((?<type>[0-9A-Fa-f]+)\\):(?<value>.*)$");
			if (!match.Success)
			{
				throw new IUException("Invalid value '{0}' for REG_HEX type, shoudl be 'hex(<type>):<binary_values>'", new object[]
				{
					value
				});
			}
			int type = 0;
			if (!int.TryParse(match.Groups["type"].Value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture.NumberFormat, out type))
			{
				throw new IUException("Invalid hex type '{0}' in REG_HEX value '{1}'", new object[]
				{
					match.Groups["type"].Value,
					value
				});
			}
			string value2 = match.Groups["value"].Value;
			RegUtil.RegOutput(output, name, type, RegUtil.HexStringToByteArray(value2));
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000AC8C File Offset: 0x00008E8C
		private static void WriteValue(RegValueInfo value, StringBuilder regContent)
		{
			switch (value.Type)
			{
			case RegValueType.String:
				RegBuilder.ConvertRegSz(regContent, value.ValueName, value.Value);
				return;
			case RegValueType.ExpandString:
				RegBuilder.ConvertRegExpandSz(regContent, value.ValueName, value.Value);
				return;
			case RegValueType.Binary:
				RegBuilder.ConvertRegBinary(regContent, value.ValueName, value.Value);
				return;
			case RegValueType.DWord:
				RegBuilder.ConvertRegDWord(regContent, value.ValueName, value.Value);
				return;
			case RegValueType.MultiString:
				RegBuilder.ConvertRegMultiSz(regContent, value.ValueName, value.Value);
				return;
			case RegValueType.QWord:
				RegBuilder.ConvertRegQWord(regContent, value.ValueName, value.Value);
				return;
			case RegValueType.Hex:
				RegBuilder.ConvertRegHex(regContent, value.ValueName, value.Value);
				return;
			default:
				throw new IUException("Unknown registry value type '{0}'", new object[]
				{
					value.Type
				});
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000AD6C File Offset: 0x00008F6C
		private static void WriteKey(string keyName, IEnumerable<RegValueInfo> values, StringBuilder regContent)
		{
			regContent.AppendFormat("[{0}]", keyName);
			regContent.AppendLine();
			foreach (RegValueInfo regValueInfo in values)
			{
				if (regValueInfo.ValueName != null)
				{
					RegBuilder.WriteValue(regValueInfo, regContent);
				}
			}
			regContent.AppendLine();
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000ADD8 File Offset: 0x00008FD8
		private static void AddRegistryKey(List<RegValueInfo> values, XElement registryKeys, bool useOEMPriority)
		{
			List<RegValueInfo> list = new List<RegValueInfo>();
			Dictionary<string, RegValueInfo> dictionary = new Dictionary<string, RegValueInfo>();
			foreach (RegValueInfo regValueInfo in values)
			{
				if (regValueInfo.ValueName != null)
				{
					RegValueInfo item = null;
					if (dictionary.TryGetValue(regValueInfo.ValueName, out item))
					{
						dictionary.Remove(regValueInfo.ValueName);
						list.Remove(item);
					}
					dictionary.Add(regValueInfo.ValueName, regValueInfo);
					list.Add(regValueInfo);
				}
			}
			XElement xelement = new XElement(RegBuilder.xmlns + "registryKey", new XAttribute("keyName", list[0].KeyName));
			foreach (RegValueInfo regValueInfo2 in list)
			{
				if (regValueInfo2.ValueName != null)
				{
					XElement xelement2 = new XElement(RegBuilder.xmlns + "registryValue");
					xelement2.Add(new XAttribute("name", regValueInfo2.ValueName.Equals("@", StringComparison.InvariantCultureIgnoreCase) ? "" : regValueInfo2.ValueName));
					xelement2.Add(new XAttribute("valueType", regValueInfo2.Type.GetXmlEnumAttributeValueFromEnum<RegValueType>()));
					xelement2.Add(new XAttribute("value", RegUtil.GetRegistryValue(regValueInfo2.Type, regValueInfo2.Value)));
					if (regValueInfo2.Type == RegValueType.MultiString)
					{
						xelement2.Add(new XAttribute("operationHint", "replace"));
					}
					if (useOEMPriority)
					{
						xelement2.Add(new XAttribute("priority", "10"));
					}
					xelement.Add(xelement2);
				}
			}
			registryKeys.Add(xelement);
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000AFF4 File Offset: 0x000091F4
		public static void BuildRegistryEntries(IEnumerable<RegValueInfo> regValueInfoList, string outputFile, bool useOEMPriority)
		{
			XDocument xdocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[0]);
			XElement xelement = new XElement(RegBuilder.xmlns + "registryKeys");
			foreach (IGrouping<string, RegValueInfo> source in from x in regValueInfoList
			group x by x.KeyName)
			{
				RegBuilder.AddRegistryKey(source.ToList<RegValueInfo>(), xelement, useOEMPriority);
			}
			xdocument.Add(xelement);
			xdocument.Save(outputFile);
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000B0A8 File Offset: 0x000092A8
		public static void BuildRegistryEntries(IEnumerable<RegValueInfo> regValueInfoList, string outputFile)
		{
			RegBuilder.BuildRegistryEntries(regValueInfoList, outputFile, false);
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000B0B2 File Offset: 0x000092B2
		public static void Build(IEnumerable<RegValueInfo> values, string outputFile)
		{
			RegBuilder.Build(values, outputFile, "");
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000B0C0 File Offset: 0x000092C0
		public static void Build(IEnumerable<RegValueInfo> values, string outputFile, string headerComment)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Windows Registry Editor Version 5.00");
			if (!string.IsNullOrEmpty(headerComment))
			{
				foreach (string text in headerComment.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
				{
					string text2 = text.TrimStart(new char[]
					{
						' '
					});
					if (text2 != string.Empty && text2[0] == ';')
					{
						stringBuilder.AppendLine(text);
					}
					else
					{
						stringBuilder.AppendLine("; " + text);
					}
				}
				stringBuilder.AppendLine("");
			}
			foreach (IGrouping<string, RegValueInfo> grouping in from x in values
			group x by x.KeyName)
			{
				RegBuilder.CheckConflicts(grouping);
				RegBuilder.WriteKey(grouping.Key, grouping, stringBuilder);
			}
			LongPathFile.WriteAllText(outputFile, stringBuilder.ToString(), Encoding.Unicode);
		}

		// Token: 0x04000110 RID: 272
		private static XNamespace xmlns = "urn:schemas-microsoft-com:asm.v3";
	}
}
