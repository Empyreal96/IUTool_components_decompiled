using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000037 RID: 55
	internal static class PseudoCustomAttributes
	{
		// Token: 0x06000423 RID: 1059 RVA: 0x0000D3E4 File Offset: 0x0000B5E4
		public static IEnumerable<CustomAttributeData> GetTypeForwardedToAttributes(MetadataOnlyAssembly assembly)
		{
			return PseudoCustomAttributes.GetTypeForwardedToAttributes(assembly.ManifestModuleInternal);
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0000D404 File Offset: 0x0000B604
		public static IEnumerable<CustomAttributeData> GetTypeForwardedToAttributes(MetadataOnlyModule manifestModule, Token token)
		{
			bool flag = token.TokenType != TokenType.Assembly;
			IEnumerable<CustomAttributeData> result;
			if (flag)
			{
				result = new CustomAttributeData[0];
			}
			else
			{
				result = PseudoCustomAttributes.GetTypeForwardedToAttributes(manifestModule);
			}
			return result;
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x0000D43B File Offset: 0x0000B63B
		public static IEnumerable<CustomAttributeData> GetTypeForwardedToAttributes(MetadataOnlyModule manifestModule)
		{
			ITypeUniverse itu = manifestModule.AssemblyResolver;
			Type argumentType = itu.GetBuiltInType(CorElementType.Type);
			Assembly systemAssembly = itu.GetSystemAssembly();
			Type attributeType = systemAssembly.GetType("System.Runtime.CompilerServices.TypeForwardedToAttribute", false, false);
			bool flag = attributeType == null;
			if (flag)
			{
				yield break;
			}
			IEnumerable<UnresolvedTypeName> raw = PseudoCustomAttributes.GetRawTypeForwardedToAttributes(manifestModule);
			foreach (UnresolvedTypeName utn in raw)
			{
				ConstructorInfo[] constructors = attributeType.GetConstructors();
				Type argumentValue = utn.ConvertToType(itu, manifestModule);
				CustomAttributeTypedArgument forwardedTypeInfo = new CustomAttributeTypedArgument(argumentType, argumentValue);
				List<CustomAttributeTypedArgument> typedArguments = new List<CustomAttributeTypedArgument>(1);
				typedArguments.Add(forwardedTypeInfo);
				List<CustomAttributeNamedArgument> namedArguments = new List<CustomAttributeNamedArgument>(0);
				MetadataOnlyCustomAttributeData attribute = new MetadataOnlyCustomAttributeData(constructors[0], typedArguments, namedArguments);
				yield return attribute;
				constructors = null;
				argumentValue = null;
				forwardedTypeInfo = default(CustomAttributeTypedArgument);
				typedArguments = null;
				namedArguments = null;
				attribute = null;
				utn = null;
			}
			IEnumerator<UnresolvedTypeName> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x0000D44C File Offset: 0x0000B64C
		internal static IEnumerable<UnresolvedTypeName> GetRawTypeForwardedToAttributes(MetadataOnlyAssembly assembly)
		{
			return PseudoCustomAttributes.GetRawTypeForwardedToAttributes(assembly.ManifestModuleInternal);
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x0000D46C File Offset: 0x0000B66C
		internal static bool GetNextExportedType(ref HCORENUM hEnum, IMetadataAssemblyImport assemblyImport, StringBuilder typeName, out Token implementationToken)
		{
			implementationToken = Token.Nil;
			int mdct;
			uint num;
			assemblyImport.EnumExportedTypes(ref hEnum, out mdct, 1, out num);
			bool flag = num == 0U;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int capacity;
				int value;
				int num2;
				CorTypeAttr corTypeAttr;
				assemblyImport.GetExportedTypeProps(mdct, null, 0, out capacity, out value, out num2, out corTypeAttr);
				implementationToken = new Token(value);
				bool flag2 = implementationToken.TokenType == TokenType.AssemblyRef;
				if (flag2)
				{
					typeName.Length = 0;
					typeName.EnsureCapacity(capacity);
					assemblyImport.GetExportedTypeProps(mdct, typeName, typeName.Capacity, out capacity, out value, out num2, out corTypeAttr);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x0000D503 File Offset: 0x0000B703
		internal static IEnumerable<UnresolvedTypeName> GetRawTypeForwardedToAttributes(MetadataOnlyModule manifestModule)
		{
			HCORENUM hEnum = default(HCORENUM);
			IMetadataAssemblyImport assemblyImport = (IMetadataAssemblyImport)manifestModule.RawImport;
			try
			{
				StringBuilder typeName = StringBuilderPool.Get();
				Token implementationToken;
				while (PseudoCustomAttributes.GetNextExportedType(ref hEnum, assemblyImport, typeName, out implementationToken))
				{
					bool flag = implementationToken.TokenType == TokenType.AssemblyRef;
					if (flag)
					{
						AssemblyName assemblyName = AssemblyNameHelper.GetAssemblyNameFromRef(implementationToken, manifestModule, assemblyImport);
						yield return new UnresolvedTypeName(typeName.ToString(), assemblyName);
						assemblyName = null;
					}
				}
				StringBuilderPool.Release(ref typeName);
				typeName = null;
				implementationToken = default(Token);
			}
			finally
			{
				hEnum.Close(assemblyImport);
			}
			yield break;
			yield break;
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x0000D514 File Offset: 0x0000B714
		internal static UnresolvedTypeName GetRawTypeForwardedToAttribute(MetadataOnlyAssembly assembly, string fullname, bool ignoreCase)
		{
			return PseudoCustomAttributes.GetRawTypeForwardedToAttribute(assembly.ManifestModuleInternal, fullname, ignoreCase);
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x0000D534 File Offset: 0x0000B734
		internal static UnresolvedTypeName GetRawTypeForwardedToAttribute(MetadataOnlyModule manifestModule, string fullname, bool ignoreCase)
		{
			HCORENUM hcorenum = default(HCORENUM);
			IMetadataAssemblyImport metadataAssemblyImport = (IMetadataAssemblyImport)manifestModule.RawImport;
			bool flag = string.IsNullOrEmpty(fullname);
			UnresolvedTypeName result;
			if (flag)
			{
				result = null;
			}
			else
			{
				UnresolvedTypeName unresolvedTypeName = null;
				try
				{
					StringBuilder stringBuilder = StringBuilderPool.Get();
					Token assemblyRefToken;
					while (PseudoCustomAttributes.GetNextExportedType(ref hcorenum, metadataAssemblyImport, stringBuilder, out assemblyRefToken))
					{
						bool flag2 = assemblyRefToken.TokenType == TokenType.AssemblyRef;
						if (flag2)
						{
							bool flag3 = fullname.Length != stringBuilder.Length;
							if (!flag3)
							{
								string text = stringBuilder.ToString();
								bool flag4 = !Utility.Compare(text, fullname, ignoreCase);
								if (!flag4)
								{
									AssemblyName assemblyNameFromRef = AssemblyNameHelper.GetAssemblyNameFromRef(assemblyRefToken, manifestModule, metadataAssemblyImport);
									unresolvedTypeName = new UnresolvedTypeName(text, assemblyNameFromRef);
									break;
								}
							}
						}
					}
					StringBuilderPool.Release(ref stringBuilder);
				}
				finally
				{
					hcorenum.Close(metadataAssemblyImport);
				}
				result = unresolvedTypeName;
			}
			return result;
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x0000D61C File Offset: 0x0000B81C
		public static Type GetTypeFromTypeForwardToAttribute(CustomAttributeData data)
		{
			return (Type)data.ConstructorArguments[0].Value;
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x0000D64C File Offset: 0x0000B84C
		public static CustomAttributeData GetSerializableAttribute(MetadataOnlyModule module, Token token)
		{
			bool flag = token.TokenType != TokenType.TypeDef;
			CustomAttributeData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int num;
				TypeAttributes typeAttributes;
				int num2;
				module.RawImport.GetTypeDefProps(token.Value, null, 0, out num, out typeAttributes, out num2);
				bool flag2 = (typeAttributes & TypeAttributes.Serializable) == TypeAttributes.NotPublic;
				if (flag2)
				{
					result = null;
				}
				else
				{
					result = PseudoCustomAttributes.GetSerializableAttribute(module, true);
				}
			}
			return result;
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x0000D6B4 File Offset: 0x0000B8B4
		internal static CustomAttributeData GetSerializableAttribute(MetadataOnlyModule module, bool isRequired)
		{
			Assembly systemAssembly = module.AssemblyResolver.GetSystemAssembly();
			Type type = systemAssembly.GetType("System.SerializableAttribute", isRequired, false);
			bool flag = type == null;
			CustomAttributeData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				ConstructorInfo[] constructors = type.GetConstructors();
				List<CustomAttributeTypedArgument> typedArguments = new List<CustomAttributeTypedArgument>(0);
				List<CustomAttributeNamedArgument> namedArguments = new List<CustomAttributeNamedArgument>(0);
				MetadataOnlyCustomAttributeData metadataOnlyCustomAttributeData = new MetadataOnlyCustomAttributeData(constructors[0], typedArguments, namedArguments);
				result = metadataOnlyCustomAttributeData;
			}
			return result;
		}

		// Token: 0x040000C6 RID: 198
		public const string TypeForwardedToAttributeName = "System.Runtime.CompilerServices.TypeForwardedToAttribute";

		// Token: 0x040000C7 RID: 199
		public const string SerializableAttributeName = "System.SerializableAttribute";
	}
}
