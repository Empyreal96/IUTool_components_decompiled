using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Phone.TestInfra
{
	// Token: 0x0200000A RID: 10
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00003D08 File Offset: 0x00001F08
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00003D20 File Offset: 0x00001F20
		// (set) Token: 0x06000079 RID: 121 RVA: 0x00003D42 File Offset: 0x00001F42
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("0.14")]
		public string Version
		{
			get
			{
				return (string)this["Version"];
			}
			set
			{
				this["Version"] = value;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00003D54 File Offset: 0x00001F54
		// (set) Token: 0x0600007B RID: 123 RVA: 0x00003D76 File Offset: 0x00001F76
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("logs")]
		public string DefaultLogDirectory
		{
			get
			{
				return (string)this["DefaultLogDirectory"];
			}
			set
			{
				this["DefaultLogDirectory"] = value;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00003D88 File Offset: 0x00001F88
		// (set) Token: 0x0600007D RID: 125 RVA: 0x00003DAA File Offset: 0x00001FAA
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("DeployTest.log")]
		public string DefaultLogName
		{
			get
			{
				return (string)this["DefaultLogName"];
			}
			set
			{
				this["DefaultLogName"] = value;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00003DBC File Offset: 0x00001FBC
		// (set) Token: 0x0600007F RID: 127 RVA: 0x00003DDE File Offset: 0x00001FDE
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("24")]
		public int DefaultFileExpiresInHours
		{
			get
			{
				return (int)this["DefaultFileExpiresInHours"];
			}
			set
			{
				this["DefaultFileExpiresInHours"] = value;
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00003DF4 File Offset: 0x00001FF4
		// (set) Token: 0x06000081 RID: 129 RVA: 0x00003E16 File Offset: 0x00002016
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("testdeployroot=\\data\\test;testshare=\\\\winphonelabs\\securestorage\\Blue\\TestData;multimediashare=\\\\wdcmmcontent\\blue; testlabconfigdirectory=Internal;OSGTestContentShare=\\\\redmond\\1windows\\testcontent")]
		public string DefaultMacros
		{
			get
			{
				return (string)this["DefaultMacros"];
			}
			set
			{
				this["DefaultMacros"] = value;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00003E28 File Offset: 0x00002028
		// (set) Token: 0x06000083 RID: 131 RVA: 0x00003E4A File Offset: 0x0000204A
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("3600000")]
		public int CacheTimeoutInMs
		{
			get
			{
				return (int)this["CacheTimeoutInMs"];
			}
			set
			{
				this["CacheTimeoutInMs"] = value;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00003E60 File Offset: 0x00002060
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00003E82 File Offset: 0x00002082
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("1")]
		public int CacheExpirationInDays
		{
			get
			{
				return (int)this["CacheExpirationInDays"];
			}
			set
			{
				this["CacheExpirationInDays"] = value;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000086 RID: 134 RVA: 0x00003E98 File Offset: 0x00002098
		// (set) Token: 0x06000087 RID: 135 RVA: 0x00003EBA File Offset: 0x000020BA
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("4")]
		public int MaxConcurrentDownloads
		{
			get
			{
				return (int)this["MaxConcurrentDownloads"];
			}
			set
			{
				this["MaxConcurrentDownloads"] = value;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00003ED0 File Offset: 0x000020D0
		// (set) Token: 0x06000089 RID: 137 RVA: 0x00003EF2 File Offset: 0x000020F2
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("8")]
		public int MaxConcurrentLocalCopies
		{
			get
			{
				return (int)this["MaxConcurrentLocalCopies"];
			}
			set
			{
				this["MaxConcurrentLocalCopies"] = value;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600008A RID: 138 RVA: 0x00003F08 File Offset: 0x00002108
		// (set) Token: 0x0600008B RID: 139 RVA: 0x00003F2A File Offset: 0x0000212A
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("3")]
		public int CopyRetryCount
		{
			get
			{
				return (int)this["CopyRetryCount"];
			}
			set
			{
				this["CopyRetryCount"] = value;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00003F40 File Offset: 0x00002140
		// (set) Token: 0x0600008D RID: 141 RVA: 0x00003F62 File Offset: 0x00002162
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("3000")]
		public int CopyRetryDelayInMs
		{
			get
			{
				return (int)this["CopyRetryDelayInMs"];
			}
			set
			{
				this["CopyRetryDelayInMs"] = value;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00003F78 File Offset: 0x00002178
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00003F9A File Offset: 0x0000219A
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("96")]
		public int MaxConcurrentReaders
		{
			get
			{
				return (int)this["MaxConcurrentReaders"];
			}
			set
			{
				this["MaxConcurrentReaders"] = value;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000090 RID: 144 RVA: 0x00003FB0 File Offset: 0x000021B0
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00003FD2 File Offset: 0x000021D2
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("Microsoft.Phone.CacheManager.DownloadSemaphoreName")]
		public string DownloadSemaphoreName
		{
			get
			{
				return (string)this["DownloadSemaphoreName"];
			}
			set
			{
				this["DownloadSemaphoreName"] = value;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00003FE4 File Offset: 0x000021E4
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00004006 File Offset: 0x00002206
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("Microsoft.Phone.CacheManager.LocalCopySemaphoreName")]
		public string LocalCopySemaphoreName
		{
			get
			{
				return (string)this["LocalCopySemaphoreName"];
			}
			set
			{
				this["LocalCopySemaphoreName"] = value;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00004018 File Offset: 0x00002218
		// (set) Token: 0x06000095 RID: 149 RVA: 0x0000403A File Offset: 0x0000223A
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("\\\\winbuilds\\release\\")]
		public string WinBuildSharePrefix
		{
			get
			{
				return (string)this["WinBuildSharePrefix"];
			}
			set
			{
				this["WinBuildSharePrefix"] = value;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000096 RID: 150 RVA: 0x0000404C File Offset: 0x0000224C
		// (set) Token: 0x06000097 RID: 151 RVA: 0x0000406E File Offset: 0x0000226E
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("\\\\build\\release\\")]
		public string PhoneBuildSharePrefix
		{
			get
			{
				return (string)this["PhoneBuildSharePrefix"];
			}
			set
			{
				this["PhoneBuildSharePrefix"] = value;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00004080 File Offset: 0x00002280
		// (set) Token: 0x06000099 RID: 153 RVA: 0x000040A2 File Offset: 0x000022A2
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("3")]
		public int ShareAccessRetryCount
		{
			get
			{
				return (int)this["ShareAccessRetryCount"];
			}
			set
			{
				this["ShareAccessRetryCount"] = value;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600009A RID: 154 RVA: 0x000040B8 File Offset: 0x000022B8
		// (set) Token: 0x0600009B RID: 155 RVA: 0x000040DA File Offset: 0x000022DA
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("1000")]
		public int ShareAccessRetryDelayInMs
		{
			get
			{
				return (int)this["ShareAccessRetryDelayInMs"];
			}
			set
			{
				this["ShareAccessRetryDelayInMs"] = value;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600009C RID: 156 RVA: 0x000040F0 File Offset: 0x000022F0
		// (set) Token: 0x0600009D RID: 157 RVA: 0x00004112 File Offset: 0x00002312
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("bldinfo;sdp")]
		public string ExcludedSubDirsUnderPrebuiltPath
		{
			get
			{
				return (string)this["ExcludedSubDirsUnderPrebuiltPath"];
			}
			set
			{
				this["ExcludedSubDirsUnderPrebuiltPath"] = value;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00004124 File Offset: 0x00002324
		// (set) Token: 0x0600009F RID: 159 RVA: 0x00004146 File Offset: 0x00002346
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("30")]
		public int LocationCacheExpiresInDays
		{
			get
			{
				return (int)this["LocationCacheExpiresInDays"];
			}
			set
			{
				this["LocationCacheExpiresInDays"] = value;
			}
		}

		// Token: 0x04000046 RID: 70
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
