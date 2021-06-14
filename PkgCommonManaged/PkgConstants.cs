using System;
using System.IO;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x0200002D RID: 45
	public static class PkgConstants
	{
		// Token: 0x0400008C RID: 140
		public static readonly string c_strCBSPackageExtension = ".cab";

		// Token: 0x0400008D RID: 141
		public static readonly string c_strPackageExtension = ".spkg";

		// Token: 0x0400008E RID: 142
		public static readonly string c_strPackageSearchPattern = "*" + PkgConstants.c_strPackageExtension;

		// Token: 0x0400008F RID: 143
		public static readonly string c_strRemovalPkgExtension = ".spkr";

		// Token: 0x04000090 RID: 144
		public static readonly string c_strRemovalPkgSearchPattern = "*" + PkgConstants.c_strRemovalPkgExtension;

		// Token: 0x04000091 RID: 145
		public static readonly string c_strRemovalCbsExtension = ".cbsr";

		// Token: 0x04000092 RID: 146
		public static readonly string c_strRemovalCbsSearchPattern = "*" + PkgConstants.c_strRemovalCbsExtension;

		// Token: 0x04000093 RID: 147
		public static readonly string c_strDiffPackageExtension = ".spku";

		// Token: 0x04000094 RID: 148
		public static readonly string c_strDiffPackageSearchPattern = "*" + PkgConstants.c_strPackageExtension;

		// Token: 0x04000095 RID: 149
		public static readonly string c_strDsmExtension = ".dsm.xml";

		// Token: 0x04000096 RID: 150
		public static readonly string c_strMumExtension = ".mum";

		// Token: 0x04000097 RID: 151
		public static readonly string c_strDsmFile = "man" + PkgConstants.c_strDsmExtension;

		// Token: 0x04000098 RID: 152
		public static readonly string c_strMumFile = "update" + PkgConstants.c_strMumExtension;

		// Token: 0x04000099 RID: 153
		public static readonly string c_strCatalogFileExtension = ".cat";

		// Token: 0x0400009A RID: 154
		public static readonly string c_strCatalogFile = "content" + PkgConstants.c_strCatalogFileExtension;

		// Token: 0x0400009B RID: 155
		public static readonly string c_strCBSCatalogFile = "update" + PkgConstants.c_strCatalogFileExtension;

		// Token: 0x0400009C RID: 156
		public static readonly string c_strDsmSearchPattern = "*" + PkgConstants.c_strDsmExtension;

		// Token: 0x0400009D RID: 157
		public static readonly string c_strMumSearchPattern = "*" + PkgConstants.c_strMumExtension;

		// Token: 0x0400009E RID: 158
		public static readonly string c_strDiffDsmExtension = ".ddsm.xml";

		// Token: 0x0400009F RID: 159
		public static readonly string c_strDiffDsmFile = "dman" + PkgConstants.c_strDiffDsmExtension;

		// Token: 0x040000A0 RID: 160
		public static readonly string c_strDiffDsmSearchPattern = "*" + PkgConstants.c_strDiffDsmExtension;

		// Token: 0x040000A1 RID: 161
		public static readonly string c_strRguExtension = ".reg";

		// Token: 0x040000A2 RID: 162
		public static readonly string c_strRegAppendExtension = ".rga";

		// Token: 0x040000A3 RID: 163
		public static readonly string c_strPolicyExtension = ".policy.xml";

		// Token: 0x040000A4 RID: 164
		public static readonly string c_strCustomMetadataExtension = ".meta.xml";

		// Token: 0x040000A5 RID: 165
		public static readonly string c_strCIX = "_manifest_.cix.xml";

		// Token: 0x040000A6 RID: 166
		public static readonly string c_strCertStoreExtension = ".dat";

		// Token: 0x040000A7 RID: 167
		public static readonly string c_strPkgMetadataFolder = "\\Windows\\Packages";

		// Token: 0x040000A8 RID: 168
		public static readonly string c_strDsmDeviceFolder = PkgConstants.c_strPkgMetadataFolder + "\\DsmFiles";

		// Token: 0x040000A9 RID: 169
		public static readonly string c_strMumDeviceFolder = "Windows\\servicing\\Packages";

		// Token: 0x040000AA RID: 170
		public static readonly string c_strRguDeviceFolder = PkgConstants.c_strPkgMetadataFolder + "\\RegistryFiles";

		// Token: 0x040000AB RID: 171
		public static readonly string c_strRgaDeviceFolder = PkgConstants.c_strRguDeviceFolder;

		// Token: 0x040000AC RID: 172
		public static readonly string c_strPolicyDeviceFolder = PkgConstants.c_strPkgMetadataFolder + "\\PolicyFiles";

		// Token: 0x040000AD RID: 173
		public static readonly string c_strCustomMetadataDeviceFolder = PkgConstants.c_strPkgMetadataFolder + "\\CustomMetadata";

		// Token: 0x040000AE RID: 174
		private static readonly string c_strBackupMetadataRootDeviceFolder = PkgConstants.c_strPkgMetadataFolder + "\\BackupMetadata";

		// Token: 0x040000AF RID: 175
		public static readonly string c_strBackupMetadataDirectoriesDeviceFolder = PkgConstants.c_strBackupMetadataRootDeviceFolder + "\\Directories";

		// Token: 0x040000B0 RID: 176
		public static readonly string c_strBackupMetadataFilesDeviceFolder = PkgConstants.c_strBackupMetadataRootDeviceFolder + "\\Files";

		// Token: 0x040000B1 RID: 177
		public static string c_strCertStoreDeviceFolder = PkgConstants.c_strPkgMetadataFolder + "\\Certificates";

		// Token: 0x040000B2 RID: 178
		public static readonly string c_strCatalogDeviceFolder = "\\Windows\\System32\\catroot\\{F750E6C3-38EE-11D1-85E5-00C04FC295EE}";

		// Token: 0x040000B3 RID: 179
		public static readonly string c_strCBSPublicKey = "628844477771337a";

		// Token: 0x040000B4 RID: 180
		public static readonly string[] c_strSpecialFolders = new string[]
		{
			PkgConstants.c_strDsmDeviceFolder,
			PkgConstants.c_strRguDeviceFolder,
			PkgConstants.c_strRgaDeviceFolder,
			PkgConstants.c_strPolicyDeviceFolder,
			PkgConstants.c_strCertStoreDeviceFolder
		};

		// Token: 0x040000B5 RID: 181
		public static readonly string c_strMainOsPartition = "MainOS";

		// Token: 0x040000B6 RID: 182
		public static readonly string c_strUpdateOsPartition = "UpdateOS";

		// Token: 0x040000B7 RID: 183
		public static readonly string c_strEfiPartition = "EFIESP";

		// Token: 0x040000B8 RID: 184
		public static readonly string c_strDataPartition = "Data";

		// Token: 0x040000B9 RID: 185
		public static readonly string c_strPlatPartition = "PLAT";

		// Token: 0x040000BA RID: 186
		public static readonly string c_strCrashDumpPartition = "CrashDump";

		// Token: 0x040000BB RID: 187
		public static readonly string c_strDPPPartition = "DPP";

		// Token: 0x040000BC RID: 188
		public static readonly string c_strDataPartitionRoot = Path.DirectorySeparatorChar.ToString() + PkgConstants.c_strDataPartition + Path.DirectorySeparatorChar.ToString();

		// Token: 0x040000BD RID: 189
		public static readonly string[] c_strHivePartitions = new string[]
		{
			PkgConstants.c_strMainOsPartition,
			PkgConstants.c_strUpdateOsPartition,
			PkgConstants.c_strEfiPartition
		};

		// Token: 0x040000BE RID: 190
		public static readonly string[] c_strJunctionPaths = new string[]
		{
			PkgConstants.c_strDataPartitionRoot,
			Path.DirectorySeparatorChar.ToString() + PkgConstants.c_strEfiPartition + Path.DirectorySeparatorChar.ToString(),
			Path.DirectorySeparatorChar.ToString() + PkgConstants.c_strCrashDumpPartition + Path.DirectorySeparatorChar.ToString(),
			Path.DirectorySeparatorChar.ToString() + PkgConstants.c_strDPPPartition + Path.DirectorySeparatorChar.ToString()
		};

		// Token: 0x040000BF RID: 191
		public static readonly string c_strDefaultPartition = PkgConstants.c_strMainOsPartition;

		// Token: 0x040000C0 RID: 192
		public static readonly string c_strDefaultDrive = "C:";

		// Token: 0x040000C1 RID: 193
		public static readonly string c_strUpdateOSDrive = "X:";

		// Token: 0x040000C2 RID: 194
		public static readonly int c_iMaxPackageString = 64;

		// Token: 0x040000C3 RID: 195
		public static readonly int c_iMaxElementCount = 8000;

		// Token: 0x040000C4 RID: 196
		public static readonly int c_iMaxPackageName = PkgConstants.c_iMaxPackageString * 3 + 2;

		// Token: 0x040000C5 RID: 197
		public static readonly string c_strPackageStringPattern = "^[0-9a-zA-Z_\\-.]+$";

		// Token: 0x040000C6 RID: 198
		public static readonly string c_strCultureStringPattern = "^[a-zA-Z][a-zA-Z0-9_\\-]+$";

		// Token: 0x040000C7 RID: 199
		public static readonly string c_strResolutionStringPattern = "^[1-9][0-9]+x|X[1-9][0-9]+$";

		// Token: 0x040000C8 RID: 200
		public static readonly int c_iMaxBuildString = 1024;

		// Token: 0x040000C9 RID: 201
		public static readonly int c_iMaxDevicePath = 32000;

		// Token: 0x040000CA RID: 202
		public static readonly string c_strHashAlgorithm = "SHA256";

		// Token: 0x040000CB RID: 203
		public static readonly int c_iHashSize = 32;

		// Token: 0x040000CC RID: 204
		public static readonly FileAttributes c_validAttributes = FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System | FileAttributes.Archive | FileAttributes.Normal | FileAttributes.Compressed;

		// Token: 0x040000CD RID: 205
		public static readonly FileAttributes c_defaultAttributes = FileAttributes.Archive | FileAttributes.Compressed;

		// Token: 0x040000CE RID: 206
		public static int c_iMaxGroupIdString = 39;

		// Token: 0x040000CF RID: 207
		public static int c_iMaxPackagingThreads = Environment.ProcessorCount * 2;

		// Token: 0x040000D0 RID: 208
		public static string c_strGroupIdPattern = "^[a-zA-Z0-9_.\\\\/\\-{}]+$";
	}
}
