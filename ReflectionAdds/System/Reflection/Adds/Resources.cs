using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace System.Reflection.Adds
{
	// Token: 0x0200001C RID: 28
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x060000B0 RID: 176 RVA: 0x00003E20 File Offset: 0x00002020
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Resources()
		{
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00003E2C File Offset: 0x0000202C
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				bool flag = Resources.resourceMan == null;
				if (flag)
				{
					ResourceManager resourceManager = new ResourceManager("ReflectionAdds.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = resourceManager;
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x00003E74 File Offset: 0x00002074
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x00003E8B File Offset: 0x0000208B
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x00003E94 File Offset: 0x00002094
		internal static string ArrayInsideArrayInAttributeNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("ArrayInsideArrayInAttributeNotSupported", Resources.resourceCulture);
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00003EBC File Offset: 0x000020BC
		internal static string AssemblyRefTokenExpected
		{
			get
			{
				return Resources.ResourceManager.GetString("AssemblyRefTokenExpected", Resources.resourceCulture);
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00003EE4 File Offset: 0x000020E4
		internal static string CannotDetermineSystemAssembly
		{
			get
			{
				return Resources.ResourceManager.GetString("CannotDetermineSystemAssembly", Resources.resourceCulture);
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x00003F0C File Offset: 0x0000210C
		internal static string CannotFindTypeInModule
		{
			get
			{
				return Resources.ResourceManager.GetString("CannotFindTypeInModule", Resources.resourceCulture);
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x00003F34 File Offset: 0x00002134
		internal static string CannotResolveModuleRefOnNetModule
		{
			get
			{
				return Resources.ResourceManager.GetString("CannotResolveModuleRefOnNetModule", Resources.resourceCulture);
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00003F5C File Offset: 0x0000215C
		internal static string CannotResolveRVA
		{
			get
			{
				return Resources.ResourceManager.GetString("CannotResolveRVA", Resources.resourceCulture);
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00003F84 File Offset: 0x00002184
		internal static string CaseInsensitiveTypeLookupNotImplemented
		{
			get
			{
				return Resources.ResourceManager.GetString("CaseInsensitiveTypeLookupNotImplemented", Resources.resourceCulture);
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00003FAC File Offset: 0x000021AC
		internal static string CorruptImage
		{
			get
			{
				return Resources.ResourceManager.GetString("CorruptImage", Resources.resourceCulture);
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00003FD4 File Offset: 0x000021D4
		internal static string DefaultTokenResolverRequired
		{
			get
			{
				return Resources.ResourceManager.GetString("DefaultTokenResolverRequired", Resources.resourceCulture);
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000BD RID: 189 RVA: 0x00003FFC File Offset: 0x000021FC
		internal static string DifferentTokenResolverForOuterType
		{
			get
			{
				return Resources.ResourceManager.GetString("DifferentTokenResolverForOuterType", Resources.resourceCulture);
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00004024 File Offset: 0x00002224
		internal static string EscapeSequenceMissingCharacter
		{
			get
			{
				return Resources.ResourceManager.GetString("EscapeSequenceMissingCharacter", Resources.resourceCulture);
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000BF RID: 191 RVA: 0x0000404C File Offset: 0x0000224C
		internal static string ExpectedPositiveNumberOfGenericParameters
		{
			get
			{
				return Resources.ResourceManager.GetString("ExpectedPositiveNumberOfGenericParameters", Resources.resourceCulture);
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00004074 File Offset: 0x00002274
		internal static string ExpectedPropertyOrFieldId
		{
			get
			{
				return Resources.ResourceManager.GetString("ExpectedPropertyOrFieldId", Resources.resourceCulture);
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x0000409C File Offset: 0x0000229C
		internal static string ExpectedTokenType
		{
			get
			{
				return Resources.ResourceManager.GetString("ExpectedTokenType", Resources.resourceCulture);
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x000040C4 File Offset: 0x000022C4
		internal static string ExtraAssemblyManifest
		{
			get
			{
				return Resources.ResourceManager.GetString("ExtraAssemblyManifest", Resources.resourceCulture);
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x000040EC File Offset: 0x000022EC
		internal static string ExtraCharactersAfterTypeName
		{
			get
			{
				return Resources.ResourceManager.GetString("ExtraCharactersAfterTypeName", Resources.resourceCulture);
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x00004114 File Offset: 0x00002314
		internal static string ExtraInformationAfterLastParameter
		{
			get
			{
				return Resources.ResourceManager.GetString("ExtraInformationAfterLastParameter", Resources.resourceCulture);
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x0000413C File Offset: 0x0000233C
		internal static string HostSpecifierMissing
		{
			get
			{
				return Resources.ResourceManager.GetString("HostSpecifierMissing", Resources.resourceCulture);
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00004164 File Offset: 0x00002364
		internal static string IdTokenTypeExpected
		{
			get
			{
				return Resources.ResourceManager.GetString("IdTokenTypeExpected", Resources.resourceCulture);
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x0000418C File Offset: 0x0000238C
		internal static string IllegalElementType
		{
			get
			{
				return Resources.ResourceManager.GetString("IllegalElementType", Resources.resourceCulture);
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x000041B4 File Offset: 0x000023B4
		internal static string IllegalLayoutMask
		{
			get
			{
				return Resources.ResourceManager.GetString("IllegalLayoutMask", Resources.resourceCulture);
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x000041DC File Offset: 0x000023DC
		internal static string IncorrectElementTypeValue
		{
			get
			{
				return Resources.ResourceManager.GetString("IncorrectElementTypeValue", Resources.resourceCulture);
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00004204 File Offset: 0x00002404
		internal static string InvalidCustomAttributeFormat
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidCustomAttributeFormat", Resources.resourceCulture);
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000CB RID: 203 RVA: 0x0000422C File Offset: 0x0000242C
		internal static string InvalidCustomAttributeFormatForEnum
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidCustomAttributeFormatForEnum", Resources.resourceCulture);
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00004254 File Offset: 0x00002454
		internal static string InvalidElementTypeInAttribute
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidElementTypeInAttribute", Resources.resourceCulture);
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000CD RID: 205 RVA: 0x0000427C File Offset: 0x0000247C
		internal static string InvalidFileFormat
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidFileFormat", Resources.resourceCulture);
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000CE RID: 206 RVA: 0x000042A4 File Offset: 0x000024A4
		internal static string InvalidFileName
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidFileName", Resources.resourceCulture);
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000CF RID: 207 RVA: 0x000042CC File Offset: 0x000024CC
		internal static string InvalidMetadata
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidMetadata", Resources.resourceCulture);
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x000042F4 File Offset: 0x000024F4
		internal static string InvalidMetadataSignature
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidMetadataSignature", Resources.resourceCulture);
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x0000431C File Offset: 0x0000251C
		internal static string InvalidMetadataToken
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidMetadataToken", Resources.resourceCulture);
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00004344 File Offset: 0x00002544
		internal static string InvalidPublicKeyTokenLength
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidPublicKeyTokenLength", Resources.resourceCulture);
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x0000436C File Offset: 0x0000256C
		internal static string JaggedArrayInAttributeNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("JaggedArrayInAttributeNotSupported", Resources.resourceCulture);
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00004394 File Offset: 0x00002594
		internal static string ManifestModuleMustBeProvided
		{
			get
			{
				return Resources.ResourceManager.GetString("ManifestModuleMustBeProvided", Resources.resourceCulture);
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x000043BC File Offset: 0x000025BC
		internal static string MethodIsUsingUnsupportedBindingFlags
		{
			get
			{
				return Resources.ResourceManager.GetString("MethodIsUsingUnsupportedBindingFlags", Resources.resourceCulture);
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x000043E4 File Offset: 0x000025E4
		internal static string MethodTokenExpected
		{
			get
			{
				return Resources.ResourceManager.GetString("MethodTokenExpected", Resources.resourceCulture);
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x0000440C File Offset: 0x0000260C
		internal static string NoAssemblyManifest
		{
			get
			{
				return Resources.ResourceManager.GetString("NoAssemblyManifest", Resources.resourceCulture);
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00004434 File Offset: 0x00002634
		internal static string OperationInvalidOnAutoLayoutFields
		{
			get
			{
				return Resources.ResourceManager.GetString("OperationInvalidOnAutoLayoutFields", Resources.resourceCulture);
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x0000445C File Offset: 0x0000265C
		internal static string OperationValidOnArrayTypeOnly
		{
			get
			{
				return Resources.ResourceManager.GetString("OperationValidOnArrayTypeOnly", Resources.resourceCulture);
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00004484 File Offset: 0x00002684
		internal static string OperationValidOnEnumOnly
		{
			get
			{
				return Resources.ResourceManager.GetString("OperationValidOnEnumOnly", Resources.resourceCulture);
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000DB RID: 219 RVA: 0x000044AC File Offset: 0x000026AC
		internal static string OperationValidOnLiteralFieldsOnly
		{
			get
			{
				return Resources.ResourceManager.GetString("OperationValidOnLiteralFieldsOnly", Resources.resourceCulture);
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000DC RID: 220 RVA: 0x000044D4 File Offset: 0x000026D4
		internal static string OperationValidOnRVAFieldsOnly
		{
			get
			{
				return Resources.ResourceManager.GetString("OperationValidOnRVAFieldsOnly", Resources.resourceCulture);
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000DD RID: 221 RVA: 0x000044FC File Offset: 0x000026FC
		internal static string ResolvedAssemblyMustBeWithinSameUniverse
		{
			get
			{
				return Resources.ResourceManager.GetString("ResolvedAssemblyMustBeWithinSameUniverse", Resources.resourceCulture);
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000DE RID: 222 RVA: 0x00004524 File Offset: 0x00002724
		internal static string ResolverMustResolveToValidAssembly
		{
			get
			{
				return Resources.ResourceManager.GetString("ResolverMustResolveToValidAssembly", Resources.resourceCulture);
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000DF RID: 223 RVA: 0x0000454C File Offset: 0x0000274C
		internal static string ResolverMustResolveToValidModule
		{
			get
			{
				return Resources.ResourceManager.GetString("ResolverMustResolveToValidModule", Resources.resourceCulture);
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x00004574 File Offset: 0x00002774
		internal static string ResolverMustSetAssemblyProperty
		{
			get
			{
				return Resources.ResourceManager.GetString("ResolverMustSetAssemblyProperty", Resources.resourceCulture);
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x0000459C File Offset: 0x0000279C
		internal static string RVAUnsupported
		{
			get
			{
				return Resources.ResourceManager.GetString("RVAUnsupported", Resources.resourceCulture);
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x000045C4 File Offset: 0x000027C4
		internal static string TypeArgumentCannotBeResolved
		{
			get
			{
				return Resources.ResourceManager.GetString("TypeArgumentCannotBeResolved", Resources.resourceCulture);
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x000045EC File Offset: 0x000027EC
		internal static string TypeTokenExpected
		{
			get
			{
				return Resources.ResourceManager.GetString("TypeTokenExpected", Resources.resourceCulture);
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00004614 File Offset: 0x00002814
		internal static string UnexpectedCharacterFound
		{
			get
			{
				return Resources.ResourceManager.GetString("UnexpectedCharacterFound", Resources.resourceCulture);
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060000E5 RID: 229 RVA: 0x0000463C File Offset: 0x0000283C
		internal static string UnexpectedEndOfInput
		{
			get
			{
				return Resources.ResourceManager.GetString("UnexpectedEndOfInput", Resources.resourceCulture);
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00004664 File Offset: 0x00002864
		internal static string UniverseCannotResolveAssembly
		{
			get
			{
				return Resources.ResourceManager.GetString("UniverseCannotResolveAssembly", Resources.resourceCulture);
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x0000468C File Offset: 0x0000288C
		internal static string UnrecognizedAssemblyAttribute
		{
			get
			{
				return Resources.ResourceManager.GetString("UnrecognizedAssemblyAttribute", Resources.resourceCulture);
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x000046B4 File Offset: 0x000028B4
		internal static string UnsupportedExceptionFlags
		{
			get
			{
				return Resources.ResourceManager.GetString("UnsupportedExceptionFlags", Resources.resourceCulture);
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x000046DC File Offset: 0x000028DC
		internal static string UnsupportedImageType
		{
			get
			{
				return Resources.ResourceManager.GetString("UnsupportedImageType", Resources.resourceCulture);
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00004704 File Offset: 0x00002904
		internal static string UnsupportedTypeInAttributeSignature
		{
			get
			{
				return Resources.ResourceManager.GetString("UnsupportedTypeInAttributeSignature", Resources.resourceCulture);
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060000EB RID: 235 RVA: 0x0000472C File Offset: 0x0000292C
		internal static string ValidOnGenericParameterTypeOnly
		{
			get
			{
				return Resources.ResourceManager.GetString("ValidOnGenericParameterTypeOnly", Resources.resourceCulture);
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00004754 File Offset: 0x00002954
		internal static string VarargSignaturesNotImplemented
		{
			get
			{
				return Resources.ResourceManager.GetString("VarargSignaturesNotImplemented", Resources.resourceCulture);
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060000ED RID: 237 RVA: 0x0000477C File Offset: 0x0000297C
		internal static string VersionAlreadyDefined
		{
			get
			{
				return Resources.ResourceManager.GetString("VersionAlreadyDefined", Resources.resourceCulture);
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060000EE RID: 238 RVA: 0x000047A4 File Offset: 0x000029A4
		internal static string WrongNumberOfGenericArguments
		{
			get
			{
				return Resources.ResourceManager.GetString("WrongNumberOfGenericArguments", Resources.resourceCulture);
			}
		}

		// Token: 0x04000049 RID: 73
		private static ResourceManager resourceMan;

		// Token: 0x0400004A RID: 74
		private static CultureInfo resourceCulture;
	}
}
