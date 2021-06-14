using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.WindowsPhone.MCSF.Offline
{
	// Token: 0x02000004 RID: 4
	public class PolicyMacroTable
	{
		// Token: 0x0600001D RID: 29 RVA: 0x00002518 File Offset: 0x00000718
		private static bool IsBuiltinMacro(string macroName)
		{
			return macroName.StartsWith("__", StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002528 File Offset: 0x00000728
		public static List<string> OEMMacroList(string macroContainingString)
		{
			List<string> list = new List<string>();
			foreach (string text in macroContainingString.Split(new char[]
			{
				'/'
			}))
			{
				Match match = Regex.Match(text, "^~(?<macroName>[A-Za-z0-9_]*)~$");
				if (match.Success && !PolicyMacroTable.IsBuiltinMacro(match.Groups["macroName"].Value))
				{
					list.Add(text);
				}
			}
			return list;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x0000259C File Offset: 0x0000079C
		public static bool IsMatch(string macroContainingString, string expandedString, StringComparison comparisonType)
		{
			string[] array = macroContainingString.Split(new char[]
			{
				'/'
			});
			string[] array2 = expandedString.Split(new char[]
			{
				'/'
			});
			if (array.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				Match match = Regex.Match(text, "^~(?<macroName>[A-Za-z0-9_]*)~$");
				if (!match.Success)
				{
					if (!text.Equals(array2[i], comparisonType))
					{
						return false;
					}
				}
				else
				{
					string value = match.Groups["macroName"].Value;
					if (PolicyMacroTable.IsBuiltinMacro(value) && !string.Equals(array2[i], string.Format("$({0})", value), comparisonType) && !string.Equals(array2[i], string.Format("~{0}~", value), comparisonType))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002662 File Offset: 0x00000862
		public PolicyMacroTable(string macroDefiningString, string expandedString)
		{
			this.macros = new Dictionary<string, string>();
			this.AddMacros(macroDefiningString, expandedString);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000267D File Offset: 0x0000087D
		public string ReplaceMacros(string inputString)
		{
			if (string.IsNullOrEmpty(inputString))
			{
				return inputString;
			}
			return Regex.Replace(inputString, "~(?<macroName>[A-Za-z0-9_]*)~", delegate(Match match)
			{
				string value = match.Groups[1].Value;
				return this.macros[value];
			});
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000026A0 File Offset: 0x000008A0
		internal void AddMacros(PolicyMacroTable otherTable)
		{
			foreach (KeyValuePair<string, string> keyValuePair in otherTable.macros)
			{
				if (!this.macros.ContainsKey(keyValuePair.Key))
				{
					this.macros.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x0000271C File Offset: 0x0000091C
		internal void AddMacros(string macroDefiningString, string expandedString)
		{
			string[] array = macroDefiningString.Split(new char[]
			{
				'/'
			});
			string[] array2 = expandedString.Split(new char[]
			{
				'/'
			});
			if (!PolicyMacroTable.IsMatch(macroDefiningString, expandedString, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("macroDefiningString and expandedString must be the same path or setting name.");
			}
			for (int i = 0; i < array.Length; i++)
			{
				Match match = Regex.Match(array[i], "^~(?<macroName>[A-Za-z0-9_]*)~$");
				if (match.Success)
				{
					string value = match.Groups[1].Value;
					string value2 = array2[i];
					this.macros.Add(value, value2);
				}
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000027AD File Offset: 0x000009AD
		public static string MacroTildeToDollar(string input)
		{
			return Regex.Replace(input, "~(?<macroName>[A-Za-z0-9_]*)~", (Match x) => string.Format("$({0})", x.Groups["macroName"].Value));
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000027D9 File Offset: 0x000009D9
		public static string MacroDollarToTilde(string input)
		{
			return Regex.Replace(input, "\\$\\((?<macroName>[A-Za-z0-9_]*)\\)", (Match x) => string.Format("~{0}~", x.Groups["macroName"].Value));
		}

		// Token: 0x0400000A RID: 10
		private const char PathSeperator = '/';

		// Token: 0x0400000B RID: 11
		private const string MacroRegexTilde = "~(?<macroName>[A-Za-z0-9_]*)~";

		// Token: 0x0400000C RID: 12
		private const string MacroRegexDollar = "\\$\\((?<macroName>[A-Za-z0-9_]*)\\)";

		// Token: 0x0400000D RID: 13
		private const string MacroExactRegex = "^~(?<macroName>[A-Za-z0-9_]*)~$";

		// Token: 0x0400000E RID: 14
		private const string MacroName = "macroName";

		// Token: 0x0400000F RID: 15
		public Dictionary<string, string> macros;
	}
}
