using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.WindowsPhone.Multivariant.Offline
{
	// Token: 0x02000006 RID: 6
	public class MVCondition : IEquatable<MVCondition>
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000023C5 File Offset: 0x000005C5
		// (set) Token: 0x06000011 RID: 17 RVA: 0x000023CD File Offset: 0x000005CD
		public Dictionary<string, WPConstraintValue> KeyValues { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000023D6 File Offset: 0x000005D6
		public static IEnumerable<string> ValidKeys
		{
			get
			{
				return MVCondition.BuiltInKeys;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000023E0 File Offset: 0x000005E0
		public static IEnumerable<string> BuiltInKeys
		{
			get
			{
				return new List<string>
				{
					"mcc",
					"mnc",
					"spn",
					"uiname",
					"uiorder",
					"gid1"
				};
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002434 File Offset: 0x00000634
		public static IEnumerable<string> WildCardKeys
		{
			get
			{
				return new List<string>
				{
					"mcc",
					"mnc",
					"carrierid"
				};
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000245C File Offset: 0x0000065C
		private static bool IsWildCardCondition(WPConstraintValue value)
		{
			return value.IsWildCard && (value.KeyValue == null || value.KeyValue == "");
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00002484 File Offset: 0x00000684
		private static Dictionary<string, Func<WPConstraintValue, bool>> keyTypes
		{
			get
			{
				Dictionary<string, Func<WPConstraintValue, bool>> dictionary = new Dictionary<string, Func<WPConstraintValue, bool>>(StringComparer.OrdinalIgnoreCase);
				uint num;
				dictionary.Add("mcc", (WPConstraintValue value) => (!value.IsWildCard && value.KeyValue != null && uint.TryParse(value.KeyValue, out num) && value.KeyValue.Length == 3) || MVCondition.IsWildCardCondition(value));
				dictionary.Add("mnc", (WPConstraintValue value) => (!value.IsWildCard && value.KeyValue != null && uint.TryParse(value.KeyValue, out num) && 2 <= value.KeyValue.Length && value.KeyValue.Length <= 3) || MVCondition.IsWildCardCondition(value));
				dictionary.Add("spn", (WPConstraintValue value) => value.KeyValue != null && value.KeyValue.Length <= 16);
				dictionary.Add("uiname", (WPConstraintValue value) => value.KeyValue != null);
				dictionary.Add("uiorder", (WPConstraintValue value) => value.KeyValue != null);
				ulong longnum;
				dictionary.Add("gid1", (WPConstraintValue value) => value.KeyValue != null && ulong.TryParse(value.KeyValue, NumberStyles.HexNumber, null, out longnum));
				return dictionary;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002564 File Offset: 0x00000764
		private static Dictionary<string, string> errors
		{
			get
			{
				return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
				{
					{
						"mcc",
						"MCC values must be a three digit numerical value or WildCard with null value"
					},
					{
						"mnc",
						"MNC values must be a two or three digit numerical value or WildCard with null value"
					},
					{
						"spn",
						"SPN values must be under 16 characters"
					},
					{
						"uiname",
						"UIName values must be defined"
					},
					{
						"uiorder",
						"UIOrder values must be defined"
					},
					{
						"gid1",
						"GID1 values must be a string of hexadecimal digits"
					}
				};
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000025DC File Offset: 0x000007DC
		public static bool IsValidValue(string keyName, WPConstraintValue value, out string errorMessage)
		{
			if (!MVCondition.ValidKeys.Contains(keyName, StringComparer.OrdinalIgnoreCase))
			{
				errorMessage = string.Empty;
				return true;
			}
			if (value.IsWildCard)
			{
				errorMessage = string.Empty;
				return true;
			}
			Match match = Regex.Match(value.KeyValue, "^(?<not>!?)(?<comparison>(pattern:|range:)?)(?<value>.*)$");
			if (!match.Success)
			{
				errorMessage = "Target values should be in the form \"[!][pattern:|range:]<match value>";
				return false;
			}
			Group group = match.Groups["comparison"];
			string text = match.Groups["value"].Success ? match.Groups["value"].Value : "";
			if (group.Success)
			{
				string value2 = group.Value;
				if (value2 == "pattern:")
				{
					errorMessage = string.Format("The provided pattern string \"{0}\" is not a valid Regular Expression.", text);
					try
					{
						Regex.Match("", text);
					}
					catch (ArgumentException)
					{
						return false;
					}
					return true;
				}
				if (value2 == "range:")
				{
					errorMessage = string.Format("The provided range string \"{0}\" is not in the form \"<minimum>, <maximum>\"", text);
					return Regex.IsMatch(text, "^(-?\\d+),\\s?(-?\\d+)$");
				}
			}
			WPConstraintValue arg = new WPConstraintValue(text, value.IsWildCard);
			errorMessage = MVCondition.errors[keyName];
			return MVCondition.keyTypes[keyName](arg);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002724 File Offset: 0x00000924
		public static bool IsWildCardKey(string keyname)
		{
			return MVCondition.WildCardKeys.Contains(keyname, StringComparer.OrdinalIgnoreCase);
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002738 File Offset: 0x00000938
		public static IEnumerable<string> InvalidKeys
		{
			get
			{
				return new List<string>
				{
					"name",
					"type",
					"data",
					"target",
					"productid",
					"settingsgroup"
				};
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000278C File Offset: 0x0000098C
		public static bool IsValidKey(string keyName)
		{
			return !MVCondition.InvalidKeys.Contains(keyName, StringComparer.OrdinalIgnoreCase);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000027A1 File Offset: 0x000009A1
		public MVCondition()
		{
			this.KeyValues = new Dictionary<string, WPConstraintValue>();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000027B4 File Offset: 0x000009B4
		public bool IsValidCondition()
		{
			return !this.KeyValues.Any((KeyValuePair<string, WPConstraintValue> x) => !MVCondition.IsValidKey(x.Key));
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000027E8 File Offset: 0x000009E8
		public bool Equals(MVCondition other)
		{
			if (other == null)
			{
				return false;
			}
			if (this.KeyValues.Count != other.KeyValues.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, WPConstraintValue> keyValuePair in this.KeyValues)
			{
				WPConstraintValue wpconstraintValue;
				if (!other.KeyValues.TryGetValue(keyValuePair.Key, out wpconstraintValue))
				{
					return false;
				}
				if (wpconstraintValue.IsWildCard != keyValuePair.Value.IsWildCard)
				{
					return false;
				}
				if (!string.Equals(wpconstraintValue.KeyValue, keyValuePair.Value.KeyValue, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000028A8 File Offset: 0x00000AA8
		public override bool Equals(object obj)
		{
			return obj != null && this.Equals(obj as MVCondition);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000028BC File Offset: 0x00000ABC
		public override int GetHashCode()
		{
			int num = 0;
			foreach (KeyValuePair<string, WPConstraintValue> keyValuePair in this.KeyValues)
			{
				num ^= keyValuePair.Key.GetHashCode();
				if (keyValuePair.Value != null)
				{
					num ^= keyValuePair.Value.GetHashCode();
				}
			}
			return num;
		}
	}
}
