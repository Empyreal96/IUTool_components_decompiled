using System;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200001B RID: 27
	[Guid("EE62470B-E94B-424e-9B7C-2F00C9249F93")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMetadataAssemblyImport
	{
		// Token: 0x0600010E RID: 270
		void GetAssemblyProps([In] Token assemblyToken, out EmbeddedBlobPointer pPublicKey, out int cbPublicKey, out int hashAlgId, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, [In] int cchName, out int pchName, [In] [Out] ref AssemblyMetaData pMetaData, out AssemblyNameFlags flags);

		// Token: 0x0600010F RID: 271
		void GetAssemblyRefProps([In] Token token, out EmbeddedBlobPointer pPublicKey, out int cbPublicKey, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, [In] int cchName, out int pchName, [In] [Out] ref AssemblyMetaData pMetaData, out UnusedIntPtr ppbHashValue, out uint pcbHashValue, out AssemblyNameFlags dwAssemblyRefFlags);

		// Token: 0x06000110 RID: 272
		void GetFileProps([In] int token, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, [In] int cchName, out int pchName, out UnusedIntPtr ppbHashValue, out uint pcbHashValue, out CorFileFlags dwFileFlags);

		// Token: 0x06000111 RID: 273
		void GetExportedTypeProps(int mdct, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, int cchName, out int pchName, out int ptkImplementation, out int ptkTypeDef, out CorTypeAttr pdwExportedTypeFlags);

		// Token: 0x06000112 RID: 274
		void GetManifestResourceProps([In] int mdmr, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, [In] int cchName, out int pchName, [ComAliasName("mdToken*")] out int ptkImplementation, [ComAliasName("DWORD*")] out uint pdwOffset, out CorManifestResourceFlags pdwResourceFlags);

		// Token: 0x06000113 RID: 275
		[PreserveSig]
		int EnumAssemblyRefs(ref HCORENUM phEnum, out Token assemblyRefs, int cMax, out int cTokens);

		// Token: 0x06000114 RID: 276
		void EnumFiles(ref HCORENUM phEnum, out int files, int cMax, out int cTokens);

		// Token: 0x06000115 RID: 277
		void EnumExportedTypes(ref HCORENUM phEnum, out int rExportedTypes, int cMax, out uint cTokens);

		// Token: 0x06000116 RID: 278
		void EnumManifestResources(ref HCORENUM phEnum, out int rManifestResources, int cMax, out int cTokens);

		// Token: 0x06000117 RID: 279
		[PreserveSig]
		int GetAssemblyFromScope(out int assemblyToken);

		// Token: 0x06000118 RID: 280
		void FindExportedTypeByName_();

		// Token: 0x06000119 RID: 281
		[PreserveSig]
		int FindManifestResourceByName([MarshalAs(UnmanagedType.LPWStr)] string szName, out int ptkManifestResource);

		// Token: 0x0600011A RID: 282
		[PreserveSig]
		int CloseEnum(IntPtr hEnum);
	}
}
