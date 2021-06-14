using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x02000011 RID: 17
	internal static class RegHelpers
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00003BF4 File Offset: 0x00001DF4
		public static XElement PkgRegValue(string name, string valueType, string value)
		{
			XNamespace ns = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00";
			string value2;
			if (string.IsNullOrEmpty(name))
			{
				value2 = "@";
			}
			else
			{
				value2 = name;
			}
			string text = valueType.ToUpperInvariant();
			string text2 = value.Trim();
			if (!(text == "REG_DWORD") && !(text == "REG_QWORD"))
			{
				if (!(text == "REG_BINARY"))
				{
					if (text == "REG_RESOURCE_REQUIREMENTS_LIST" || text == "REG_RESOURCE_LIST")
					{
						Console.WriteLine("warning: ignoring {0}", text);
						return null;
					}
					if (text == "REG_NONE")
					{
						text = "REG_SZ";
					}
				}
				else
				{
					text2 = RegHelpers.SeperateRegBinaryWithCommas(text2.ToLowerInvariant());
				}
			}
			else if (text2.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			{
				text2 = text2.Substring(2);
			}
			XElement xelement = new XElement(ns + "RegValue");
			xelement.Add(new XAttribute("Name", value2));
			xelement.Add(new XAttribute("Type", text));
			xelement.Add(new XAttribute("Value", text2));
			return xelement;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003D0D File Offset: 0x00001F0D
		public static XElement PkgRegKey(string name)
		{
			XElement xelement = new XElement("urn:Microsoft.WindowsPhone/PackageSchema.v8.00" + "RegKey");
			xelement.Add(new XAttribute("KeyName", name));
			return xelement;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003D40 File Offset: 0x00001F40
		private static string SeperateRegBinaryWithCommas(string RegBinary)
		{
			if (RegBinary == null)
			{
				return null;
			}
			if (RegBinary == "")
			{
				return "00";
			}
			if (RegBinary.Split(new char[]
			{
				','
			}).Length > 1)
			{
				return RegBinary;
			}
			string text = "";
			if (RegBinary.Length % 2 != 0)
			{
				return null;
			}
			for (int i = 0; i < RegBinary.Length; i++)
			{
				if (i % 2 == 0)
				{
					text += ",";
				}
				text += RegBinary[i].ToString();
			}
			return text.Trim(new char[]
			{
				','
			});
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003DE0 File Offset: 0x00001FE0
		public static string RegKeyNameToMacro(string RegKeyName)
		{
			if (RegKeyName.StartsWith("HKLM", StringComparison.OrdinalIgnoreCase))
			{
				RegKeyName = Regex.Replace(RegKeyName, "HKLM", "HKEY_LOCAL_MACHINE\\SYSTEM");
			}
			if (RegKeyName.StartsWith("HKCU", StringComparison.OrdinalIgnoreCase))
			{
				RegKeyName = "";
				return null;
			}
			foreach (KeyValuePair<string, string> keyValuePair in RegHelpers.regKeyNameToMacro)
			{
				if (RegKeyName.StartsWith(keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
				{
					if (keyValuePair.Value.Equals("", StringComparison.OrdinalIgnoreCase))
					{
						Console.WriteLine("warning: could not resolve {0}", keyValuePair.Value);
						return null;
					}
					string str = RegKeyName.Remove(0, keyValuePair.Key.Length);
					RegKeyName = keyValuePair.Value + str;
					return RegKeyName;
				}
			}
			if (!RegKeyName.StartsWith("$(", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			return RegKeyName;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003ED8 File Offset: 0x000020D8
		public static string RegMacroToKeyName(string RegKeyName)
		{
			foreach (KeyValuePair<string, string> keyValuePair in RegHelpers.regKeyNameToMacro)
			{
				if (RegKeyName.StartsWith(keyValuePair.Value, StringComparison.OrdinalIgnoreCase))
				{
					string str = RegKeyName.Remove(0, keyValuePair.Value.Length);
					RegKeyName = keyValuePair.Key + str;
					return RegKeyName;
				}
			}
			return RegKeyName;
		}

		// Token: 0x04000010 RID: 16
		private static Dictionary<string, string> regKeyNameToMacro = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services",
				"$(hklm.services)"
			},
			{
				"HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\Services",
				"$(hklm.services)"
			},
			{
				"HKEY_LOCAL_MACHINE\\SYSTEM",
				"$(hklm.system)"
			},
			{
				"HKEY_LOCAL_MACHINE\\SOFTWARE",
				"$(hklm.software)"
			},
			{
				"HKEY_LOCAL_MACHINE\\HARDWARE",
				"$(hklm.hardware)"
			},
			{
				"HKEY_LOCAL_MACHINE\\SAM",
				"$(hklm.sam)"
			},
			{
				"HKEY_LOCAL_MACHINE\\Security",
				"$(hklm.security)"
			},
			{
				"HKEY_LOCAL_MACHINE\\BCD",
				"$(hklm.bcd)"
			},
			{
				"HKEY_LOCAL_MACHINE\\Drivers",
				"$(hklm.drivers)"
			},
			{
				"HKEY_CLASSES_ROOT",
				"$(hkcr.root)"
			},
			{
				"HKEY_CURRENT_USER",
				"$(hkcu.root)"
			},
			{
				"HKEY_USERS\\.DEFAULT",
				"$(hkuser.default)"
			},
			{
				"HKEY_LOCAL_MACHINE\\COMPONENTS",
				""
			}
		};
	}
}
