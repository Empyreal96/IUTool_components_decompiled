using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.MCSF.Offline
{
	// Token: 0x02000007 RID: 7
	public class PolicySetting
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000028DF File Offset: 0x00000ADF
		// (set) Token: 0x0600002E RID: 46 RVA: 0x000028E7 File Offset: 0x00000AE7
		public string Name { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002F RID: 47 RVA: 0x000028F0 File Offset: 0x00000AF0
		// (set) Token: 0x06000030 RID: 48 RVA: 0x000028F8 File Offset: 0x00000AF8
		public string Description { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002901 File Offset: 0x00000B01
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00002909 File Offset: 0x00000B09
		public string FieldName { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002912 File Offset: 0x00000B12
		// (set) Token: 0x06000034 RID: 52 RVA: 0x0000291A File Offset: 0x00000B1A
		public string SampleValue { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002923 File Offset: 0x00000B23
		// (set) Token: 0x06000036 RID: 54 RVA: 0x0000292B File Offset: 0x00000B2B
		public PolicySettingType SettingType { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002934 File Offset: 0x00000B34
		// (set) Token: 0x06000038 RID: 56 RVA: 0x0000293C File Offset: 0x00000B3C
		public PolicySettingDestination Destination { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002945 File Offset: 0x00000B45
		// (set) Token: 0x0600003A RID: 58 RVA: 0x0000294D File Offset: 0x00000B4D
		public PolicyAssetInfo AssetInfo { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002956 File Offset: 0x00000B56
		// (set) Token: 0x0600003C RID: 60 RVA: 0x0000295E File Offset: 0x00000B5E
		public string DefaultValue { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002967 File Offset: 0x00000B67
		// (set) Token: 0x0600003E RID: 62 RVA: 0x0000296F File Offset: 0x00000B6F
		public int Min { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002978 File Offset: 0x00000B78
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00002980 File Offset: 0x00000B80
		public int Max { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002989 File Offset: 0x00000B89
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00002991 File Offset: 0x00000B91
		public string Partition { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000043 RID: 67 RVA: 0x0000299A File Offset: 0x00000B9A
		public static IEnumerable<string> ValidPartitions
		{
			get
			{
				return new List<string>
				{
					PkgConstants.c_strMainOsPartition,
					PkgConstants.c_strEfiPartition,
					PkgConstants.c_strUpdateOsPartition
				};
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000044 RID: 68 RVA: 0x000029C2 File Offset: 0x00000BC2
		public IEnumerable<PolicyEnum> Options
		{
			get
			{
				return this.options;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000029CA File Offset: 0x00000BCA
		public List<string> OEMMacros
		{
			get
			{
				if (this._oemMacros == null)
				{
					this._oemMacros = PolicyMacroTable.OEMMacroList(this.Name);
				}
				return this._oemMacros;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000046 RID: 70 RVA: 0x000029EB File Offset: 0x00000BEB
		public bool HasOEMMacros
		{
			get
			{
				return this.OEMMacros.Count<string>() > 0;
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000029FB File Offset: 0x00000BFB
		internal PolicySetting()
		{
			this.SettingType = PolicySettingType.String;
			this.Min = int.MinValue;
			this.Max = int.MaxValue;
			this.options = null;
			this.DefaultValue = null;
			this.AssetInfo = null;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002A35 File Offset: 0x00000C35
		public PolicySetting(XElement settingElement, PolicyGroup parent) : this(settingElement, parent, null, null)
		{
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002A41 File Offset: 0x00000C41
		public PolicySetting(XElement settingElement, PolicyGroup parent, string definedIn) : this(settingElement, parent, definedIn, null)
		{
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002A50 File Offset: 0x00000C50
		public PolicySetting(XElement settingElement, PolicyGroup parent, string definedIn, string partition) : this()
		{
			this.Name = (string)settingElement.LocalAttribute("Name");
			this.Description = (string)settingElement.LocalAttribute("Description");
			this.FieldName = (string)settingElement.LocalAttribute("FieldName");
			this.SampleValue = (string)settingElement.LocalAttribute("SampleValue");
			this.Partition = partition;
			string text = (string)settingElement.LocalAttribute("Asset");
			if (text != null)
			{
				this.AssetInfo = parent.AssetByName(text);
			}
			XElement xelement = settingElement.LocalElement("RegistrySource") ?? settingElement.LocalElement("CspSource");
			if (xelement != null)
			{
				this.Destination = new PolicySettingDestination(xelement, this, parent);
				this.DefaultValue = (string)xelement.LocalAttribute("Default");
				this.SettingType = this.DetermineType(this.Destination.Type);
			}
			else
			{
				this.Destination = new PolicySettingDestination(this, parent);
			}
			this.DefinedIn = definedIn;
			this.LoadValidation(settingElement);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002B5C File Offset: 0x00000D5C
		public PolicyMacroTable GetMacroTable(PolicyMacroTable parentTable, string name)
		{
			PolicyMacroTable policyMacroTable = new PolicyMacroTable(this.Name, name);
			if (parentTable != null)
			{
				policyMacroTable.AddMacros(parentTable);
			}
			return policyMacroTable;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002B84 File Offset: 0x00000D84
		public string expandEnumeration(string value)
		{
			foreach (PolicyEnum policyEnum in this.Options)
			{
				if (value.Equals(policyEnum.FriendlyName, StringComparison.Ordinal) || value.Equals(policyEnum.Value, StringComparison.Ordinal))
				{
					return policyEnum.Value;
				}
				try
				{
					uint num = Extensions.ParseInt(value);
					uint num2 = Extensions.ParseInt(policyEnum.Value);
					if (num == num2)
					{
						return policyEnum.Value;
					}
				}
				catch
				{
				}
			}
			return null;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002C24 File Offset: 0x00000E24
		public string TransformValue(string value, string customType)
		{
			PolicySettingType policySettingType = this.SettingType;
			if (this.SettingType == PolicySettingType.Unknown && customType != null)
			{
				policySettingType = this.DetermineType(customType);
			}
			switch (policySettingType)
			{
			case PolicySettingType.String:
				if (this.AssetInfo == null)
				{
					return value;
				}
				if (this.AssetInfo.Presets != null && this.AssetInfo.Presets.Count<PolicyEnum>() != 0 && this.AssetInfo.PresetsAltDir.ContainsKey(value))
				{
					return Path.Combine(this.AssetInfo.PresetsAltDir[value], value);
				}
				return Path.Combine(this.AssetInfo.TargetDir, value);
			case PolicySettingType.Integer:
				return Extensions.ParseSignedInt(value).ToString();
			case PolicySettingType.Enumeration:
				break;
			case PolicySettingType.Boolean:
				if (this.Options == null)
				{
					return value;
				}
				break;
			case PolicySettingType.Binary:
				return Convert.ToBase64String(RegUtil.HexStringToByteArray(value));
			case PolicySettingType.Integer64:
				return Extensions.ParseSignedInt64(value).ToString();
			default:
				throw new MCSFOfflineException("Attempted to transform an unrecognized type!");
			}
			string text = this.expandEnumeration(value);
			if (text == null)
			{
				throw new MCSFOfflineException(string.Format("Value {0} is not a valid friendly name for setting {1}.", value, this.Name));
			}
			return text;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002D38 File Offset: 0x00000F38
		public bool IsValidValue<T>(T value)
		{
			string value2 = Convert.ToString(value);
			return this.IsValidValue(value2, null);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002D5C File Offset: 0x00000F5C
		public bool IsValidValue(string value, string customType)
		{
			PolicySettingType policySettingType = this.SettingType;
			if (this.SettingType == PolicySettingType.Unknown && customType != null)
			{
				policySettingType = this.DetermineType(customType);
			}
			switch (policySettingType)
			{
			case PolicySettingType.String:
				return this.IsValidString(value);
			case PolicySettingType.Integer:
				return this.IsValidInteger(value);
			case PolicySettingType.Enumeration:
				return this.IsValidEnumeration(value);
			case PolicySettingType.Boolean:
				return this.isValidBoolean(value);
			case PolicySettingType.Binary:
				return this.isValidBinary(value);
			case PolicySettingType.Integer64:
				return this.IsValidInteger64(value);
			default:
				return false;
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002DD8 File Offset: 0x00000FD8
		private bool isValidBinary(string value)
		{
			try
			{
				RegUtil.HexStringToByteArray(value);
			}
			catch
			{
				return false;
			}
			return true;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002E08 File Offset: 0x00001008
		private bool isValidBoolean(string value)
		{
			string intString = value;
			if (this.Options != null && this.IsValidEnumeration(value))
			{
				intString = this.expandEnumeration(value);
			}
			int num;
			try
			{
				num = Extensions.ParseSignedInt(intString);
			}
			catch (Exception ex)
			{
				if (ex is FormatException || ex is OverflowException || ex is InvalidCastException)
				{
					return false;
				}
				throw;
			}
			return num >= 0 && num <= 1;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002E78 File Offset: 0x00001078
		private bool IsValidEnumeration(string value)
		{
			foreach (PolicyEnum policyEnum in this.Options)
			{
				if (value.Equals(policyEnum.Value, StringComparison.InvariantCulture) || value.Equals(policyEnum.FriendlyName, StringComparison.InvariantCulture))
				{
					return true;
				}
				try
				{
					uint num = Extensions.ParseInt(value);
					uint num2 = Extensions.ParseInt(policyEnum.Value);
					if (num == num2)
					{
						return true;
					}
				}
				catch
				{
				}
			}
			return false;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002F10 File Offset: 0x00001110
		private bool IsValidString(string value)
		{
			return value.Length >= this.Min && value.Length <= this.Max;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002F34 File Offset: 0x00001134
		private bool IsValidInteger(string value)
		{
			int num;
			try
			{
				num = Extensions.ParseSignedInt(value);
			}
			catch (Exception ex)
			{
				if (ex is FormatException || ex is OverflowException || ex is InvalidCastException)
				{
					return false;
				}
				throw;
			}
			return num >= this.Min && num <= this.Max;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002F90 File Offset: 0x00001190
		private bool IsValidInteger64(string value)
		{
			try
			{
				Extensions.ParseSignedInt64(value);
			}
			catch (Exception ex)
			{
				if (ex is FormatException || ex is OverflowException || ex is InvalidCastException)
				{
					return false;
				}
				throw;
			}
			return true;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002FD8 File Offset: 0x000011D8
		private void LoadValidation(XElement settingElement)
		{
			if (this.SettingType == PolicySettingType.Binary || this.SettingType == PolicySettingType.Unknown)
			{
				return;
			}
			XElement xelement = settingElement.LocalElement("Validate");
			if (xelement == null)
			{
				return;
			}
			IEnumerable<XElement> enumerable = xelement.LocalElements("Option");
			if (enumerable.Count<XElement>() > 0)
			{
				this.options = new List<PolicyEnum>();
				foreach (XElement option in enumerable)
				{
					this.options.Add(new PolicyEnum(option, this.SettingType == PolicySettingType.Integer));
				}
				if (this.SettingType == PolicySettingType.Boolean)
				{
					return;
				}
				this.SettingType = PolicySettingType.Enumeration;
				return;
			}
			else
			{
				if (this.SettingType == PolicySettingType.Integer)
				{
					this.Min = Extensions.ParseSignedInt((string)xelement.LocalAttribute("Min"), this.Min);
					this.Max = Extensions.ParseSignedInt((string)xelement.LocalAttribute("Max"), this.Max);
					return;
				}
				if (this.SettingType == PolicySettingType.String)
				{
					this.Min = Extensions.ParseSignedInt((string)xelement.LocalAttribute("MinLength"), 0);
					this.Max = Extensions.ParseSignedInt((string)xelement.LocalAttribute("MaxLength"), this.Max);
				}
				return;
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000311C File Offset: 0x0000131C
		private PolicySettingType DetermineType(string type)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(type);
			if (num <= 2649789690U)
			{
				if (num <= 841060930U)
				{
					if (num <= 398550328U)
					{
						if (num != 113593914U)
						{
							if (num != 398550328U)
							{
								return PolicySettingType.Unknown;
							}
							if (!(type == "string"))
							{
								return PolicySettingType.Unknown;
							}
							return PolicySettingType.String;
						}
						else
						{
							if (!(type == "CFG_DATATYPE_STRING"))
							{
								return PolicySettingType.Unknown;
							}
							return PolicySettingType.String;
						}
					}
					else if (num != 508138029U)
					{
						if (num != 841060930U)
						{
							return PolicySettingType.Unknown;
						}
						if (!(type == "CFG_DATATYPE_BINARY"))
						{
							return PolicySettingType.Unknown;
						}
						return PolicySettingType.Binary;
					}
					else if (!(type == "CFG_DATATYPE_BOOLEAN"))
					{
						return PolicySettingType.Unknown;
					}
				}
				else if (num <= 1710517951U)
				{
					if (num != 1269982935U)
					{
						if (num != 1710517951U)
						{
							return PolicySettingType.Unknown;
						}
						if (!(type == "boolean"))
						{
							return PolicySettingType.Unknown;
						}
					}
					else
					{
						if (!(type == "REG_SZ"))
						{
							return PolicySettingType.Unknown;
						}
						return PolicySettingType.String;
					}
				}
				else if (num != 1915779037U)
				{
					if (num != 2649789690U)
					{
						return PolicySettingType.Unknown;
					}
					if (!(type == "REG_EXPAND_SZ"))
					{
						return PolicySettingType.Unknown;
					}
					return PolicySettingType.String;
				}
				else
				{
					if (!(type == "CFG_DATATYPE_MULTIPLE_STRING"))
					{
						return PolicySettingType.Unknown;
					}
					return PolicySettingType.String;
				}
				return PolicySettingType.Boolean;
			}
			if (num <= 3218261061U)
			{
				if (num <= 2947007297U)
				{
					if (num != 2946827223U)
					{
						if (num != 2947007297U)
						{
							return PolicySettingType.Unknown;
						}
						if (!(type == "REG_MULTI_SZ"))
						{
							return PolicySettingType.Unknown;
						}
						return PolicySettingType.String;
					}
					else
					{
						if (!(type == "CFG_DATATYPE_UNKNOWN"))
						{
							return PolicySettingType.Unknown;
						}
						return PolicySettingType.Unknown;
					}
				}
				else if (num != 3192896778U)
				{
					if (num != 3218261061U)
					{
						return PolicySettingType.Unknown;
					}
					if (!(type == "integer"))
					{
						return PolicySettingType.Unknown;
					}
				}
				else if (!(type == "REG_DWORD"))
				{
					return PolicySettingType.Unknown;
				}
			}
			else if (num <= 3799441447U)
			{
				if (num != 3716508924U)
				{
					if (num != 3799441447U)
					{
						return PolicySettingType.Unknown;
					}
					if (!(type == "REG_BINARY"))
					{
						return PolicySettingType.Unknown;
					}
					return PolicySettingType.Binary;
				}
				else
				{
					if (!(type == "binary"))
					{
						return PolicySettingType.Unknown;
					}
					return PolicySettingType.Binary;
				}
			}
			else if (num != 3862876543U)
			{
				if (num != 3992433415U)
				{
					return PolicySettingType.Unknown;
				}
				if (!(type == "integer64"))
				{
					return PolicySettingType.Unknown;
				}
				return PolicySettingType.Integer64;
			}
			else if (!(type == "CFG_DATATYPE_INTEGER"))
			{
				return PolicySettingType.Unknown;
			}
			return PolicySettingType.Integer;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003362 File Offset: 0x00001562
		public override string ToString()
		{
			return this.Name;
		}

		// Token: 0x0400001A RID: 26
		private const string CustomIntegerType = "integer";

		// Token: 0x0400001B RID: 27
		private const string CustomStringType = "string";

		// Token: 0x0400001C RID: 28
		private const string CustomBooleanType = "boolean";

		// Token: 0x0400001D RID: 29
		private const string CustomBinaryType = "binary";

		// Token: 0x0400001E RID: 30
		private const string CustomInteger64Type = "integer64";

		// Token: 0x0400001F RID: 31
		private List<PolicyEnum> options;

		// Token: 0x0400002A RID: 42
		public string DefinedIn;

		// Token: 0x0400002C RID: 44
		private List<string> _oemMacros;
	}
}
