using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200000F RID: 15
	public class PkgFile
	{
		// Token: 0x06000064 RID: 100 RVA: 0x0000318A File Offset: 0x0000138A
		public PkgFile()
		{
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000065CB File Offset: 0x000047CB
		public PkgFile(FeatureManifest.PackageGroups fmGroup)
		{
			this.FMGroup = fmGroup;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000065DA File Offset: 0x000047DA
		public PkgFile(PkgFile srcPkg)
		{
			this.CopyPkgFile(srcPkg);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000065EC File Offset: 0x000047EC
		public static PkgFile CreatePackageBaseOnFMGroup(FeatureManifest.PackageGroups fmGroup, string groupValue, string pkgPath, string pkgName, string pkgID)
		{
			PkgFile pkgFile = null;
			switch (fmGroup)
			{
			case FeatureManifest.PackageGroups.BASE:
				pkgFile = new PkgFile(fmGroup);
				break;
			case FeatureManifest.PackageGroups.BOOTUI:
				pkgFile = new BootUIPkgFile
				{
					Language = groupValue,
					FeatureIdentifierPackage = true
				};
				break;
			case FeatureManifest.PackageGroups.BOOTLOCALE:
				pkgFile = new BootLocalePkgFile
				{
					Language = groupValue,
					FeatureIdentifierPackage = true
				};
				break;
			case FeatureManifest.PackageGroups.RELEASE:
				pkgFile = new ReleasePkgFile
				{
					ReleaseType = groupValue
				};
				break;
			case FeatureManifest.PackageGroups.DEVICELAYOUT:
				pkgFile = new DeviceLayoutPkgFile
				{
					SOC = groupValue
				};
				break;
			case FeatureManifest.PackageGroups.OEMDEVICEPLATFORM:
				pkgFile = new OEMDevicePkgFile
				{
					Device = groupValue
				};
				break;
			case FeatureManifest.PackageGroups.SV:
				pkgFile = new SVPkgFile
				{
					SV = groupValue
				};
				break;
			case FeatureManifest.PackageGroups.SOC:
				pkgFile = new SOCPkgFile
				{
					SOC = groupValue
				};
				break;
			case FeatureManifest.PackageGroups.DEVICE:
				pkgFile = new DevicePkgFile
				{
					Device = groupValue
				};
				break;
			case FeatureManifest.PackageGroups.MSFEATURE:
				pkgFile = new MSOptionalPkgFile
				{
					FeatureIDs = new List<string>
					{
						groupValue
					}
				};
				break;
			case FeatureManifest.PackageGroups.OEMFEATURE:
				pkgFile = new OEMOptionalPkgFile
				{
					FeatureIDs = new List<string>
					{
						groupValue
					}
				};
				break;
			case FeatureManifest.PackageGroups.KEYBOARD:
				pkgFile = new KeyboardPkgFile
				{
					Language = groupValue,
					FeatureIdentifierPackage = true
				};
				break;
			case FeatureManifest.PackageGroups.SPEECH:
				pkgFile = new SpeechPkgFile
				{
					Language = groupValue,
					FeatureIdentifierPackage = true
				};
				break;
			case FeatureManifest.PackageGroups.PRERELEASE:
				pkgFile = new PrereleasePkgFile
				{
					Type = groupValue
				};
				break;
			}
			pkgFile.Directory = pkgPath;
			pkgFile.Name = pkgName;
			pkgFile.ID = pkgID;
			pkgFile.FMGroup = fmGroup;
			return pkgFile;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000068 RID: 104 RVA: 0x0000676A File Offset: 0x0000496A
		// (set) Token: 0x06000069 RID: 105 RVA: 0x0000678B File Offset: 0x0000498B
		[XmlAttribute("ID")]
		public string ID
		{
			get
			{
				if (string.IsNullOrEmpty(this._ID))
				{
					return Path.GetFileNameWithoutExtension(this.Name);
				}
				return this._ID;
			}
			set
			{
				this._ID = value;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00006794 File Offset: 0x00004994
		// (set) Token: 0x0600006B RID: 107 RVA: 0x000067D0 File Offset: 0x000049D0
		[XmlAttribute("Version")]
		[DefaultValue(null)]
		public string Version
		{
			get
			{
				if (this._version == null || string.IsNullOrEmpty(this._version.ToString()))
				{
					return null;
				}
				return this._version.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this._version = null;
					return;
				}
				string[] array = value.Split(new char[]
				{
					'.'
				});
				ushort[] array2 = new ushort[4];
				for (int i = 0; i < Math.Min(array.Count<string>(), 4); i++)
				{
					if (string.IsNullOrEmpty(array[i]))
					{
						array2[i] = 0;
					}
					else
					{
						array2[i] = ushort.Parse(array[i]);
					}
				}
				if (array.Count<string>() != 4)
				{
					this._version = null;
					return;
				}
				this._version = new VersionInfo?(new VersionInfo(array2[0], array2[1], array2[2], array2[3]));
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600006C RID: 108 RVA: 0x0000686F File Offset: 0x00004A6F
		// (set) Token: 0x0600006D RID: 109 RVA: 0x00006872 File Offset: 0x00004A72
		[XmlIgnore]
		public virtual string GroupValue
		{
			get
			{
				return null;
			}
			set
			{
				if (value != null)
				{
					this._groupValues = value.Split(new char[]
					{
						';'
					}, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
					return;
				}
				this._groupValues = null;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600006E RID: 110 RVA: 0x0000689C File Offset: 0x00004A9C
		[XmlIgnore]
		public List<string> GroupValues
		{
			get
			{
				if (this._groupValues == null && !string.IsNullOrWhiteSpace(this.GroupValue))
				{
					this._groupValues = this.GroupValue.Split(new char[]
					{
						';'
					}, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
				}
				return this._groupValues;
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000068DC File Offset: 0x00004ADC
		public bool IsValidGroupValue(string groupValue)
		{
			bool result = true;
			if (!string.IsNullOrWhiteSpace(this.GroupValue))
			{
				result = this.GroupValues.Contains(groupValue, StringComparer.OrdinalIgnoreCase);
			}
			return result;
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000070 RID: 112 RVA: 0x0000690B File Offset: 0x00004B0B
		[XmlIgnore]
		public virtual bool IncludeInImage
		{
			get
			{
				return this.OemInput != null;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00006918 File Offset: 0x00004B18
		[XmlIgnore]
		public string PackagePath
		{
			get
			{
				string text = this.RawPackagePath;
				if (this.OemInput != null)
				{
					text = this.OemInput.ProcessOEMInputVariables(text);
				}
				return Environment.ExpandEnvironmentVariables(text);
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00006949 File Offset: 0x00004B49
		[XmlIgnore]
		public string RawPackagePath
		{
			get
			{
				return Path.Combine(this.Directory, this.Name);
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000073 RID: 115 RVA: 0x0000695C File Offset: 0x00004B5C
		public static List<CpuId> SupportedCPUIds
		{
			get
			{
				if (PkgFile._supportedCPUIds == null)
				{
					PkgFile._supportedCPUIds = Enum.GetValues(typeof(CpuId)).Cast<CpuId>().ToList<CpuId>();
					PkgFile._supportedCPUIds = (from id in PkgFile._supportedCPUIds
					where id > CpuId.Invalid
					select id).ToList<CpuId>();
				}
				return PkgFile._supportedCPUIds;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000074 RID: 116 RVA: 0x000069C8 File Offset: 0x00004BC8
		[XmlIgnore]
		public List<CpuId> CPUIds
		{
			get
			{
				if (this._cpuIds == null)
				{
					if (string.IsNullOrWhiteSpace(this.CPUType) || this.CPUType.Equals("*"))
					{
						this._cpuIds = PkgFile.SupportedCPUIds;
					}
					else
					{
						List<string> supportedList = PkgFile.GetSupportedList(this.CPUType);
						this._cpuIds = new List<CpuId>();
						bool ignoreCase = true;
						using (List<string>.Enumerator enumerator = supportedList.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								CpuId item;
								if (Enum.TryParse<CpuId>(enumerator.Current, ignoreCase, out item))
								{
									this._cpuIds.Add(item);
								}
							}
						}
					}
				}
				return this._cpuIds;
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00006A78 File Offset: 0x00004C78
		public bool IncludesCPUType(string cpuType)
		{
			bool ignoreCase = true;
			CpuId cpuId;
			return Enum.TryParse<CpuId>(cpuType, ignoreCase, out cpuId) && cpuId != CpuId.Invalid && this.CPUIds.Contains(cpuId);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00006AA8 File Offset: 0x00004CA8
		public string GetLanguagePackagePath(string language)
		{
			string text = this.RawLanguagePackagePath;
			text = text.Replace("$(langid)", language, StringComparison.OrdinalIgnoreCase);
			if (this.OemInput != null)
			{
				text = this.OemInput.ProcessOEMInputVariables(text);
			}
			return Environment.ExpandEnvironmentVariables(text);
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00006AE8 File Offset: 0x00004CE8
		[XmlIgnore]
		public string RawLanguagePackagePath
		{
			get
			{
				string path = string.IsNullOrEmpty(this.LangDirectory) ? this.Directory : this.LangDirectory;
				string extension = Path.GetExtension(this.Name);
				return Path.Combine(path, this.Name.Replace(extension, PkgFile.DefaultLanguagePattern + "$(langid)" + extension, StringComparison.OrdinalIgnoreCase));
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00006B40 File Offset: 0x00004D40
		public string GetWowPackagePath(string guestCpuType)
		{
			string text = this.RawWowPackagePath;
			text = text.Replace("$(guestcputype)", guestCpuType, StringComparison.OrdinalIgnoreCase);
			if (this.OemInput != null)
			{
				text = this.OemInput.ProcessOEMInputVariables(text);
			}
			return Environment.ExpandEnvironmentVariables(text);
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00006B80 File Offset: 0x00004D80
		[XmlIgnore]
		public string RawWowPackagePath
		{
			get
			{
				string extension = Path.GetExtension(this.Name);
				return Path.Combine(this.Directory, this.Name.Replace(extension, PkgFile.DefaultWowPattern + "$(cputype).$(guestcputype)" + extension, StringComparison.OrdinalIgnoreCase));
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00006BC4 File Offset: 0x00004DC4
		public string GetResolutionPackagePath(string resolution)
		{
			string text = this.RawResolutionPackagePath;
			text = text.Replace("$(resid)", resolution, StringComparison.OrdinalIgnoreCase);
			if (this.OemInput != null)
			{
				text = this.OemInput.ProcessOEMInputVariables(text);
			}
			return Environment.ExpandEnvironmentVariables(text);
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00006C04 File Offset: 0x00004E04
		[XmlIgnore]
		public string RawResolutionPackagePath
		{
			get
			{
				string extension = Path.GetExtension(this.Name);
				return Path.Combine(this.Directory, this.Name.Replace(extension, PkgFile.DefaultResolutionPattern + "$(resid)" + extension, StringComparison.OrdinalIgnoreCase));
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00006C45 File Offset: 0x00004E45
		public void ProcessVariables()
		{
			if (this.OemInput != null)
			{
				this.Directory = this.OemInput.ProcessOEMInputVariables(this.Directory);
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00006C68 File Offset: 0x00004E68
		public static List<string> GetSupportedList(string list)
		{
			char[] separator = new char[]
			{
				';'
			};
			List<string> list2 = new List<string>();
			list = list.Trim();
			list = list.Replace("(", "", StringComparison.OrdinalIgnoreCase);
			list = list.Replace(")", "", StringComparison.OrdinalIgnoreCase);
			list = list.Replace("!", "", StringComparison.OrdinalIgnoreCase);
			list2.AddRange(list.Split(separator));
			return list2;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00006CD4 File Offset: 0x00004ED4
		public static string GetSupportedListString(List<string> list, List<string> supportedList)
		{
			string result = null;
			if (list.Count<string>() > 0)
			{
				if (list.Except(supportedList, StringComparer.OrdinalIgnoreCase).Count<string>() == 0 && supportedList.Except(list, StringComparer.OrdinalIgnoreCase).Count<string>() == 0)
				{
					result = "*";
				}
				else
				{
					result = "(" + string.Join(";", list) + ")";
				}
			}
			return result;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00006D38 File Offset: 0x00004F38
		public static List<string> GetSupportedList(string list, List<string> supportedValues)
		{
			List<string> list2 = new List<string>();
			list = list.Trim();
			if (string.Compare(list, "*", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return supportedValues;
			}
			if (list.Contains("*"))
			{
				throw new FeatureAPIException("FeatureAPI!GetSupportedList: Supported values list '" + list + "' cannot include '*' unless it is alone (\"*\" to specify all supported values)");
			}
			List<string> supportedList = PkgFile.GetSupportedList(list);
			if (list.Contains("!"))
			{
				int num = list.IndexOf("!", StringComparison.OrdinalIgnoreCase);
				if (list.LastIndexOf("!", StringComparison.OrdinalIgnoreCase) != num || num != 0)
				{
					throw new FeatureAPIException("FeatureAPI!GetSupportedList: Supported values list '" + list + "' cannot contain both include and exclude values.  Exclude lists must contain a '!' at the beginning.");
				}
				using (List<string>.Enumerator enumerator = supportedValues.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						bool flag = false;
						foreach (string b in supportedList)
						{
							if (string.Equals(text, b, StringComparison.OrdinalIgnoreCase))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							list2.Add(text);
						}
					}
					return list2;
				}
			}
			foreach (string b2 in supportedList)
			{
				foreach (string text2 in supportedValues)
				{
					if (string.Equals(text2, b2, StringComparison.OrdinalIgnoreCase))
					{
						list2.Add(text2);
						break;
					}
				}
			}
			return list2;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00006EEC File Offset: 0x000050EC
		public virtual void CopyPkgFile(PkgFile srcPkgFile)
		{
			this.Directory = srcPkgFile.Directory;
			this.Language = srcPkgFile.Language;
			this.Wow = srcPkgFile.Wow;
			this.LangWow = srcPkgFile.LangWow;
			this.ResWow = srcPkgFile.ResWow;
			this.Name = srcPkgFile.Name;
			this.NoBasePackage = srcPkgFile.NoBasePackage;
			this.OemInput = srcPkgFile.OemInput;
			this.Resolution = srcPkgFile.Resolution;
			this.FeatureIdentifierPackage = srcPkgFile.FeatureIdentifierPackage;
			this.CPUType = srcPkgFile.CPUType;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00006F80 File Offset: 0x00005180
		public virtual void InitializeWithMergeResult(MergeResult result, FeatureManifest.PackageGroups fmGroup, string groupValue, List<string> supportedLanguages, List<string> supportedResolutions)
		{
			this.Directory = LongPath.GetDirectoryName(result.FilePath);
			this.Name = Path.GetFileName(result.FilePath);
			this._ID = result.PkgInfo.Name;
			this.FeatureIdentifierPackage = result.FeatureIdentifierPackage;
			this.Partition = result.PkgInfo.Partition;
			this._version = new VersionInfo?(result.PkgInfo.Version);
			this.PublicKey = result.PkgInfo.PublicKey;
			this.BinaryPartition = result.PkgInfo.IsBinaryPartition;
			this.FMGroup = fmGroup;
			if (result.Languages != null)
			{
				this.Language = PkgFile.GetSupportedListString(result.Languages.ToList<string>(), supportedLanguages);
			}
			if (result.Resolutions != null)
			{
				this.Resolution = PkgFile.GetSupportedListString(result.Resolutions.ToList<string>(), supportedResolutions);
			}
			this.GroupValue = groupValue;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00007063 File Offset: 0x00005263
		public override string ToString()
		{
			return this.ID;
		}

		// Token: 0x04000074 RID: 116
		public static readonly string DefaultLanguagePattern = "_Lang_";

		// Token: 0x04000075 RID: 117
		public static readonly string DefaultResolutionPattern = "_Res_";

		// Token: 0x04000076 RID: 118
		public static readonly string DefaultWowPattern = "_Wow_";

		// Token: 0x04000077 RID: 119
		private string _ID;

		// Token: 0x04000078 RID: 120
		[XmlAttribute("Path")]
		public string Directory;

		// Token: 0x04000079 RID: 121
		[XmlAttribute("LangPath")]
		public string LangDirectory;

		// Token: 0x0400007A RID: 122
		[XmlAttribute("Name")]
		public string Name;

		// Token: 0x0400007B RID: 123
		[XmlAttribute("NoBasePackage")]
		[DefaultValue(false)]
		public bool NoBasePackage;

		// Token: 0x0400007C RID: 124
		[XmlAttribute("FeatureIdentifierPackage")]
		[DefaultValue(false)]
		public bool FeatureIdentifierPackage;

		// Token: 0x0400007D RID: 125
		[XmlAttribute("Resolution")]
		public string Resolution;

		// Token: 0x0400007E RID: 126
		[XmlAttribute("Language")]
		public string Language;

		// Token: 0x0400007F RID: 127
		[XmlAttribute("Wow")]
		public string Wow;

		// Token: 0x04000080 RID: 128
		[XmlAttribute("LangWow")]
		public string LangWow;

		// Token: 0x04000081 RID: 129
		[XmlAttribute("ResWow")]
		public string ResWow;

		// Token: 0x04000082 RID: 130
		[XmlAttribute("CPUType")]
		[DefaultValue(null)]
		public string CPUType;

		// Token: 0x04000083 RID: 131
		[XmlAttribute("Partition")]
		[DefaultValue(null)]
		public string Partition;

		// Token: 0x04000084 RID: 132
		private VersionInfo? _version;

		// Token: 0x04000085 RID: 133
		[XmlAttribute("PublicKey")]
		[DefaultValue(null)]
		public string PublicKey;

		// Token: 0x04000086 RID: 134
		[DefaultValue(false)]
		[XmlAttribute]
		public bool BinaryPartition;

		// Token: 0x04000087 RID: 135
		[XmlIgnore]
		public FeatureManifest.PackageGroups FMGroup;

		// Token: 0x04000088 RID: 136
		private List<string> _groupValues;

		// Token: 0x04000089 RID: 137
		[XmlIgnore]
		public OEMInput OemInput;

		// Token: 0x0400008A RID: 138
		private static List<CpuId> _supportedCPUIds = null;

		// Token: 0x0400008B RID: 139
		private List<CpuId> _cpuIds;
	}
}
