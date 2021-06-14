using System;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000018 RID: 24
	[Guid("FCE5EFA0-8BBA-4f8e-A036-8F2022B08466")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMetadataImport2 : IMetadataImport
	{
		// Token: 0x060000BF RID: 191
		[PreserveSig]
		void CloseEnum(IntPtr hEnum);

		// Token: 0x060000C0 RID: 192
		void CountEnum(HCORENUM hEnum, [ComAliasName("ULONG*")] out int pulCount);

		// Token: 0x060000C1 RID: 193
		void ResetEnum(HCORENUM hEnum, int ulPos);

		// Token: 0x060000C2 RID: 194
		void EnumTypeDefs(ref HCORENUM phEnum, [ComAliasName("mdTypeDef*")] out int rTypeDefs, uint cMax, [ComAliasName("ULONG*")] out uint pcTypeDefs);

		// Token: 0x060000C3 RID: 195
		void EnumInterfaceImpls(ref HCORENUM phEnum, int td, out int rImpls, int cMax, ref int pcImpls);

		// Token: 0x060000C4 RID: 196
		void EnumTypeRefs_();

		// Token: 0x060000C5 RID: 197
		void FindTypeDefByName([MarshalAs(UnmanagedType.LPWStr)] [In] string szTypeDef, [In] int tkEnclosingClass, [ComAliasName("mdTypeDef*")] out int token);

		// Token: 0x060000C6 RID: 198
		void GetScopeProps([MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, [In] int cchName, [ComAliasName("ULONG*")] out int pchName, out Guid mvid);

		// Token: 0x060000C7 RID: 199
		void GetModuleFromScope(out int mdModule);

		// Token: 0x060000C8 RID: 200
		void GetTypeDefProps([In] int td, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szTypeDef, [In] int cchTypeDef, [ComAliasName("ULONG*")] out int pchTypeDef, [MarshalAs(UnmanagedType.U4)] out TypeAttributes pdwTypeDefFlags, [ComAliasName("mdToken*")] out int ptkExtends);

		// Token: 0x060000C9 RID: 201
		void GetInterfaceImplProps(int iiImpl, out int pClass, out int ptkIface);

		// Token: 0x060000CA RID: 202
		void GetTypeRefProps(int tr, [ComAliasName("mdToken*")] out int ptkResolutionScope, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, [In] int cchName, [ComAliasName("ULONG*")] out int pchName);

		// Token: 0x060000CB RID: 203
		void ResolveTypeRef_();

		// Token: 0x060000CC RID: 204
		void EnumMembers_();

		// Token: 0x060000CD RID: 205
		void EnumMembersWithName_();

		// Token: 0x060000CE RID: 206
		void EnumMethods(ref HCORENUM phEnum, int cl, [ComAliasName("mdMethodDef*")] out int mdMethodDef, int cMax, [ComAliasName("ULONG*")] out int pcTokens);

		// Token: 0x060000CF RID: 207
		void EnumMethodsWithName(ref HCORENUM phEnum, int cl, [MarshalAs(UnmanagedType.LPWStr)] [In] string szName, [ComAliasName("mdMethodDef*")] out int mdMethodDef, int cMax, [ComAliasName("ULONG*")] out int pcTokens);

		// Token: 0x060000D0 RID: 208
		void EnumFields(ref HCORENUM phEnum, int cl, [ComAliasName("mdFieldDef*")] out int mdFieldDef, int cMax, [ComAliasName("ULONG*")] out uint pcTokens);

		// Token: 0x060000D1 RID: 209
		void EnumFieldsWithName_();

		// Token: 0x060000D2 RID: 210
		[PreserveSig]
		int EnumParams(ref HCORENUM phEnum, int mdMethodDef, [MarshalAs(UnmanagedType.LPArray)] int[] rParams, int cMax, [ComAliasName("ULONG*")] out uint pcTokens);

		// Token: 0x060000D3 RID: 211
		void EnumMemberRefs_();

		// Token: 0x060000D4 RID: 212
		void EnumMethodImpls(ref HCORENUM hEnum, Token typeDef, out Token methodBody, out Token methodDecl, int cMax, out int cTokens);

		// Token: 0x060000D5 RID: 213
		void EnumPermissionSets_();

		// Token: 0x060000D6 RID: 214
		void FindMember([In] int typeDefToken, [MarshalAs(UnmanagedType.LPWStr)] [In] string szName, [In] byte[] pvSigBlob, [In] int cbSigBlob, out int memberDefToken);

		// Token: 0x060000D7 RID: 215
		void FindMethod([In] int typeDef, [MarshalAs(UnmanagedType.LPWStr)] [In] string szName, [In] EmbeddedBlobPointer pvSigBlob, [In] int cbSigBlob, out int methodDef);

		// Token: 0x060000D8 RID: 216
		void FindField([In] int typeDef, [MarshalAs(UnmanagedType.LPWStr)] [In] string szName, [In] byte[] pvSigBlob, [In] int cbSigBlob, out int fieldDef);

		// Token: 0x060000D9 RID: 217
		void FindMemberRef([In] int typeRef, [MarshalAs(UnmanagedType.LPWStr)] [In] string szName, [In] byte[] pvSigBlob, [In] int cbSigBlob, out int result);

		// Token: 0x060000DA RID: 218
		void GetMethodProps([In] uint md, [ComAliasName("mdTypeDef*")] out int pClass, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szMethod, [In] int cchMethod, [ComAliasName("ULONG*")] out uint pchMethod, [ComAliasName("DWORD*")] out MethodAttributes pdwAttr, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSigBlob, [ComAliasName("ULONG*")] out uint pcbSigBlob, [ComAliasName("ULONG*")] out uint pulCodeRVA, [ComAliasName("DWORD*")] out uint pdwImplFlags);

		// Token: 0x060000DB RID: 219
		void GetMemberRefProps([In] Token mr, [ComAliasName("mdMemberRef*")] out Token ptk, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szMember, [In] int cchMember, [ComAliasName("ULONG*")] out uint pchMember, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSigBlob, [ComAliasName("ULONG*")] out uint pbSig);

		// Token: 0x060000DC RID: 220
		void EnumProperties(ref HCORENUM phEnum, int td, [ComAliasName("mdProperty*")] out int mdFieldDef, int cMax, [ComAliasName("ULONG*")] out uint pcTokens);

		// Token: 0x060000DD RID: 221
		void EnumEvents(ref HCORENUM phEnum, int td, [ComAliasName("mdEvent*")] out int mdFieldDef, int cMax, [ComAliasName("ULONG*")] out uint pcEvents);

		// Token: 0x060000DE RID: 222
		void GetEventProps(int ev, [ComAliasName("mdTypeDef*")] out int pClass, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szEvent, int cchEvent, [ComAliasName("ULONG*")] out int pchEvent, [ComAliasName("DWORD*")] out int pdwEventFlags, [ComAliasName("mdToken*")] out int ptkEventType, [ComAliasName("mdMethodDef*")] out int pmdAddOn, [ComAliasName("mdMethodDef*")] out int pmdRemoveOn, [ComAliasName("mdMethodDef*")] out int pmdFire, [ComAliasName("mdMethodDef*")] out int rmdOtherMethod, uint cMax, [ComAliasName("ULONG*")] out uint pcOtherMethod);

		// Token: 0x060000DF RID: 223
		void EnumMethodSemantics_();

		// Token: 0x060000E0 RID: 224
		void GetMethodSemantics_();

		// Token: 0x060000E1 RID: 225
		[PreserveSig]
		uint GetClassLayout(int typeDef, out uint dwPackSize, UnusedIntPtr zeroPtr, uint zeroCount, UnusedIntPtr zeroPtr2, ref uint ulClassSize);

		// Token: 0x060000E2 RID: 226
		void GetFieldMarshal_();

		// Token: 0x060000E3 RID: 227
		void GetRVA(int token, out uint rva, out uint flags);

		// Token: 0x060000E4 RID: 228
		void GetPermissionSetProps_();

		// Token: 0x060000E5 RID: 229
		void GetSigFromToken(int token, out EmbeddedBlobPointer pSig, out int cbSig);

		// Token: 0x060000E6 RID: 230
		void GetModuleRefProps(int mur, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, int cchName, [ComAliasName("ULONG*")] out int pchName);

		// Token: 0x060000E7 RID: 231
		void EnumModuleRefs(ref HCORENUM phEnum, [ComAliasName("mdModuleRef*")] out int mdModuleRef, int cMax, [ComAliasName("ULONG*")] out uint pcModuleRefs);

		// Token: 0x060000E8 RID: 232
		[PreserveSig]
		int GetTypeSpecFromToken(Token typeSpec, out EmbeddedBlobPointer pSig, out int cbSig);

		// Token: 0x060000E9 RID: 233
		void GetNameFromToken_();

		// Token: 0x060000EA RID: 234
		void EnumUnresolvedMethods_();

		// Token: 0x060000EB RID: 235
		void GetUserString([In] int stk, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] [Out] char[] szString, [In] int cchString, [ComAliasName("ULONG*")] out int pchString);

		// Token: 0x060000EC RID: 236
		void GetPinvokeMap_();

		// Token: 0x060000ED RID: 237
		void EnumSignatures_();

		// Token: 0x060000EE RID: 238
		void EnumTypeSpecs_();

		// Token: 0x060000EF RID: 239
		void EnumUserStrings_();

		// Token: 0x060000F0 RID: 240
		void GetParamForMethodIndex_();

		// Token: 0x060000F1 RID: 241
		void EnumCustomAttributes(ref HCORENUM phEnum, int tk, int tkType, [ComAliasName("mdCustomAttribute*")] out Token mdCustomAttribute, uint cMax, [ComAliasName("ULONG*")] out uint pcTokens);

		// Token: 0x060000F2 RID: 242
		void GetCustomAttributeProps([In] Token cv, out Token tkObj, out Token tkType, out EmbeddedBlobPointer blob, out int cbSize);

		// Token: 0x060000F3 RID: 243
		void FindTypeRef([In] int tkResolutionScope, [MarshalAs(UnmanagedType.LPWStr)] [In] string szName, out int typeRef);

		// Token: 0x060000F4 RID: 244
		void GetMemberProps_();

		// Token: 0x060000F5 RID: 245
		void GetFieldProps(int mb, [ComAliasName("mdTypeDef*")] out int mdTypeDef, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szField, int cchField, [ComAliasName("ULONG*")] out int pchField, [ComAliasName("DWORD*")] out FieldAttributes pdwAttr, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSigBlob, [ComAliasName("ULONG*")] out int pcbSigBlob, [ComAliasName("DWORD*")] out int pdwCPlusTypeFlab, [ComAliasName("UVCP_CONSTANT*")] out IntPtr ppValue, [ComAliasName("ULONG*")] out int pcchValue);

		// Token: 0x060000F6 RID: 246
		void GetPropertyProps(Token prop, [ComAliasName("mdTypeDef*")] out Token pClass, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szProperty, int cchProperty, [ComAliasName("ULONG*")] out int pchProperty, [ComAliasName("DWORD*")] out PropertyAttributes pdwPropFlags, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSig, [ComAliasName("ULONG*")] out int pbSig, [ComAliasName("DWORD*")] out int pdwCPlusTypeFlag, [ComAliasName("UVCP_CONSTANT*")] out UnusedIntPtr ppDefaultValue, [ComAliasName("ULONG*")] out int pcchDefaultValue, [ComAliasName("mdMethodDef*")] out Token pmdSetter, [ComAliasName("mdMethodDef*")] out Token pmdGetter, [ComAliasName("mdMethodDef*")] out Token rmdOtherMethod, uint cMax, [ComAliasName("ULONG*")] out uint pcOtherMethod);

		// Token: 0x060000F7 RID: 247
		void GetParamProps(int tk, [ComAliasName("mdMethodDef*")] out int pmd, [ComAliasName("ULONG*")] out uint pulSequence, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, uint cchName, [ComAliasName("ULONG*")] out uint pchName, [ComAliasName("DWORD*")] out uint pdwAttr, [ComAliasName("DWORD*")] out uint pdwCPlusTypeFlag, [ComAliasName("UVCP_CONSTANT*")] out UnusedIntPtr ppValue, [ComAliasName("ULONG*")] out uint pcchValue);

		// Token: 0x060000F8 RID: 248
		[PreserveSig]
		int GetCustomAttributeByName(int tkObj, [MarshalAs(UnmanagedType.LPWStr)] string szName, out EmbeddedBlobPointer ppData, out uint pcbData);

		// Token: 0x060000F9 RID: 249
		[PreserveSig]
		bool IsValidToken([MarshalAs(UnmanagedType.U4)] [In] uint tk);

		// Token: 0x060000FA RID: 250
		void GetNestedClassProps(int tdNestedClass, [ComAliasName("mdTypeDef*")] out int tdEnclosingClass);

		// Token: 0x060000FB RID: 251
		void GetNativeCallConvFromSig_();

		// Token: 0x060000FC RID: 252
		void IsGlobal_();

		// Token: 0x060000FD RID: 253
		void EnumGenericParams(ref HCORENUM hEnum, int tk, [ComAliasName("mdGenericParam*")] out int rGenericParams, uint cMax, [ComAliasName("ULONG*")] out uint pcGenericParams);

		// Token: 0x060000FE RID: 254
		void GetGenericParamProps(int gp, [ComAliasName("ULONG*")] out uint pulParamSeq, [ComAliasName("DWORD*")] out int pdwParamFlags, [ComAliasName("mdToken*")] out int ptOwner, [ComAliasName("mdToken*")] out int ptkKind, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder wzName, [ComAliasName("ULONG*")] uint cchName, [ComAliasName("ULONG*")] out uint pchName);

		// Token: 0x060000FF RID: 255
		void GetMethodSpecProps([ComAliasName("mdMethodSpec")] Token mi, [ComAliasName("mdToken*")] out Token tkParent, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSigBlob, [ComAliasName("ULONG*")] out int pcbSigBlob);

		// Token: 0x06000100 RID: 256
		void EnumGenericParamConstraints(ref HCORENUM hEnum, int tk, [ComAliasName("mdGenericParamConstraint*")] out int rGenericParamConstraints, uint cMax, [ComAliasName("ULONG*")] out uint pcGenericParams);

		// Token: 0x06000101 RID: 257
		void GetGenericParamConstraintProps(int gpc, [ComAliasName("mdGenericParam*")] out int ptGenericParam, [ComAliasName("mdToken*")] out int ptkConstraintType);

		// Token: 0x06000102 RID: 258
		void GetPEKind(out PortableExecutableKinds dwPEKind, out ImageFileMachine pdwMachine);

		// Token: 0x06000103 RID: 259
		void GetVersionString([MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, [In] int cchName, out int pchName);

		// Token: 0x06000104 RID: 260
		void EnumMethodSpecs_();
	}
}
