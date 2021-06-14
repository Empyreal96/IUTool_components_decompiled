using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.WindowsPhone.Security.SecurityPolicySchema
{
	// Token: 0x02000002 RID: 2
	public static class SchemaStrings
	{
		// Token: 0x04000001 RID: 1
		public const string Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00";

		// Token: 0x04000002 RID: 2
		public const string ElementRoot = "PhoneSecurityPolicy";

		// Token: 0x04000003 RID: 3
		public const string AttributeDescription = "Description";

		// Token: 0x04000004 RID: 4
		public const string AttributeVendor = "Vendor";

		// Token: 0x04000005 RID: 5
		public const string AttributeRequiredOSVersion = "RequiredOSVersion";

		// Token: 0x04000006 RID: 6
		public const string AttributeFileVersion = "FileVersion";

		// Token: 0x04000007 RID: 7
		public const string ElementMacros = "Macros";

		// Token: 0x04000008 RID: 8
		public const string ElementMacro = "Macro";

		// Token: 0x04000009 RID: 9
		public const string AttributeId = "Id";

		// Token: 0x0400000A RID: 10
		public const string AttributeValue = "Value";

		// Token: 0x0400000B RID: 11
		public const string ElementCapabilities = "Capabilities";

		// Token: 0x0400000C RID: 12
		public const string ElementCapability = "Capability";

		// Token: 0x0400000D RID: 13
		public const string AttributeFriendlyName = "FriendlyName";

		// Token: 0x0400000E RID: 14
		public const string AttributeVisibility = "Visibility";

		// Token: 0x0400000F RID: 15
		public const string ElementWindowsCapability = "WindowsCapability";

		// Token: 0x04000010 RID: 16
		public const string ElementCapabilityRules = "CapabilityRules";

		// Token: 0x04000011 RID: 17
		public const string ElementWindowsRules = "WindowsRules";

		// Token: 0x04000012 RID: 18
		public const string ElementFile = "File";

		// Token: 0x04000013 RID: 19
		public const string ElementDirectory = "Directory";

		// Token: 0x04000014 RID: 20
		public const string ElementRegKey = "RegKey";

		// Token: 0x04000015 RID: 21
		public const string ElementService = "Service";

		// Token: 0x04000016 RID: 22
		public const string ElementServiceAccess = "ServiceAccess";

		// Token: 0x04000017 RID: 23
		public const string ElementTransientObject = "TransientObject";

		// Token: 0x04000018 RID: 24
		public const string ElementPrivilege = "Privilege";

		// Token: 0x04000019 RID: 25
		public const string ElementDeviceSetupClass = "DeviceSetupClass";

		// Token: 0x0400001A RID: 26
		public const string ElementEtwProvider = "ETWProvider";

		// Token: 0x0400001B RID: 27
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Wnf", Justification = "Wnf is correct acronym for Windows Notification Framework")]
		public const string ElementWnf = "WNF";

		// Token: 0x0400001C RID: 28
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "COM", Justification = "COM is correct acronym")]
		public const string ElementCOM = "COM";

		// Token: 0x0400001D RID: 29
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "WinRT", Justification = "WinRT is correct acronym")]
		public const string ElementWinRT = "WinRT";

		// Token: 0x0400001E RID: 30
		public const string ElementSDRegValue = "SDRegValue";

		// Token: 0x0400001F RID: 31
		public const string AttributeSaveAsString = "SaveAsString";

		// Token: 0x04000020 RID: 32
		public const string AttributeSetOwner = "SetOwner";

		// Token: 0x04000021 RID: 33
		public const string ElementSecurity = "Security";

		// Token: 0x04000022 RID: 34
		public const string AttributeInfSectionName = "InfSectionName";

		// Token: 0x04000023 RID: 35
		public const string AttributeRuleTemplate = "RuleTemplate";

		// Token: 0x04000024 RID: 36
		public const string ElementAccessedByCapability = "AccessedByCapability";

		// Token: 0x04000025 RID: 37
		public const string ElementAccessedByService = "AccessedByService";

		// Token: 0x04000026 RID: 38
		public const string ElementAccessedByApplication = "AccessedByApplication";

		// Token: 0x04000027 RID: 39
		public const string AttributeRights = "Rights";

		// Token: 0x04000028 RID: 40
		public const string AttributePath = "Path";

		// Token: 0x04000029 RID: 41
		public const string AttributeSource = "Source";

		// Token: 0x0400002A RID: 42
		public const string AttributeDestinationDir = "DestinationDir";

		// Token: 0x0400002B RID: 43
		public const string AttributeName = "Name";

		// Token: 0x0400002C RID: 44
		public const string AttributeGuid = "Guid";

		// Token: 0x0400002D RID: 45
		public const string AttributeType = "Type";

		// Token: 0x0400002E RID: 46
		public const string AttributeAppId = "AppId";

		// Token: 0x0400002F RID: 47
		public const string AttributeServerName = "ServerName";

		// Token: 0x04000030 RID: 48
		public const string AttributeLaunchPermission = "LaunchPermission";

		// Token: 0x04000031 RID: 49
		public const string AttributeAccessPermission = "AccessPermission";

		// Token: 0x04000032 RID: 50
		public const string AttributeScope = "Scope";

		// Token: 0x04000033 RID: 51
		public const string AttributeTag = "Tag";

		// Token: 0x04000034 RID: 52
		public const string AttributeSequence = "Sequence";

		// Token: 0x04000035 RID: 53
		public const string AttributeDataPermanent = "DataPermanent";

		// Token: 0x04000036 RID: 54
		public const string AttributeReadOnly = "ReadOnly";

		// Token: 0x04000037 RID: 55
		public const string ElementComponents = "Components";

		// Token: 0x04000038 RID: 56
		public const string ElementApplication = "Application";

		// Token: 0x04000039 RID: 57
		public const string ElementFiles = "Files";

		// Token: 0x0400003A RID: 58
		public const string ElementExecutable = "Executable";

		// Token: 0x0400003B RID: 59
		public const string ElementAppResource = "AppResource";

		// Token: 0x0400003C RID: 60
		public const string ElementRequiredCapabilities = "RequiredCapabilities";

		// Token: 0x0400003D RID: 61
		public const string ElementRequiredCapability = "RequiredCapability";

		// Token: 0x0400003E RID: 62
		public const string AttributeCapId = "CapId";

		// Token: 0x0400003F RID: 63
		public const string ElementPrivateResources = "PrivateResources";

		// Token: 0x04000040 RID: 64
		public const string AttributeSvcHostGroupName = "SvcHostGroupName";

		// Token: 0x04000041 RID: 65
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "TCB", Justification = "TCB is correct acronym")]
		public const string AttributeIsTCB = "IsTCB";

		// Token: 0x04000042 RID: 66
		public const string AttributeLogonAccount = "LogonAccount";

		// Token: 0x04000043 RID: 67
		public const string AttributeOEMExtensible = "OEMExtensible";

		// Token: 0x04000044 RID: 68
		public const string ElementServiceDll = "ServiceDll";

		// Token: 0x04000045 RID: 69
		public const string AttributeHostExe = "HostExe";

		// Token: 0x04000046 RID: 70
		public const string ElementDriverRule = "DriverRule";

		// Token: 0x04000047 RID: 71
		public const string ElementAuthorization = "Authorization";

		// Token: 0x04000048 RID: 72
		public const string ElementPrincipalClass = "PrincipalClass";

		// Token: 0x04000049 RID: 73
		public const string ElementExecutables = "Executables";

		// Token: 0x0400004A RID: 74
		public const string ElementDirectories = "Directories";

		// Token: 0x0400004B RID: 75
		public const string ElementCertificates = "Certificates";

		// Token: 0x0400004C RID: 76
		public const string ElementCertificate = "Certificate";

		// Token: 0x0400004D RID: 77
		public const string ElementChambers = "Chambers";

		// Token: 0x0400004E RID: 78
		public const string ElementChamber = "Chamber";

		// Token: 0x0400004F RID: 79
		public const string ElementCapabilityClass = "CapabilityClass";

		// Token: 0x04000050 RID: 80
		public const string ElementMemberCapability = "MemberCapability";

		// Token: 0x04000051 RID: 81
		public const string ElementMemberCapabilityClass = "MemberCapabilityClass";

		// Token: 0x04000052 RID: 82
		public const string ElementExecuteRule = "ExecuteRule";

		// Token: 0x04000053 RID: 83
		public const string ElementCapabilityRule = "CapabilityRule";

		// Token: 0x04000054 RID: 84
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "EKU", Justification = "EKU is correct acronym")]
		public const string AttributeEKU = "EKU";

		// Token: 0x04000055 RID: 85
		public const string AttributeThumbprint = "Thumbprint";

		// Token: 0x04000056 RID: 86
		public const string AttributeThumbprintAlgorithm = "Alg";

		// Token: 0x04000057 RID: 87
		public const string AttributePrincipalClass = "PrincipalClass";

		// Token: 0x04000058 RID: 88
		public const string AttributeTargetChamber = "TargetChamber";

		// Token: 0x04000059 RID: 89
		public const string AttributeCapabilityClass = "CapabilityClass";

		// Token: 0x0400005A RID: 90
		public const string AttributeSvcOwnProcess = "Win32OwnProcess";

		// Token: 0x0400005B RID: 91
		public const string ElementFullTrust = "FullTrust";

		// Token: 0x0400005C RID: 92
		public const string AttributeSkip = "Skip";

		// Token: 0x0400005D RID: 93
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SID", Justification = "SID is correct acronym")]
		public const string AttributeSID = "SID";
	}
}
