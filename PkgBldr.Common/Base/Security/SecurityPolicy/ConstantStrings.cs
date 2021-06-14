using System;
using System.Collections.Generic;

namespace Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy
{
	// Token: 0x02000049 RID: 73
	public static class ConstantStrings
	{
		// Token: 0x04000094 RID: 148
		public const string EveryoneCapabilityName = "everyone";

		// Token: 0x04000095 RID: 149
		public static Dictionary<string, string> LegacyApplicationCapabilityRids = new Dictionary<string, string>
		{
			{
				"internetClient",
				"1"
			},
			{
				"internetServer",
				"2"
			},
			{
				"privateNetworkClientServer",
				"3"
			},
			{
				"picturesLibrary",
				"4"
			},
			{
				"videosLibrary",
				"5"
			},
			{
				"musicLibrary",
				"6"
			},
			{
				"documentsLibrary",
				"7"
			},
			{
				"enterpriseAuthentication",
				"8"
			},
			{
				"sharedUserCertificates",
				"9"
			},
			{
				"removableStorage",
				"10"
			},
			{
				"appointments",
				"11"
			},
			{
				"contacts",
				"12"
			}
		};

		// Token: 0x04000096 RID: 150
		public const string AuthenticatedUsers = "AU";

		// Token: 0x04000097 RID: 151
		public const string AllApplicationPackages = "S-1-15-2-1";

		// Token: 0x04000098 RID: 152
		public const string InteractiveUsers = "IU";

		// Token: 0x04000099 RID: 153
		public const string ApplicationSidPrefix = "S-1-15-2";

		// Token: 0x0400009A RID: 154
		public const string ServiceSidPrefix = "S-1-5-80";

		// Token: 0x0400009B RID: 155
		public const string LegacyCapabilitySidPrefix = "S-1-15-3";

		// Token: 0x0400009C RID: 156
		public const string ApplicationCapabilitySidPrefix = "S-1-15-3-1024";

		// Token: 0x0400009D RID: 157
		public const string ServiceCapabilitySidPrefix = "S-1-5-32";

		// Token: 0x0400009E RID: 158
		public const string TrustedInstallerSid = "S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";

		// Token: 0x0400009F RID: 159
		public const string TrustedInstallerOwner = "O:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";

		// Token: 0x040000A0 RID: 160
		public const string TrustedInstallerGroup = "G:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";

		// Token: 0x040000A1 RID: 161
		public const string SystemOwner = "O:SY";

		// Token: 0x040000A2 RID: 162
		public const string SystemGroup = "G:SY";

		// Token: 0x040000A3 RID: 163
		public const string DaclPrefix = "D:";

		// Token: 0x040000A4 RID: 164
		public const string SaclPrefix = "S:";

		// Token: 0x040000A5 RID: 165
		public const string ProtectedAclFlag = "P";

		// Token: 0x040000A6 RID: 166
		public const string AutoInheritAclFlag = "AI";

		// Token: 0x040000A7 RID: 167
		public const string AutoInheritReqAclFlag = "AR";

		// Token: 0x040000A8 RID: 168
		public const string SystemAdminAllAccessNoInheritanceAce = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";

		// Token: 0x040000A9 RID: 169
		public const string ProtectedSystemAllAccessNoInheritanceAce = "P(A;;GA;;;SY)";

		// Token: 0x040000AA RID: 170
		public const string NoWriteUpLowLabelAce = "(ML;;NX;;;LW)";

		// Token: 0x040000AB RID: 171
		public const string PrivateResourceAccess = "0x111FFFFF";

		// Token: 0x040000AC RID: 172
		public const string PrivateResourceReadOnlyAccess = "GR";

		// Token: 0x040000AD RID: 173
		public const string ServiceAccessPrivateResourceAccess = "CCLCSWRPLO";

		// Token: 0x040000AE RID: 174
		public const string ComLaunchPrivateResourceAccess = "CCDCSW";

		// Token: 0x040000AF RID: 175
		public const string ComAccessPrivateResourceAccess = "CCDC";

		// Token: 0x040000B0 RID: 176
		public const string FileOwner = "O:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";

		// Token: 0x040000B1 RID: 177
		public const string FileGroup = "G:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";

		// Token: 0x040000B2 RID: 178
		public const string FileDefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";

		// Token: 0x040000B3 RID: 179
		public const string DirectoryInheritanceFlags = "CIOI";

		// Token: 0x040000B4 RID: 180
		public const string DirectoryOwner = "O:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";

		// Token: 0x040000B5 RID: 181
		public const string DirectoryGroup = "G:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";

		// Token: 0x040000B6 RID: 182
		public const string DirectoryDefaultDacl = "(A;CIOI;0x111FFFFF;;;CO)(A;CIOI;0x111FFFFF;;;SY)(A;CIOI;0x111FFFFF;;;BA)";

		// Token: 0x040000B7 RID: 183
		public const string RegistryInheritanceFlags = "CI";

		// Token: 0x040000B8 RID: 184
		public const string RegistryOwner = "O:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";

		// Token: 0x040000B9 RID: 185
		public const string RegistryGroup = "G:S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";

		// Token: 0x040000BA RID: 186
		public const string RegistryDefaultDacl = "(A;CI;0x111FFFFF;;;CO)(A;CI;0x111FFFFF;;;SY)(A;CI;0x111FFFFF;;;BA)";

		// Token: 0x040000BB RID: 187
		public const string TransientObjectDefaultDacl = "(A;;0x111FFFFF;;;CO)(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";

		// Token: 0x040000BC RID: 188
		public const string ServiceAccessOwner = "O:SY";

		// Token: 0x040000BD RID: 189
		public const string ServiceAccessGroup = "G:SY";

		// Token: 0x040000BE RID: 190
		public const string ServiceAccessDefaultDacl = "(A;;GRCR;;;IU)(A;;GRCR;;;SU)(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";

		// Token: 0x040000BF RID: 191
		public const string ComOwner = "O:SY";

		// Token: 0x040000C0 RID: 192
		public const string ComGroup = "G:SY";

		// Token: 0x040000C1 RID: 193
		public const string ComDefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";

		// Token: 0x040000C2 RID: 194
		public const string ComDefaultSacl = "(ML;;NX;;;LW)";

		// Token: 0x040000C3 RID: 195
		public const string WinRtOwner = "O:SY";

		// Token: 0x040000C4 RID: 196
		public const string WinRtGroup = "G:SY";

		// Token: 0x040000C5 RID: 197
		public const string WinRtDefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";

		// Token: 0x040000C6 RID: 198
		public const string WinRtDefaultSacl = "(ML;;NX;;;LW)";

		// Token: 0x040000C7 RID: 199
		public const string EtwProviderOwner = "O:SY";

		// Token: 0x040000C8 RID: 200
		public const string EtwProviderGroup = "G:SY";

		// Token: 0x040000C9 RID: 201
		public const string EtwProviderDefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";

		// Token: 0x040000CA RID: 202
		public const string WnfDefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";

		// Token: 0x040000CB RID: 203
		public const string SdRegValueOwner = "O:SY";

		// Token: 0x040000CC RID: 204
		public const string SdRegValueGroup = "G:SY";

		// Token: 0x040000CD RID: 205
		public const string SdRegValueDefaultDacl = "(A;;0x111FFFFF;;;SY)(A;;0x111FFFFF;;;BA)";

		// Token: 0x040000CE RID: 206
		public const string DriverDefaultDacl = "P(A;;GA;;;SY)";

		// Token: 0x040000CF RID: 207
		public const string TransientObjectRegistryPath = "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\SecurityManager\\TransientObjects\\";

		// Token: 0x040000D0 RID: 208
		public const string TransientObjectSecurityDescriptorValueName = "SecurityDescriptor";

		// Token: 0x040000D1 RID: 209
		public const string TransientObjectTypePrefix = "%5C%5C.%5C";

		// Token: 0x040000D2 RID: 210
		public const string TransientObjectTypeSuffix = "%5C";

		// Token: 0x040000D3 RID: 211
		public const string ComPermissionRegistryPath = "HKEY_LOCAL_MACHINE\\Software\\Classes\\AppId\\";

		// Token: 0x040000D4 RID: 212
		public const string ComAccessPermissionValueName = "AccessPermission";

		// Token: 0x040000D5 RID: 213
		public const string ComLaunchPermissionValueName = "LaunchPermission";

		// Token: 0x040000D6 RID: 214
		public const string WinRtPermissionRegistryPath = "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\WindowsRuntime\\Server\\";

		// Token: 0x040000D7 RID: 215
		public const string WinRtPermissionValueName = "Permissions";

		// Token: 0x040000D8 RID: 216
		public const string EtwProviderRegistryPath = "HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Control\\WMI\\Security";
	}
}
