using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200001D RID: 29
	public static class RegBuilder
	{
		// Token: 0x06000126 RID: 294 RVA: 0x00005BE8 File Offset: 0x00003DE8
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

		// Token: 0x06000127 RID: 295 RVA: 0x00005C8C File Offset: 0x00003E8C
		private static void ConvertRegSz(StringBuilder output, string name, string value)
		{
			RegUtil.RegOutput(output, name, value, false);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00005C97 File Offset: 0x00003E97
		private static void ConvertRegExpandSz(StringBuilder output, string name, string value)
		{
			RegUtil.RegOutput(output, name, value, true);
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00005CA2 File Offset: 0x00003EA2
		private static void ConvertRegMultiSz(StringBuilder output, string name, string value)
		{
			RegUtil.RegOutput(output, name, value.Split(new char[]
			{
				';'
			}));
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00005CBC File Offset: 0x00003EBC
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

		// Token: 0x0600012B RID: 299 RVA: 0x00005D04 File Offset: 0x00003F04
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

		// Token: 0x0600012C RID: 300 RVA: 0x00005D4A File Offset: 0x00003F4A
		private static void ConvertRegBinary(StringBuilder output, string name, string value)
		{
			RegUtil.RegOutput(output, name, RegUtil.HexStringToByteArray(value));
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00005D5C File Offset: 0x00003F5C
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

		// Token: 0x0600012E RID: 302 RVA: 0x00005E14 File Offset: 0x00004014
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

		// Token: 0x0600012F RID: 303 RVA: 0x00005EF4 File Offset: 0x000040F4
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

		// Token: 0x06000130 RID: 304 RVA: 0x00005F60 File Offset: 0x00004160
		public static void Build(IEnumerable<RegValueInfo> values, string outputFile)
		{
			RegBuilder.Build(values, outputFile, null);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00005F6C File Offset: 0x0000416C
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static void Build(IEnumerable<RegValueInfo> values, string outputFile, string headerComment = "")
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
	}
}
