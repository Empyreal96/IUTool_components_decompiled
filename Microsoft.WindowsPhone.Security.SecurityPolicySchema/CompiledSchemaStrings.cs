using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.WindowsPhone.Security.SecurityPolicySchema
{
	// Token: 0x02000003 RID: 3
	public static class CompiledSchemaStrings
	{
		// Token: 0x0400005E RID: 94
		public const string Namespace = "urn:Microsoft.WindowsPhone/PhoneSecurityPolicyInternal.v8.00";

		// Token: 0x0400005F RID: 95
		public const string ElementRoot = "PhoneSecurityPolicy";

		// Token: 0x04000060 RID: 96
		public const string AttributeDescription = "Description";

		// Token: 0x04000061 RID: 97
		public const string AttributeVendor = "Vendor";

		// Token: 0x04000062 RID: 98
		public const string AttributeRequiredOSVersion = "RequiredOSVersion";

		// Token: 0x04000063 RID: 99
		public const string AttributeFileVersion = "FileVersion";

		// Token: 0x04000064 RID: 100
		public const string AttributePackageId = "PackageID";

		// Token: 0x04000065 RID: 101
		public const string AttributeHashType = "HashType";

		// Token: 0x04000066 RID: 102
		public const string ElementRules = "Rules";

		// Token: 0x04000067 RID: 103
		public const string ElementRule = "Rule";

		// Token: 0x04000068 RID: 104
		public const string ElementSDRegValue = "SDRegValue";

		// Token: 0x04000069 RID: 105
		public const string AttributeSaveAsString = "SaveAsString";

		// Token: 0x0400006A RID: 106
		public const string AttributeProtected = "Protected";

		// Token: 0x0400006B RID: 107
		public const string AttributeOwner = "Owner";

		// Token: 0x0400006C RID: 108
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "DACL", Justification = "DACL is correct acronym")]
		public const string AttributeDACL = "DACL";

		// Token: 0x0400006D RID: 109
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SACL", Justification = "SACL is correct acronym")]
		public const string AttributeSACL = "SACL";

		// Token: 0x0400006E RID: 110
		public const string AttributeAttributeHash = "AttributeHash";

		// Token: 0x0400006F RID: 111
		public const string AttributeElementId = "ElementID";

		// Token: 0x04000070 RID: 112
		public const string ElementCapabilities = "Capabilities";

		// Token: 0x04000071 RID: 113
		public const string ElementCapability = "Capability";

		// Token: 0x04000072 RID: 114
		public const string AttributeFriendlyName = "FriendlyName";

		// Token: 0x04000073 RID: 115
		public const string AttributeVisibility = "Visibility";

		// Token: 0x04000074 RID: 116
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SID", Justification = "SID is correct acronym")]
		public const string AttributeAppCapSID = "AppCapSID";

		// Token: 0x04000075 RID: 117
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SID", Justification = "SID is correct acronym")]
		public const string AttributeSvcCapSID = "SvcCapSID";

		// Token: 0x04000076 RID: 118
		public const string ElementWindowsCapability = "WindowsCapability";

		// Token: 0x04000077 RID: 119
		public const string ElementCapabilityRules = "CapabilityRules";

		// Token: 0x04000078 RID: 120
		public const string ElementFile = "File";

		// Token: 0x04000079 RID: 121
		public const string ElementDirectory = "Directory";

		// Token: 0x0400007A RID: 122
		public const string ElementRegKey = "RegKey";

		// Token: 0x0400007B RID: 123
		public const string ElementTransientObject = "TransientObject";

		// Token: 0x0400007C RID: 124
		public const string ElementPrivilege = "Privilege";

		// Token: 0x0400007D RID: 125
		public const string ElementDeviceSetupClass = "DeviceSetupClass";

		// Token: 0x0400007E RID: 126
		public const string ElementServiceAccess = "ServiceAccess";

		// Token: 0x0400007F RID: 127
		public const string ElementEtwProvider = "ETWProvider";

		// Token: 0x04000080 RID: 128
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Wnf", Justification = "Wnf is correct acronym for Windows Notification Framework")]
		public const string ElementWnf = "WNF";

		// Token: 0x04000081 RID: 129
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "COM", Justification = "COM is correct acronym")]
		public const string ElementCOM = "COM";

		// Token: 0x04000082 RID: 130
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "WinRT", Justification = "WinRT is correct acronym")]
		public const string ElementWinRT = "WinRT";

		// Token: 0x04000083 RID: 131
		public const string AttributeRights = "Rights";

		// Token: 0x04000084 RID: 132
		public const string AttributeFlags = "Flags";

		// Token: 0x04000085 RID: 133
		public const string AttributePath = "Path";

		// Token: 0x04000086 RID: 134
		public const string AttributeName = "Name";

		// Token: 0x04000087 RID: 135
		public const string AttributeType = "Type";

		// Token: 0x04000088 RID: 136
		public const string AttributeGuid = "Guid";

		// Token: 0x04000089 RID: 137
		public const string AttributeAppId = "AppId";

		// Token: 0x0400008A RID: 138
		public const string AttributeServerName = "ServerName";

		// Token: 0x0400008B RID: 139
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SID", Justification = "SID is correct acronym")]
		public const string AttributeSID = "SID";

		// Token: 0x0400008C RID: 140
		public const string AttributeId = "Id";

		// Token: 0x0400008D RID: 141
		public const string AttributeLaunchPermission = "LaunchPermission";

		// Token: 0x0400008E RID: 142
		public const string AttributeAccessPermission = "AccessPermission";

		// Token: 0x0400008F RID: 143
		public const string ElementComponents = "Components";

		// Token: 0x04000090 RID: 144
		public const string ElementApplication = "Application";

		// Token: 0x04000091 RID: 145
		public const string ElementBinaries = "Binaries";

		// Token: 0x04000092 RID: 146
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SID", Justification = "SID is correct acronym")]
		public const string AttributePrivateCapSID = "PrivateCapSID";

		// Token: 0x04000093 RID: 147
		public const string ElementAppBinaries = "AppBinaries";

		// Token: 0x04000094 RID: 148
		public const string AttributeAppName = "AppName";

		// Token: 0x04000095 RID: 149
		public const string ElementBinary = "Binary";

		// Token: 0x04000096 RID: 150
		public const string AttributeBinaryId = "BinaryId";

		// Token: 0x04000097 RID: 151
		public const string ElementRequiredCapabilities = "RequiredCapabilities";

		// Token: 0x04000098 RID: 152
		public const string ElementRequiredCapability = "RequiredCapability";

		// Token: 0x04000099 RID: 153
		public const string AttributeCapId = "CapId";

		// Token: 0x0400009A RID: 154
		public const string ElementPrivateResources = "PrivateResources";

		// Token: 0x0400009B RID: 155
		public const string ElementService = "Service";

		// Token: 0x0400009C RID: 156
		public const string AttributeExecutable = "Executable";

		// Token: 0x0400009D RID: 157
		public const string AttributeSvcHostGroupName = "SvcHostGroupName";

		// Token: 0x0400009E RID: 158
		public const string AttributeSvcProcessOwnership = "OwnedProc";

		// Token: 0x0400009F RID: 159
		public const string AttributeSvcProcessOwnershipIsOwned = "Y";

		// Token: 0x040000A0 RID: 160
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "TCB", Justification = "TCB is correct acronym")]
		public const string AttributeIsTCB = "IsTCB";

		// Token: 0x040000A1 RID: 161
		public const string AttributeLogonAccount = "LogonAccount";

		// Token: 0x040000A2 RID: 162
		public const string AttributeOEMExtensible = "OEMExtensible";

		// Token: 0x040000A3 RID: 163
		public const string ElementAuthorization = "Authorization";

		// Token: 0x040000A4 RID: 164
		public const string ElementPrincipalClass = "PrincipalClass";

		// Token: 0x040000A5 RID: 165
		public const string ElementExecutables = "Executables";

		// Token: 0x040000A6 RID: 166
		public const string ElementExecutable = "Executable";

		// Token: 0x040000A7 RID: 167
		public const string ElementDirectories = "Directories";

		// Token: 0x040000A8 RID: 168
		public const string ElementCertificates = "Certificates";

		// Token: 0x040000A9 RID: 169
		public const string ElementCertificate = "Certificate";

		// Token: 0x040000AA RID: 170
		public const string ElementChambers = "Chambers";

		// Token: 0x040000AB RID: 171
		public const string ElementChamber = "Chamber";

		// Token: 0x040000AC RID: 172
		public const string ElementCapabilityClass = "CapabilityClass";

		// Token: 0x040000AD RID: 173
		public const string ElementMemberCapability = "MemberCapability";

		// Token: 0x040000AE RID: 174
		public const string ElementMemberCapabilityClass = "MemberCapabilityClass";

		// Token: 0x040000AF RID: 175
		public const string ElementExecuteRule = "ExecuteRule";

		// Token: 0x040000B0 RID: 176
		public const string ElementCapabilityRule = "CapabilityRule";

		// Token: 0x040000B1 RID: 177
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "EKU", Justification = "EKU is correct acronym")]
		public const string AttributeEKU = "EKU";

		// Token: 0x040000B2 RID: 178
		public const string AttributeThumbprint = "Thumbprint";

		// Token: 0x040000B3 RID: 179
		public const string AttributeThumbprintAlgorithm = "Alg";

		// Token: 0x040000B4 RID: 180
		public const string AttributePrincipalClass = "PrincipalClass";

		// Token: 0x040000B5 RID: 181
		public const string AttributeTargetChamber = "TargetChamber";

		// Token: 0x040000B6 RID: 182
		public const string AttributeCapabilityClass = "CapabilityClass";

		// Token: 0x040000B7 RID: 183
		public const string ElementFullTrust = "FullTrust";

		// Token: 0x040000B8 RID: 184
		public const string AttributeSkip = "Skip";
	}
}
