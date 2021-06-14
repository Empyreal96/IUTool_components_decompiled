using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000014 RID: 20
	internal static class NativeMethods
	{
		// Token: 0x060000AC RID: 172
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern FileType FileEntryBase_Get_FileType(IntPtr objPtr);

		// Token: 0x060000AD RID: 173
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string FileEntryBase_Get_DevicePath(IntPtr objPtr);

		// Token: 0x060000AE RID: 174
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string FileEntryBase_Get_CabPath(IntPtr objPtr);

		// Token: 0x060000AF RID: 175
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string FileEntryBase_Get_FileHash(IntPtr objPtr);

		// Token: 0x060000B0 RID: 176
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool FileEntryBase_Get_SignInfo(IntPtr objPtr);

		// Token: 0x060000B1 RID: 177
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern FileAttributes DSMFileEntry_Get_Attributes(IntPtr objPtr);

		// Token: 0x060000B2 RID: 178
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string DSMFileEntry_Get_SourcePackage(IntPtr objPtr);

		// Token: 0x060000B3 RID: 179
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string DSMFileEntry_Get_EmbeddedSigningCategory(IntPtr objPtr);

		// Token: 0x060000B4 RID: 180
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U8)]
		public static extern ulong DSMFileEntry_Get_FileSize(IntPtr objPtr);

		// Token: 0x060000B5 RID: 181
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U8)]
		public static extern ulong DSMFileEntry_Get_CompressedFileSize(IntPtr objPtr);

		// Token: 0x060000B6 RID: 182
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U8)]
		public static extern ulong DSMFileEntry_Get_StagedFileSize(IntPtr objPtr);

		// Token: 0x060000B7 RID: 183
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern DiffType DiffFileEntry_Get_DiffType(IntPtr objPtr);

		// Token: 0x060000B8 RID: 184
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string PackageDescriptor_Get_Keyform(IntPtr objPtr);

		// Token: 0x060000B9 RID: 185
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string PackageDescriptor_Get_Name(IntPtr objPtr);

		// Token: 0x060000BA RID: 186
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string PackageDescriptor_Get_Owner(IntPtr objPtr);

		// Token: 0x060000BB RID: 187
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_Owner(IntPtr objPtr, string owner);

		// Token: 0x060000BC RID: 188
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string PackageDescriptor_Get_Component(IntPtr objPtr);

		// Token: 0x060000BD RID: 189
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_Component(IntPtr objPtr, string component);

		// Token: 0x060000BE RID: 190
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string PackageDescriptor_Get_SubComponent(IntPtr objPtr);

		// Token: 0x060000BF RID: 191
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_SubComponent(IntPtr objPtr, string subComponent);

		// Token: 0x060000C0 RID: 192
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string PackageDescriptor_Get_BuildString(IntPtr objPtr);

		// Token: 0x060000C1 RID: 193
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_BuildString(IntPtr objPtr, string buildString);

		// Token: 0x060000C2 RID: 194
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern string PackageDescriptor_Get_PublicKey(IntPtr objPtr);

		// Token: 0x060000C3 RID: 195
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_PublicKey(IntPtr objPtr, string publicKeyString);

		// Token: 0x060000C4 RID: 196
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern OwnerType PackageDescriptor_Get_OwnerType(IntPtr objPtr);

		// Token: 0x060000C5 RID: 197
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_OwnerType(IntPtr objPtr, OwnerType ownerType);

		// Token: 0x060000C6 RID: 198
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern ReleaseType PackageDescriptor_Get_ReleaseType(IntPtr objPtr);

		// Token: 0x060000C7 RID: 199
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_ReleaseType(IntPtr objPtr, ReleaseType releaseType);

		// Token: 0x060000C8 RID: 200
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern BuildType PackageDescriptor_Get_BuildType(IntPtr objPtr);

		// Token: 0x060000C9 RID: 201
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_BuildType(IntPtr objPtr, BuildType buildType);

		// Token: 0x060000CA RID: 202
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern CpuId PackageDescriptor_Get_CpuType(IntPtr objPtr);

		// Token: 0x060000CB RID: 203
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_CpuType(IntPtr objPtr, CpuId cpuType);

		// Token: 0x060000CC RID: 204
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string PackageDescriptor_Get_Culture(IntPtr objPtr);

		// Token: 0x060000CD RID: 205
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_Culture(IntPtr objPtr, string culture);

		// Token: 0x060000CE RID: 206
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string PackageDescriptor_Get_Resolution(IntPtr objPtr);

		// Token: 0x060000CF RID: 207
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_Resolution(IntPtr objPtr, string Resolution);

		// Token: 0x060000D0 RID: 208
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string PackageDescriptor_Get_Description(IntPtr objPtr);

		// Token: 0x060000D1 RID: 209
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_Description(IntPtr objPtr, string description);

		// Token: 0x060000D2 RID: 210
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string PackageDescriptor_Get_GroupingKey(IntPtr objPtr);

		// Token: 0x060000D3 RID: 211
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_GroupingKey(IntPtr objPtr, string groupingKey);

		// Token: 0x060000D4 RID: 212
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool PackageDescriptor_Get_IsBinaryPartition(IntPtr objPtr);

		// Token: 0x060000D5 RID: 213
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool PackageDescriptor_Get_IsRemoval(IntPtr objPtr);

		// Token: 0x060000D6 RID: 214
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_IsRemoval(IntPtr objPtr, [MarshalAs(UnmanagedType.U1)] bool isRemoval);

		// Token: 0x060000D7 RID: 215
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string PackageDescriptor_Get_Partition(IntPtr objPtr);

		// Token: 0x060000D8 RID: 216
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_Partition(IntPtr objPtr, string partition);

		// Token: 0x060000D9 RID: 217
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string PackageDescriptor_Get_Platform(IntPtr objPtr);

		// Token: 0x060000DA RID: 218
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_Platform(IntPtr objPtr, string platform);

		// Token: 0x060000DB RID: 219
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern IntPtr PackageDescriptor_Get_TargetGroups(IntPtr objPtr, ref int cGroups);

		// Token: 0x060000DC RID: 220
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_TargetGroups(IntPtr objPtr, string[] targetGroups, int cGroups);

		// Token: 0x060000DD RID: 221
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void PackageDescriptor_Get_Version(IntPtr objPtr, [In] [Out] ref VersionInfo version);

		// Token: 0x060000DE RID: 222
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int PackageDescriptor_Set_Version(IntPtr objPtr, [In] ref VersionInfo version);

		// Token: 0x060000DF RID: 223
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void DeviceSideManifest_Clear_Files(IntPtr objPtr);

		// Token: 0x060000E0 RID: 224
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DeviceSideManifest_Add_File(IntPtr objPtr, FileType fileType, string devicePath, string cabPath, FileAttributes attributes, string sourcePackage, string embedSignCategory, ulong FileSize, ulong CompressedFileSize, ulong StagedFileSize, string fileHash, [MarshalAs(UnmanagedType.U1)] bool signFile);

		// Token: 0x060000E1 RID: 225
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DeviceSideManifest_Get_Files(IntPtr objPtr, ref IntPtr filesObjPtr);

		// Token: 0x060000E2 RID: 226
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DeviceSideManifest_Load(IntPtr objPtr, string xmlPath);

		// Token: 0x060000E3 RID: 227
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DeviceSideManifest_Load_CBS(IntPtr objPtr, string cabPath);

		// Token: 0x060000E4 RID: 228
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DeviceSideManifest_Save(IntPtr objPtr, string xmlPath);

		// Token: 0x060000E5 RID: 229
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern IntPtr DeviceSideManifest_Create();

		// Token: 0x060000E6 RID: 230
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern IntPtr DeviceSideManifest_Free(IntPtr objPtr);

		// Token: 0x060000E7 RID: 231
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string DiffManifest_Get_Name(IntPtr objPtr);

		// Token: 0x060000E8 RID: 232
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DiffManifest_Set_Name(IntPtr objPtr, string name);

		// Token: 0x060000E9 RID: 233
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern IntPtr DiffManifest_Get_SourceHash(IntPtr objPtr, out int cbHash);

		// Token: 0x060000EA RID: 234
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DiffManifest_Set_SourceHash(IntPtr objPtr, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] hash, int cbHash);

		// Token: 0x060000EB RID: 235
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void DiffManifest_Get_TargetVersion(IntPtr objPtr, [In] [Out] ref VersionInfo version);

		// Token: 0x060000EC RID: 236
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DiffManifest_Set_TargetVersion(IntPtr objPtr, [In] ref VersionInfo version);

		// Token: 0x060000ED RID: 237
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void DiffManifest_Get_SourceVersion(IntPtr objPtr, [In] [Out] ref VersionInfo version);

		// Token: 0x060000EE RID: 238
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DiffManifest_Set_SourceVersion(IntPtr objPtr, [In] ref VersionInfo version);

		// Token: 0x060000EF RID: 239
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DiffManifest_Add_File(IntPtr objPtr, FileType fileType, DiffType diffType, string devicePath, string cabPath);

		// Token: 0x060000F0 RID: 240
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DiffManifest_Get_Files(IntPtr objPtr, ref IntPtr filesObjPtr);

		// Token: 0x060000F1 RID: 241
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DiffManifest_Load(IntPtr objPtr, string xmlPath);

		// Token: 0x060000F2 RID: 242
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DiffManifest_Load_XPD(IntPtr dmsPtr, IntPtr diffPtr, string xmlPath);

		// Token: 0x060000F3 RID: 243
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int DiffManifest_Save(IntPtr objPtr, string xmlPath);

		// Token: 0x060000F4 RID: 244
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern IntPtr DiffManifest_Create();

		// Token: 0x060000F5 RID: 245
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void DiffManifest_Clear_Files(IntPtr objPtr);

		// Token: 0x060000F6 RID: 246
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern IntPtr DiffManifest_Free(IntPtr objPtr);

		// Token: 0x060000F7 RID: 247
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void Helper_Free_Array(IntPtr arrayPtr);

		// Token: 0x060000F8 RID: 248 RVA: 0x00006769 File Offset: 0x00004969
		public static void CheckHResult(int hr, string function)
		{
			if (hr != 0)
			{
				throw new PackageException("Unexpected hr value ({0:X8}) from function '{1}'", new object[]
				{
					hr,
					function
				});
			}
		}

		// Token: 0x060000F9 RID: 249
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern uint IU_GetStagedAndCompressedSize(string file, out ulong fileSize, out ulong stagedSize, out ulong compressedSize);

		// Token: 0x0400001C RID: 28
		private const string PKGCOMMON_DLL = "UpdateDLL.dll";
	}
}
