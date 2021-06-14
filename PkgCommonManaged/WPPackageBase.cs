using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x0200000B RID: 11
	internal class WPPackageBase
	{
		// Token: 0x0600003D RID: 61 RVA: 0x00003777 File Offset: 0x00001977
		protected WPPackageBase(PkgManifest pkgManifest)
		{
			this.m_pkgManifest = pkgManifest;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00003792 File Offset: 0x00001992
		public PkgManifest Manifest
		{
			get
			{
				return this.m_pkgManifest;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003F RID: 63 RVA: 0x0000379A File Offset: 0x0000199A
		// (set) Token: 0x06000040 RID: 64 RVA: 0x000037A7 File Offset: 0x000019A7
		public ReleaseType ReleaseType
		{
			get
			{
				return this.m_pkgManifest.ReleaseType;
			}
			set
			{
				if (value == ReleaseType.Invalid)
				{
					throw new PackageException("Invalid release type");
				}
				this.m_pkgManifest.ReleaseType = value;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000041 RID: 65 RVA: 0x000032F2 File Offset: 0x000014F2
		// (set) Token: 0x06000042 RID: 66 RVA: 0x000037C3 File Offset: 0x000019C3
		public PackageStyle PackageStyle
		{
			get
			{
				return this.m_pkgManifest.PackageStyle;
			}
			set
			{
				if (value == PackageStyle.Invalid)
				{
					throw new PackageException("Invalid cab type");
				}
				this.m_pkgManifest.PackageStyle = value;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000037DF File Offset: 0x000019DF
		// (set) Token: 0x06000044 RID: 68 RVA: 0x000037EC File Offset: 0x000019EC
		public BuildType BuildType
		{
			get
			{
				return this.m_pkgManifest.BuildType;
			}
			set
			{
				if (value == BuildType.Invalid)
				{
					throw new PackageException("Invalid build type");
				}
				this.m_pkgManifest.BuildType = value;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00003808 File Offset: 0x00001A08
		// (set) Token: 0x06000046 RID: 70 RVA: 0x00003848 File Offset: 0x00001A48
		public CpuId CpuType
		{
			get
			{
				switch (this.m_pkgManifest.CpuType)
				{
				case CpuId.AMD64_X86:
					return CpuId.X86;
				case CpuId.ARM64_ARM:
					return CpuId.ARM;
				case CpuId.ARM64_X86:
					return CpuId.X86;
				default:
					return this.m_pkgManifest.CpuType;
				}
			}
			set
			{
				if (value == CpuId.Invalid)
				{
					throw new PackageException("Invalid CPU type");
				}
				this.m_pkgManifest.CpuType = value;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00003864 File Offset: 0x00001A64
		public CpuId ComplexCpuType
		{
			get
			{
				return this.m_pkgManifest.CpuType;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00003871 File Offset: 0x00001A71
		public string Keyform
		{
			get
			{
				return this.m_pkgManifest.Keyform;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000049 RID: 73 RVA: 0x0000387E File Offset: 0x00001A7E
		// (set) Token: 0x0600004A RID: 74 RVA: 0x0000388B File Offset: 0x00001A8B
		public OwnerType OwnerType
		{
			get
			{
				return this.m_pkgManifest.OwnerType;
			}
			set
			{
				if (value == OwnerType.Invalid)
				{
					throw new PackageException("Invalid Owner type");
				}
				this.m_pkgManifest.OwnerType = value;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600004B RID: 75 RVA: 0x000038A7 File Offset: 0x00001AA7
		// (set) Token: 0x0600004C RID: 76 RVA: 0x000038B4 File Offset: 0x00001AB4
		public string Culture
		{
			get
			{
				return this.m_pkgManifest.Culture;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.m_pkgManifest.Culture = string.Empty;
					return;
				}
				if (!Regex.Match(value, PkgConstants.c_strCultureStringPattern).Success)
				{
					throw new PackageException("Invalid culture string {0}", new object[]
					{
						value
					});
				}
				this.m_pkgManifest.Culture = value;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600004D RID: 77 RVA: 0x0000390D File Offset: 0x00001B0D
		// (set) Token: 0x0600004E RID: 78 RVA: 0x0000391C File Offset: 0x00001B1C
		public string Resolution
		{
			get
			{
				return this.m_pkgManifest.Resolution;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.m_pkgManifest.Resolution = string.Empty;
					return;
				}
				if (!Regex.Match(value, PkgConstants.c_strResolutionStringPattern).Success)
				{
					throw new PackageException("Invalid resolution string {0}", new object[]
					{
						value
					});
				}
				this.m_pkgManifest.Resolution = value;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00003975 File Offset: 0x00001B75
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00003984 File Offset: 0x00001B84
		public string Owner
		{
			get
			{
				return this.m_pkgManifest.Owner;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new PackageException("Owner string can't be empty");
				}
				if (value.Length > PkgConstants.c_iMaxPackageString)
				{
					throw new PackageException("Owner string can't be longer than {0} characters", new object[]
					{
						PkgConstants.c_iMaxPackageString
					});
				}
				if (!Regex.Match(value, PkgConstants.c_strPackageStringPattern).Success)
				{
					throw new PackageException("Invalid owner string '{0}'", new object[]
					{
						value
					});
				}
				this.m_pkgManifest.Owner = value;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00003A02 File Offset: 0x00001C02
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00003A10 File Offset: 0x00001C10
		public string Component
		{
			get
			{
				return this.m_pkgManifest.Component;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new PackageException("Component string can't be empty");
				}
				if (value.Length > PkgConstants.c_iMaxPackageString)
				{
					throw new PackageException("Component string can't be longer than {0} characters", new object[]
					{
						PkgConstants.c_iMaxPackageString
					});
				}
				if (!Regex.Match(value, PkgConstants.c_strPackageStringPattern).Success)
				{
					throw new PackageException("Invalid Component string '{0}'", new object[]
					{
						value
					});
				}
				this.m_pkgManifest.Component = value;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00003A8E File Offset: 0x00001C8E
		// (set) Token: 0x06000054 RID: 84 RVA: 0x00003A9C File Offset: 0x00001C9C
		public string SubComponent
		{
			get
			{
				return this.m_pkgManifest.SubComponent;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.m_pkgManifest.SubComponent = string.Empty;
					return;
				}
				if (value.Length > PkgConstants.c_iMaxPackageString)
				{
					throw new PackageException("SubComponent string can't be longer than {0} characters", new object[]
					{
						PkgConstants.c_iMaxPackageString
					});
				}
				if (!Regex.Match(value, PkgConstants.c_strPackageStringPattern).Success)
				{
					throw new PackageException("Invalid SubComponent string '{0}'", new object[]
					{
						value
					});
				}
				this.m_pkgManifest.SubComponent = value;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00003B20 File Offset: 0x00001D20
		public string Name
		{
			get
			{
				return this.m_pkgManifest.Name;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00003B2D File Offset: 0x00001D2D
		// (set) Token: 0x06000057 RID: 87 RVA: 0x00003B3A File Offset: 0x00001D3A
		public string PackageName
		{
			get
			{
				return this.m_pkgManifest.PackageName;
			}
			set
			{
				this.m_pkgManifest.PackageName = value;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00003B48 File Offset: 0x00001D48
		// (set) Token: 0x06000059 RID: 89 RVA: 0x00003B55 File Offset: 0x00001D55
		public VersionInfo Version
		{
			get
			{
				return this.m_pkgManifest.Version;
			}
			set
			{
				this.m_pkgManifest.Version = value;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00003B63 File Offset: 0x00001D63
		// (set) Token: 0x0600005B RID: 91 RVA: 0x00003B70 File Offset: 0x00001D70
		public string PublicKey
		{
			get
			{
				return this.m_pkgManifest.PublicKey;
			}
			set
			{
				this.m_pkgManifest.PublicKey = value;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00003B7E File Offset: 0x00001D7E
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00003B8C File Offset: 0x00001D8C
		public string Partition
		{
			get
			{
				return this.m_pkgManifest.Partition;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new PackageException("Partition string can't be empty");
				}
				if (value.Length > PkgConstants.c_iMaxPackageString)
				{
					throw new PackageException("Partition string can't be longer than {0} characters", new object[]
					{
						PkgConstants.c_iMaxPackageString
					});
				}
				if (!Regex.Match(value, PkgConstants.c_strPackageStringPattern).Success)
				{
					throw new PackageException("Invalid Partition string '{0}'", new object[]
					{
						value
					});
				}
				this.m_pkgManifest.Partition = value;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00003C0A File Offset: 0x00001E0A
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00003C18 File Offset: 0x00001E18
		public string Platform
		{
			get
			{
				return this.m_pkgManifest.Platform;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.m_pkgManifest.Platform = string.Empty;
					return;
				}
				if (value.Length > PkgConstants.c_iMaxPackageString)
				{
					throw new PackageException("Platform string can't be longer than {0} characters", new object[]
					{
						PkgConstants.c_iMaxPackageString
					});
				}
				if (!Regex.Match(value, PkgConstants.c_strPackageStringPattern).Success)
				{
					throw new PackageException("Invalid Platform string '{0}'", new object[]
					{
						value
					});
				}
				this.m_pkgManifest.Platform = value;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00003C9C File Offset: 0x00001E9C
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00003CAC File Offset: 0x00001EAC
		public string BuildString
		{
			get
			{
				return this.m_pkgManifest.BuildString;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.m_pkgManifest.BuildString = string.Empty;
					return;
				}
				if (value.Length > PkgConstants.c_iMaxBuildString)
				{
					throw new PackageException("Build string can't be longer than {0} characters", new object[]
					{
						PkgConstants.c_iMaxBuildString
					});
				}
				this.m_pkgManifest.BuildString = value;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00003D09 File Offset: 0x00001F09
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00003D18 File Offset: 0x00001F18
		public string GroupingKey
		{
			get
			{
				return this.m_pkgManifest.GroupingKey;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.m_pkgManifest.GroupingKey = string.Empty;
					return;
				}
				if (value.Length > PkgConstants.c_iMaxPackageString)
				{
					throw new PackageException("GroupingKey string can't be longer than {0} characters", new object[]
					{
						PkgConstants.c_iMaxPackageString
					});
				}
				if (!Regex.Match(value, PkgConstants.c_strPackageStringPattern).Success)
				{
					throw new PackageException("Invalid GroupingKey string '{0}'", new object[]
					{
						value
					});
				}
				this.m_pkgManifest.GroupingKey = value;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00003D9C File Offset: 0x00001F9C
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00003DAC File Offset: 0x00001FAC
		public string[] TargetGroups
		{
			get
			{
				return this.m_pkgManifest.TargetGroups;
			}
			set
			{
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						string text = value[i];
						if (string.IsNullOrEmpty(text))
						{
							throw new PackageException("Group ID can't be empty or null");
						}
						if (text.Length > PkgConstants.c_iMaxGroupIdString)
						{
							throw new PackageException("Group ID '{0}' can not exceed '{1}' characters", new object[]
							{
								PkgConstants.c_iMaxGroupIdString
							});
						}
						if (!Regex.Match(text, PkgConstants.c_strGroupIdPattern).Success)
						{
							throw new PackageException("Invalid group ID string '{0}'", new object[]
							{
								text
							});
						}
					}
					this.m_pkgManifest.TargetGroups = value;
					return;
				}
				this.m_pkgManifest.TargetGroups = this.s_emptyTargetGroups;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00003E56 File Offset: 0x00002056
		public int FileCount
		{
			get
			{
				return this.m_pkgManifest.m_files.Count;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000067 RID: 103 RVA: 0x00003E68 File Offset: 0x00002068
		public IEnumerable<IFileEntry> Files
		{
			get
			{
				return this.m_pkgManifest.m_files.Values;
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003E7C File Offset: 0x0000207C
		public IFileEntry FindFile(string devicePath)
		{
			FileEntry result = null;
			this.m_pkgManifest.m_files.TryGetValue(devicePath, out result);
			return result;
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00003EA0 File Offset: 0x000020A0
		public bool IsWow
		{
			get
			{
				return this.ComplexCpuType != this.CpuType;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00003EB3 File Offset: 0x000020B3
		public bool IsBinaryPartition
		{
			get
			{
				return this.m_pkgManifest.IsBinaryPartition;
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003EC0 File Offset: 0x000020C0
		public IFileEntry GetDsmFile()
		{
			return this.m_pkgManifest.m_files.First((KeyValuePair<string, FileEntry> x) => x.Value.FileType == FileType.Manifest).Value;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003B20 File Offset: 0x00001D20
		public override string ToString()
		{
			return this.m_pkgManifest.Name;
		}

		// Token: 0x04000012 RID: 18
		private string[] s_emptyTargetGroups = new string[0];

		// Token: 0x04000013 RID: 19
		protected PkgManifest m_pkgManifest;
	}
}
