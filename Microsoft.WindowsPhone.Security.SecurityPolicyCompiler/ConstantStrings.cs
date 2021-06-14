using System;
using System.Text.RegularExpressions;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000002 RID: 2
	public static class ConstantStrings
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static string[] GetValidCapabilityVisibilityList()
		{
			return ConstantStrings.capabilityVisibilityList;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
		public static string[] GetPackageAllowList()
		{
			return ConstantStrings.packageAllowList;
		}

		// Token: 0x04000001 RID: 1
		public const string Extension = ".XML";

		// Token: 0x04000002 RID: 2
		public const string WinPhoneRoot = "_WINPHONEROOT";

		// Token: 0x04000003 RID: 3
		public const string Namespace = "WP_Policy";

		// Token: 0x04000004 RID: 4
		public const string NodePathHelper = "/WP_Policy:";

		// Token: 0x04000005 RID: 5
		public const string NodePathRoot = "/WP_Policy:PhoneSecurityPolicy";

		// Token: 0x04000006 RID: 6
		public const string NodePathMacro = "//WP_Policy:Macros/WP_Policy:Macro";

		// Token: 0x04000007 RID: 7
		public const string NodePathCapabilities = "//WP_Policy:Capabilities";

		// Token: 0x04000008 RID: 8
		public const string NodePathComponents = "//WP_Policy:Components";

		// Token: 0x04000009 RID: 9
		public const string NodePathAuthorization = "//WP_Policy:Authorization";

		// Token: 0x0400000A RID: 10
		public const string ServiceOwnProcess = "Win32OwnProcess";

		// Token: 0x0400000B RID: 11
		public const string ServiceSharedProcess = "Win32ShareProcess";

		// Token: 0x0400000C RID: 12
		public const string NodeService = "./WP_Policy:Service[@Type='Win32OwnProcess' or @Type='Win32ShareProcess']";

		// Token: 0x0400000D RID: 13
		public const string NodeFullTrust = "./WP_Policy:FullTrust";

		// Token: 0x0400000E RID: 14
		public const string NoAttributeValue = "No";

		// Token: 0x0400000F RID: 15
		public const string MacroPrefixCharacters = "$(";

		// Token: 0x04000010 RID: 16
		public const string MacroPostfixCharacters = ")";

		// Token: 0x04000011 RID: 17
		public const string DefaultAccountName000 = "DA0";

		// Token: 0x04000012 RID: 18
		public const string InstallationFolderPath = "\\PROGRAMS\\{0}\\(*)";

		// Token: 0x04000013 RID: 19
		public const string ElementInstallationFolder = "InstallationFolder";

		// Token: 0x04000014 RID: 20
		public const string InstallationFolderRight = "0x001200A9";

		// Token: 0x04000015 RID: 21
		public const string OwnerRight = "OW";

		// Token: 0x04000016 RID: 22
		public const string ChamberProfileDataDefaultFolder = "ChamberProfileDataDefaultFolder";

		// Token: 0x04000017 RID: 23
		public const string ChamberProfileDataShellContentFolder = "ChamberProfileDataShellContentFolder";

		// Token: 0x04000018 RID: 24
		public const string ChamberProfileDataMediaFolder = "ChamberProfileDataMediaFolder";

		// Token: 0x04000019 RID: 25
		public const string ChamberProfileDataPlatformDataFolder = "ChamberProfileDataPlatformDataFolder";

		// Token: 0x0400001A RID: 26
		public const string ChamberProfileDataLiveTilesFolder = "ChamberProfileDataLiveTilesFolder";

		// Token: 0x0400001B RID: 27
		public const string ApplicationDefaultDataFolderPath = "\\DATA\\{0}\\{1}\\(*)";

		// Token: 0x0400001C RID: 28
		public const string ApplicationShellContentFolderPath = "\\DATA\\{0}\\{1}\\Local\\Shared\\ShellContent\\(*)";

		// Token: 0x0400001D RID: 29
		public const string ApplicationMediaFolderPath = "\\DATA\\{0}\\{1}\\Local\\Shared\\Media\\(*)";

		// Token: 0x0400001E RID: 30
		public const string ApplicationPlatformDataFolderPath = "\\DATA\\{0}\\{1}\\PlatformData\\(*)";

		// Token: 0x0400001F RID: 31
		public const string ApplicationLiveTilesFolderPath = "\\DATA\\{0}\\{1}\\PlatformData\\LiveTiles\\(*)";

		// Token: 0x04000020 RID: 32
		public const string ApplicationFolderRRight = "0x001200A9";

		// Token: 0x04000021 RID: 33
		public const string ApplicationFolderRWRight = "0x001201bf";

		// Token: 0x04000022 RID: 34
		public const string ApplicationFolderRWDRight = "0x001301bf";

		// Token: 0x04000023 RID: 35
		public const string ApplicationFolderAllRights = "0x001301ff";

		// Token: 0x04000024 RID: 36
		public const string ApplicationFolderFullRight = "FA";

		// Token: 0x04000025 RID: 37
		public const string System = "SY";

		// Token: 0x04000026 RID: 38
		public const string CapabilityChamberProfileRead = "ID_CAP_CHAMBER_PROFILE_CODE_R";

		// Token: 0x04000027 RID: 39
		public const string CapabilityChamberProfileWrite = "ID_CAP_CHAMBER_PROFILE_CODE_RW";

		// Token: 0x04000028 RID: 40
		public const string CapabilityChamberProfileTempInstall = "ID_CAP_CHAMBER_PROFILE_CODE_INSTALLTEMP_RWD";

		// Token: 0x04000029 RID: 41
		public const string CapabilityChamberProfileTempNi = "ID_CAP_CHAMBER_PROFILE_CODE_NITEMP_RW";

		// Token: 0x0400002A RID: 42
		public const string CapabilityChamberProfileDataRead = "ID_CAP_CHAMBER_PROFILE_DATA_R";

		// Token: 0x0400002B RID: 43
		public const string CapabilityChamberProfileDataWrite = "ID_CAP_CHAMBER_PROFILE_DATA_RW";

		// Token: 0x0400002C RID: 44
		public const string CapabilityChamberProfileShellContentRead = "ID_CAP_CHAMBER_PROFILE_DATA_SHELLCONTENT_R";

		// Token: 0x0400002D RID: 45
		public const string CapabilityChamberProfileShellContentAll = "ID_CAP_CHAMBER_PROFILE_DATA_SHELLCONTENT_RWD";

		// Token: 0x0400002E RID: 46
		public const string CapabilityChamberProfileMediaAll = "ID_CAP_CHAMBER_PROFILE_DATA_MEDIA_RWD";

		// Token: 0x0400002F RID: 47
		public const string CapabilityChamberProfilePlatformDataAll = "ID_CAP_CHAMBER_PROFILE_DATA_PLATFORMDATA_ALL";

		// Token: 0x04000030 RID: 48
		public const string CapabilityChamberProfileLiveTilesAll = "ID_CAP_CHAMBER_PROFILE_DATA_LIVETILES_RWD";

		// Token: 0x04000031 RID: 49
		public const string RequiredInheritancePostfix = "\\(*)";

		// Token: 0x04000032 RID: 50
		public const string RequiredInheritanceOnlyPostfix = "\\(+)";

		// Token: 0x04000033 RID: 51
		public const string RegKeyRuleInheritanceFlag = "CI";

		// Token: 0x04000034 RID: 52
		public const string DirectoryRuleInheritanceFlag = "CIOI";

		// Token: 0x04000035 RID: 53
		public const string DirectoryRuleInheritanceOnlyFlag = "IOCIOI";

		// Token: 0x04000036 RID: 54
		public const string ApplicationCapabilitySIDPrefix = "S-1-15-3-1024";

		// Token: 0x04000037 RID: 55
		public const string ServiceCapabilitySIDPrefix = "S-1-5-21-2702878673-795188819-444038987";

		// Token: 0x04000038 RID: 56
		public const string WindowsCapabilitySIDPrefix = "S-1-15-3";

		// Token: 0x04000039 RID: 57
		public const int ServiceCapabilityStartRID = 1031;

		// Token: 0x0400003A RID: 58
		public const int NumberOfServiceCapabilities = 1750;

		// Token: 0x0400003B RID: 59
		public const string AllApplicationCapabilityGroupSID = "S-1-5-21-2702878673-795188819-444038987-1030";

		// Token: 0x0400003C RID: 60
		public const string DefAppsAccountSID = "S-1-5-21-2702878673-795188819-444038987-2781";

		// Token: 0x0400003D RID: 61
		public const string TrustedInstallerSID = "S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";

		// Token: 0x0400003E RID: 62
		public const string DataSharingServiceSID = "S-1-5-80-1551822644-3134808374-1042292604-2865742758-3851661496";

		// Token: 0x0400003F RID: 63
		public const string PolicyCompilerTestFlag = "POLICY_COMPILER_TEST";

		// Token: 0x04000040 RID: 64
		public const string CapabilityVisibilityPrivate = "Private";

		// Token: 0x04000041 RID: 65
		public const string CapabilityVisibilityInternal = "Internal";

		// Token: 0x04000042 RID: 66
		public const string CapabilityVisibilityPublic = "Public";

		// Token: 0x04000043 RID: 67
		private static string[] capabilityVisibilityList = new string[]
		{
			"Public",
			"Internal",
			"Private"
		};

		// Token: 0x04000044 RID: 68
		public const string ApplicationSIDPrefix = "S-1-15-2";

		// Token: 0x04000045 RID: 69
		public const string ServiceSIDPrefix = "S-1-5-80";

		// Token: 0x04000046 RID: 70
		public const string BuiltinUsers = "BU";

		// Token: 0x04000047 RID: 71
		public const string WriteRestrictedSID = "S-1-5-33";

		// Token: 0x04000048 RID: 72
		public const string NtServicesCapability = "ID_CAP_NTSERVICES";

		// Token: 0x04000049 RID: 73
		public static string[] PredefinedServiceCapabilities = new string[]
		{
			"ID_CAP_NTSERVICES"
		};

		// Token: 0x0400004A RID: 74
		public const string DACLFormat = "(A;{0};{1};;;{2})";

		// Token: 0x0400004B RID: 75
		public const string RuleString = "Rule";

		// Token: 0x0400004C RID: 76
		public static string[] ComponentCapabilityIdFilter = new string[]
		{
			"ID_CAP_EVERYONE",
			"ID_CAP_BUILTIN_DEFAULT",
			"ID_CAP_EVERYONE_INROM"
		};

		// Token: 0x0400004D RID: 77
		public static string[] ServiceCapabilityIdFilter = new string[]
		{
			"ID_CAP_BUILTIN_CREATEGLOBAL"
		};

		// Token: 0x0400004E RID: 78
		public static string EveryoneCapability = "ID_CAP_EVERYONE";

		// Token: 0x0400004F RID: 79
		public static string[] BlockedCapabilityIdForApplicationFilter = new string[]
		{
			"ID_CAP_BUILTIN_TCB",
			"ID_CAP_BUILTIN_SYMBOLICLINK",
			"ID_CAP_BUILTIN_IMPERSONATE"
		};

		// Token: 0x04000050 RID: 80
		public const string WindowsRulesIdPrefix = "ID_CAP_WINRULES_";

		// Token: 0x04000051 RID: 81
		public const string PrivateResourcesIdPrefix = "ID_CAP_PRIV_";

		// Token: 0x04000052 RID: 82
		public const string PrivateResourcesFriendlyNamePostfix = " private capability";

		// Token: 0x04000053 RID: 83
		public const string PrivateResourcesRights = "$(ALL_ACCESS)";

		// Token: 0x04000054 RID: 84
		public const string PrivateResourcesReadOnlyRights = "$(GENERIC_READ)";

		// Token: 0x04000055 RID: 85
		public const string PrivateResourcesServiceAccessRights = "$(SERVICE_PRIVATE_RESOURCE_ACCESS)";

		// Token: 0x04000056 RID: 86
		public const string PrivateResourcesCOMAccessRuleRights = "$(COM_LOCAL_ACCESS)";

		// Token: 0x04000057 RID: 87
		public const string PrivateResourcesCOMLaunchRuleRights = "$(COM_LOCAL_LAUNCH)";

		// Token: 0x04000058 RID: 88
		public static Regex[] BlockedFilePathRegexes = new Regex[]
		{
			new Regex("^\\\\CRASHDUMP($|\\\\.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
			new Regex("^\\\\DATA$", RegexOptions.IgnoreCase | RegexOptions.Compiled),
			new Regex("^\\\\DPP($|\\\\.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
			new Regex("^\\\\EFIESP($|\\\\.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
			new Regex("^\\\\MMOS($|\\\\.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
			new Regex("^\\\\WINDOWS\\\\SERVICEPROFILES($|\\\\.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
			new Regex("^\\\\SDCARD($|\\\\.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled)
		};

		// Token: 0x04000059 RID: 89
		public static Regex[] BlockedRegPathRegexes = new Regex[]
		{
			new Regex("^HKEY_LOCAL_MACHINE\\\\SYSTEM\\\\CONTROLSET001\\\\SERVICES\\\\[A-Z0-9_-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled),
			new Regex("^HKEY_LOCAL_MACHINE\\\\SYSTEM\\\\CONTROLSET001\\\\SERVICES\\\\[A-Z0-9_-]+\\\\PARAMETERS$", RegexOptions.IgnoreCase | RegexOptions.Compiled)
		};

		// Token: 0x0400005A RID: 90
		public static string[] AllowedRegPaths = new string[]
		{
			"HKEY_LOCAL_MACHINE\\SYSTEM\\CONTROLSET001\\SERVICES\\TCPIP\\PARAMETERS",
			"HKEY_LOCAL_MACHINE\\SYSTEM\\CONTROLSET001\\SERVICES\\W32TIME\\PARAMETERS"
		};

		// Token: 0x0400005B RID: 91
		public const string ServiceAccessRulePathPrefix = "HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\services\\";

		// Token: 0x0400005C RID: 92
		public const string DeviceSetupClassRulePathPrefix = "HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\Control\\Class\\";

		// Token: 0x0400005D RID: 93
		public const string COMRulePathPrefix = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Classes\\AppID\\";

		// Token: 0x0400005E RID: 94
		public const string WinRTRulePathPrefix = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\WindowsRuntime\\Server\\";

		// Token: 0x0400005F RID: 95
		public const string EtwProviderPathPrefix = "HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\Control\\WMI\\Security\\";

		// Token: 0x04000060 RID: 96
		public const string WnfPathPrefix = "HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\Control\\Notifications\\";

		// Token: 0x04000061 RID: 97
		public const string ServiceAccessRulePathPostfix = "\\Security\\Security";

		// Token: 0x04000062 RID: 98
		public const string DeviceSetupClassRulePathPostfix = "\\Properties\\Security";

		// Token: 0x04000063 RID: 99
		public const string COMRuleAccessPermissionPathPostfix = "\\AccessPermission";

		// Token: 0x04000064 RID: 100
		public const string COMRuleLaunchPermissionPathPostfix = "\\LaunchPermission";

		// Token: 0x04000065 RID: 101
		public const string WinRTRulePermissionsPathPostfix = "\\Permissions";

		// Token: 0x04000066 RID: 102
		public const string COMRuleLaunchPermissionTag = "COMLaunchPermission";

		// Token: 0x04000067 RID: 103
		public const string COMRuleAccessPermissionTag = "COMAccessPermission";

		// Token: 0x04000068 RID: 104
		public const string WinRTRuleLaunchPermissionTag = "WinRTLaunchPermission";

		// Token: 0x04000069 RID: 105
		public const string WinRTRuleAccessPermissionTag = "WinRTAccessPermission";

		// Token: 0x0400006A RID: 106
		public const string TransientObjectPathPrefix = "%5C%5C.%5C";

		// Token: 0x0400006B RID: 107
		public const string TransientObjectPathSeparator = "%5C";

		// Token: 0x0400006C RID: 108
		public const string TransientObjectPathOriginalSeparator = "\\";

		// Token: 0x0400006D RID: 109
		public const string AttributeNotSet = null;

		// Token: 0x0400006E RID: 110
		public const string AttributeNotCalculated = "Not Calculated";

		// Token: 0x0400006F RID: 111
		public const string DefaultHashType = "Sha256";

		// Token: 0x04000070 RID: 112
		public const string DefaultServiceExecutableAttribute = "$(RUNTIME.SYSTEM32)\\SvcHost.exe";

		// Token: 0x04000071 RID: 113
		public const string DefaultServiceIsTCBAttribute = "No";

		// Token: 0x04000072 RID: 114
		private static string[] packageAllowList = new string[]
		{
			"Microsoft.BaseOS.SecurityModel",
			"Microsoft.BaseOS.CoreSecurityPolicy"
		};

		// Token: 0x04000073 RID: 115
		private const int IndentationLength = 3;

		// Token: 0x04000074 RID: 116
		public const string IndentationLevel0 = "";

		// Token: 0x04000075 RID: 117
		public static readonly string IndentationLevel1 = string.Format(GlobalVariables.Culture, "{0}{1}", new object[]
		{
			"",
			new string(' ', 3)
		});

		// Token: 0x04000076 RID: 118
		public static readonly string IndentationLevel2 = string.Format(GlobalVariables.Culture, "{0}{1}", new object[]
		{
			"",
			new string(' ', 6)
		});

		// Token: 0x04000077 RID: 119
		public static readonly string IndentationLevel3 = string.Format(GlobalVariables.Culture, "{0}{1}", new object[]
		{
			"",
			new string(' ', 9)
		});

		// Token: 0x04000078 RID: 120
		public static readonly string IndentationLevel4 = string.Format(GlobalVariables.Culture, "{0}{1}", new object[]
		{
			"",
			new string(' ', 12)
		});

		// Token: 0x04000079 RID: 121
		public static readonly string IndentationLevel5 = string.Format(GlobalVariables.Culture, "{0}{1}", new object[]
		{
			"",
			new string(' ', 15)
		});

		// Token: 0x0400007A RID: 122
		public const string ErrorMessagePrefix = "Error: CompileSecurityPolicy {0}";

		// Token: 0x0400007B RID: 123
		public const string DebugMessagePrefix = "Debug: ";

		// Token: 0x0400007C RID: 124
		public const string OutputFilePrintElementFormat = "{0}{1}";

		// Token: 0x0400007D RID: 125
		public const string OutputFilePrintAttributeFormat = "{0}{1}=\"{2}\"";

		// Token: 0x0400007E RID: 126
		public const string MacroReferencingErrorMessage = "Macro Referencing Error: {0}, Value= {1}";

		// Token: 0x0400007F RID: 127
		public const string MacroDefinitionNotFoundErrorMessage = "Macro Definition Not Found: {0}, Value= {1}";

		// Token: 0x04000080 RID: 128
		public const string ElementAndAttributeFormat = "Element={0}, Attribute={1}";

		// Token: 0x04000081 RID: 129
		public const string ElementTwoAttributeExclusiveMessageFormat = "Element={0} '{1}', Attributes {2} and {3} are mutually exclusive";

		// Token: 0x04000082 RID: 130
		public const string ReadOnlyViolationErrorMessage = "It is not allowed to grants write access on '{0}'. Only the folders under \\DATA can be granted write access.";

		// Token: 0x04000083 RID: 131
		public const string DirectoryOnlyErrorMessage = "It is not allowed to define a capability rule for a file '{0}' under \\DATA, only Directory rule is allowed.";

		// Token: 0x04000084 RID: 132
		public const string CapabilityDefinitionErrorMessage = "The package definition file should not have capability definition";

		// Token: 0x04000085 RID: 133
		public const string ServiceChangeConfigViolationErrorMessage = "It is not allowed to grant SERVICE_CHANGE_CONFIG on service '{0}'.";

		// Token: 0x04000086 RID: 134
		public const string BlockedCapabilityIdForApplicationErrorMessage = "The capability '{0}' can't be used in application.";

		// Token: 0x04000087 RID: 135
		public const string UnsupportedUserRegKeyErrorMessage = "It is not allowed to define a capability rule or private resource for registry key '{0}' under HKEY_USERS or HKEY_CURRENT_USER.";

		// Token: 0x04000088 RID: 136
		public const string CoexistenceErrorMessage = "'{0}' and '{1}' can't be present in the same time in service '{2}'.";

		// Token: 0x04000089 RID: 137
		public const string PrivateResourcesDefinitionErrorMessage = "The private resources can't be defined in this package.";

		// Token: 0x0400008A RID: 138
		public const string BlockedPathErrorMessage = "'{0}' should not be defined in capability rule or private resource.";

		// Token: 0x0400008B RID: 139
		public const string UnsupportedRegKeyErrorMessage = "It is not allowed to define a capability rule or private resource for write access on registry key '{0}'.";
	}
}
