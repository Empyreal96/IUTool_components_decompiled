using System;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000017 RID: 23
	[Guid("7DAC8207-D3AE-4c75-9B67-92801A497D44")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMetadataImport
	{
		// Token: 0x06000081 RID: 129
		[PreserveSig]
		void CloseEnum(IntPtr hEnum);

		// Token: 0x06000082 RID: 130
		void CountEnum(HCORENUM hEnum, [ComAliasName("ULONG*")] out int pulCount);

		// Token: 0x06000083 RID: 131
		void ResetEnum(HCORENUM hEnum, int ulPos);

		// Token: 0x06000084 RID: 132
		void EnumTypeDefs(ref HCORENUM phEnum, [ComAliasName("mdTypeDef*")] out int rTypeDefs, uint cMax, [ComAliasName("ULONG*")] out uint pcTypeDefs);

		// Token: 0x06000085 RID: 133
		void EnumInterfaceImpls(ref HCORENUM phEnum, int td, out int rImpls, int cMax, ref int pcImpls);

		// Token: 0x06000086 RID: 134
		void EnumTypeRefs_();

		// Token: 0x06000087 RID: 135
		[PreserveSig]
		int FindTypeDefByName([MarshalAs(UnmanagedType.LPWStr)] [In] string szTypeDef, [In] int tkEnclosingClass, [ComAliasName("mdTypeDef*")] out int token);

		// Token: 0x06000088 RID: 136
		void GetScopeProps([MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, [In] int cchName, [ComAliasName("ULONG*")] out int pchName, out Guid mvid);

		// Token: 0x06000089 RID: 137
		void GetModuleFromScope(out int mdModule);

		// Token: 0x0600008A RID: 138
		void GetTypeDefProps([In] int td, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szTypeDef, [In] int cchTypeDef, [ComAliasName("ULONG*")] out int pchTypeDef, [MarshalAs(UnmanagedType.U4)] out TypeAttributes pdwTypeDefFlags, [ComAliasName("mdToken*")] out int ptkExtends);

		// Token: 0x0600008B RID: 139
		void GetInterfaceImplProps(int iiImpl, out int pClass, out int ptkIface);

		// Token: 0x0600008C RID: 140
		void GetTypeRefProps(int tr, [ComAliasName("mdToken*")] out int ptkResolutionScope, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, [In] int cchName, [ComAliasName("ULONG*")] out int pchName);

		// Token: 0x0600008D RID: 141
		void ResolveTypeRef_();

		// Token: 0x0600008E RID: 142
		void EnumMembers_();

		// Token: 0x0600008F RID: 143
		void EnumMembersWithName_();

		// Token: 0x06000090 RID: 144
		void EnumMethods(ref HCORENUM phEnum, int cl, [ComAliasName("mdMethodDef*")] out int mdMethodDef, int cMax, [ComAliasName("ULONG*")] out int pcTokens);

		// Token: 0x06000091 RID: 145
		void EnumMethodsWithName(ref HCORENUM phEnum, int cl, [MarshalAs(UnmanagedType.LPWStr)] [In] string szName, [ComAliasName("mdMethodDef*")] out int mdMethodDef, int cMax, [ComAliasName("ULONG*")] out int pcTokens);

		// Token: 0x06000092 RID: 146
		void EnumFields(ref HCORENUM phEnum, int cl, [ComAliasName("mdFieldDef*")] out int mdFieldDef, int cMax, [ComAliasName("ULONG*")] out uint pcTokens);

		// Token: 0x06000093 RID: 147
		void EnumFieldsWithName_();

		// Token: 0x06000094 RID: 148
		[PreserveSig]
		int EnumParams(ref HCORENUM phEnum, int mdMethodDef, [MarshalAs(UnmanagedType.LPArray)] int[] rParams, int cMax, [ComAliasName("ULONG*")] out uint pcTokens);

		// Token: 0x06000095 RID: 149
		void EnumMemberRefs_();

		// Token: 0x06000096 RID: 150
		void EnumMethodImpls(ref HCORENUM hEnum, Token typeDef, out Token methodBody, out Token methodDecl, int cMax, out int cTokens);

		// Token: 0x06000097 RID: 151
		void EnumPermissionSets_();

		// Token: 0x06000098 RID: 152
		void FindMember([In] int typeDefToken, [MarshalAs(UnmanagedType.LPWStr)] [In] string szName, [In] byte[] pvSigBlob, [In] int cbSigBlob, out int memberDefToken);

		// Token: 0x06000099 RID: 153
		void FindMethod([In] int typeDef, [MarshalAs(UnmanagedType.LPWStr)] [In] string szName, [In] EmbeddedBlobPointer pvSigBlob, [In] int cbSigBlob, out int methodDef);

		// Token: 0x0600009A RID: 154
		void FindField([In] int typeDef, [MarshalAs(UnmanagedType.LPWStr)] [In] string szName, [In] byte[] pvSigBlob, [In] int cbSigBlob, out int fieldDef);

		// Token: 0x0600009B RID: 155
		void FindMemberRef([In] int typeRef, [MarshalAs(UnmanagedType.LPWStr)] [In] string szName, [In] byte[] pvSigBlob, [In] int cbSigBlob, out int result);

		// Token: 0x0600009C RID: 156
		void GetMethodProps([In] uint md, [ComAliasName("mdTypeDef*")] out int pClass, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szMethod, [In] int cchMethod, [ComAliasName("ULONG*")] out uint pchMethod, [ComAliasName("DWORD*")] out MethodAttributes pdwAttr, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSigBlob, [ComAliasName("ULONG*")] out uint pcbSigBlob, [ComAliasName("ULONG*")] out uint pulCodeRVA, [ComAliasName("DWORD*")] out uint pdwImplFlags);

		// Token: 0x0600009D RID: 157
		void GetMemberRefProps([In] Token mr, [ComAliasName("mdMemberRef*")] out Token ptk, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szMember, [In] int cchMember, [ComAliasName("ULONG*")] out uint pchMember, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSigBlob, [ComAliasName("ULONG*")] out uint pbSig);

		// Token: 0x0600009E RID: 158
		void EnumProperties(ref HCORENUM phEnum, int td, [ComAliasName("mdProperty*")] out int mdFieldDef, int cMax, [ComAliasName("ULONG*")] out uint pcTokens);

		// Token: 0x0600009F RID: 159
		void EnumEvents(ref HCORENUM phEnum, int td, [ComAliasName("mdEvent*")] out int mdFieldDef, int cMax, [ComAliasName("ULONG*")] out uint pcEvents);

		// Token: 0x060000A0 RID: 160
		void GetEventProps(int ev, [ComAliasName("mdTypeDef*")] out int pClass, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szEvent, int cchEvent, [ComAliasName("ULONG*")] out int pchEvent, [ComAliasName("DWORD*")] out int pdwEventFlags, [ComAliasName("mdToken*")] out int ptkEventType, [ComAliasName("mdMethodDef*")] out int pmdAddOn, [ComAliasName("mdMethodDef*")] out int pmdRemoveOn, [ComAliasName("mdMethodDef*")] out int pmdFire, [ComAliasName("mdMethodDef*")] out int rmdOtherMethod, uint cMax, [ComAliasName("ULONG*")] out uint pcOtherMethod);

		// Token: 0x060000A1 RID: 161
		void EnumMethodSemantics_();

		// Token: 0x060000A2 RID: 162
		void GetMethodSemantics_();

		// Token: 0x060000A3 RID: 163
		[PreserveSig]
		uint GetClassLayout(int typeDef, out uint dwPackSize, UnusedIntPtr zeroPtr, uint zeroCount, UnusedIntPtr zeroPtr2, ref uint ulClassSize);

		// Token: 0x060000A4 RID: 164
		void GetFieldMarshal_();

		// Token: 0x060000A5 RID: 165
		void GetRVA(int token, out uint rva, out uint flags);

		// Token: 0x060000A6 RID: 166
		void GetPermissionSetProps_();

		// Token: 0x060000A7 RID: 167
		void GetSigFromToken(int token, out EmbeddedBlobPointer pSig, out int cbSig);

		// Token: 0x060000A8 RID: 168
		void GetModuleRefProps(int mur, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, int cchName, [ComAliasName("ULONG*")] out int pchName);

		// Token: 0x060000A9 RID: 169
		void EnumModuleRefs(ref HCORENUM phEnum, [ComAliasName("mdModuleRef*")] out int mdModuleRef, int cMax, [ComAliasName("ULONG*")] out uint pcModuleRefs);

		// Token: 0x060000AA RID: 170
		[PreserveSig]
		int GetTypeSpecFromToken(Token typeSpec, out EmbeddedBlobPointer pSig, out int cbSig);

		// Token: 0x060000AB RID: 171
		void GetNameFromToken_();

		// Token: 0x060000AC RID: 172
		void EnumUnresolvedMethods_();

		// Token: 0x060000AD RID: 173
		void GetUserString([In] int stk, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] [Out] char[] szString, [In] int cchString, [ComAliasName("ULONG*")] out int pchString);

		// Token: 0x060000AE RID: 174
		void GetPinvokeMap_();

		// Token: 0x060000AF RID: 175
		void EnumSignatures_();

		// Token: 0x060000B0 RID: 176
		void EnumTypeSpecs_();

		// Token: 0x060000B1 RID: 177
		void EnumUserStrings_();

		// Token: 0x060000B2 RID: 178
		void GetParamForMethodIndex_();

		// Token: 0x060000B3 RID: 179
		void EnumCustomAttributes(ref HCORENUM phEnum, int tk, int tkType, [ComAliasName("mdCustomAttribute*")] out Token mdCustomAttribute, uint cMax, [ComAliasName("ULONG*")] out uint pcTokens);

		// Token: 0x060000B4 RID: 180
		void GetCustomAttributeProps([In] Token cv, out Token tkObj, out Token tkType, out EmbeddedBlobPointer blob, out int cbSize);

		// Token: 0x060000B5 RID: 181
		void FindTypeRef([In] int tkResolutionScope, [MarshalAs(UnmanagedType.LPWStr)] [In] string szName, out int typeRef);

		// Token: 0x060000B6 RID: 182
		void GetMemberProps_();

		// Token: 0x060000B7 RID: 183
		void GetFieldProps(int mb, [ComAliasName("mdTypeDef*")] out int mdTypeDef, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szField, int cchField, [ComAliasName("ULONG*")] out int pchField, [ComAliasName("DWORD*")] out FieldAttributes pdwAttr, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSigBlob, [ComAliasName("ULONG*")] out int pcbSigBlob, [ComAliasName("DWORD*")] out int pdwCPlusTypeFlab, [ComAliasName("UVCP_CONSTANT*")] out IntPtr ppValue, [ComAliasName("ULONG*")] out int pcchValue);

		// Token: 0x060000B8 RID: 184
		void GetPropertyProps(Token prop, [ComAliasName("mdTypeDef*")] out Token pClass, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szProperty, int cchProperty, [ComAliasName("ULONG*")] out int pchProperty, [ComAliasName("DWORD*")] out PropertyAttributes pdwPropFlags, [ComAliasName("PCCOR_SIGNATURE*")] out EmbeddedBlobPointer ppvSig, [ComAliasName("ULONG*")] out int pbSig, [ComAliasName("DWORD*")] out int pdwCPlusTypeFlag, [ComAliasName("UVCP_CONSTANT*")] out UnusedIntPtr ppDefaultValue, [ComAliasName("ULONG*")] out int pcchDefaultValue, [ComAliasName("mdMethodDef*")] out Token pmdSetter, [ComAliasName("mdMethodDef*")] out Token pmdGetter, [ComAliasName("mdMethodDef*")] out Token rmdOtherMethod, uint cMax, [ComAliasName("ULONG*")] out uint pcOtherMethod);

		// Token: 0x060000B9 RID: 185
		void GetParamProps(int tk, [ComAliasName("mdMethodDef*")] out int pmd, [ComAliasName("ULONG*")] out uint pulSequence, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szName, uint cchName, [ComAliasName("ULONG*")] out uint pchName, [ComAliasName("DWORD*")] out uint pdwAttr, [ComAliasName("DWORD*")] out uint pdwCPlusTypeFlag, [ComAliasName("UVCP_CONSTANT*")] out UnusedIntPtr ppValue, [ComAliasName("ULONG*")] out uint pcchValue);

		// Token: 0x060000BA RID: 186
		[PreserveSig]
		int GetCustomAttributeByName(int tkObj, [MarshalAs(UnmanagedType.LPWStr)] string szName, out EmbeddedBlobPointer ppData, out uint pcbData);

		// Token: 0x060000BB RID: 187
		[PreserveSig]
		bool IsValidToken([MarshalAs(UnmanagedType.U4)] [In] uint tk);

		// Token: 0x060000BC RID: 188
		[PreserveSig]
		int GetNestedClassProps(int tdNestedClass, [ComAliasName("mdTypeDef*")] out int tdEnclosingClass);

		// Token: 0x060000BD RID: 189
		void GetNativeCallConvFromSig_();

		// Token: 0x060000BE RID: 190
		void IsGlobal_();
	}
}
