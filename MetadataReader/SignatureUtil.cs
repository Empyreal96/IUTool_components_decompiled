using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200003D RID: 61
	internal static class SignatureUtil
	{
		// Token: 0x0600044C RID: 1100 RVA: 0x0000DB38 File Offset: 0x0000BD38
		internal static CorElementType ExtractElementType(byte[] sig, ref int index)
		{
			return (CorElementType)SignatureUtil.ExtractInt(sig, ref index);
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x0000DB54 File Offset: 0x0000BD54
		internal static CorCallingConvention ExtractCallingConvention(byte[] sig, ref int index)
		{
			return (CorCallingConvention)SignatureUtil.ExtractInt(sig, ref index);
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x0000DB70 File Offset: 0x0000BD70
		internal static CustomModifiers ExtractCustomModifiers(byte[] sig, ref int index, MetadataOnlyModule resolver, GenericContext context)
		{
			int num = index;
			CorElementType corElementType = SignatureUtil.ExtractElementType(sig, ref index);
			bool flag = corElementType == CorElementType.CModOpt || corElementType == CorElementType.CModReqd;
			CustomModifiers result;
			if (flag)
			{
				List<Type> list = new List<Type>();
				List<Type> list2 = new List<Type>();
				while (corElementType == CorElementType.CModOpt || corElementType == CorElementType.CModReqd)
				{
					Token token = SignatureUtil.ExtractToken(sig, ref index);
					Type item = resolver.ResolveTypeTokenInternal(token, context);
					bool flag2 = corElementType == CorElementType.CModOpt;
					if (flag2)
					{
						list.Add(item);
					}
					else
					{
						list2.Add(item);
					}
					num = index;
					corElementType = SignatureUtil.ExtractElementType(sig, ref index);
				}
				index = num;
				result = new CustomModifiers(list, list2);
			}
			else
			{
				index = num;
				result = null;
			}
			return result;
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x0000DC24 File Offset: 0x0000BE24
		internal static Type ExtractType(byte[] sig, ref int index, MetadataOnlyModule resolver, GenericContext context)
		{
			TypeSignatureDescriptor typeSignatureDescriptor = SignatureUtil.ExtractType(sig, ref index, resolver, context, false);
			return typeSignatureDescriptor.Type;
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x0000DC48 File Offset: 0x0000BE48
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		internal static TypeSignatureDescriptor ExtractType(byte[] sig, ref int index, MetadataOnlyModule resolver, GenericContext context, bool fAllowPinned)
		{
			TypeSignatureDescriptor typeSignatureDescriptor = new TypeSignatureDescriptor();
			typeSignatureDescriptor.IsPinned = false;
			CorElementType elementType = SignatureUtil.ExtractElementType(sig, ref index);
			switch (elementType)
			{
			case CorElementType.Void:
			case CorElementType.Bool:
			case CorElementType.Char:
			case CorElementType.SByte:
			case CorElementType.Byte:
			case CorElementType.Short:
			case CorElementType.UShort:
			case CorElementType.Int:
			case CorElementType.UInt:
			case CorElementType.Long:
			case CorElementType.ULong:
			case CorElementType.Float:
			case CorElementType.Double:
			case CorElementType.String:
			case CorElementType.IntPtr:
			case CorElementType.UIntPtr:
			case CorElementType.Object:
				typeSignatureDescriptor.Type = resolver.AssemblyResolver.GetBuiltInType(elementType);
				return typeSignatureDescriptor;
			case CorElementType.Pointer:
				typeSignatureDescriptor.Type = SignatureUtil.ExtractType(sig, ref index, resolver, context).MakePointerType();
				return typeSignatureDescriptor;
			case CorElementType.Byref:
				typeSignatureDescriptor.Type = SignatureUtil.ExtractType(sig, ref index, resolver, context).MakeByRefType();
				return typeSignatureDescriptor;
			case CorElementType.ValueType:
			case CorElementType.Class:
			{
				Token token = SignatureUtil.ExtractToken(sig, ref index);
				typeSignatureDescriptor.Type = resolver.ResolveTypeTokenInternal(token, context);
				return typeSignatureDescriptor;
			}
			case CorElementType.TypeVar:
			{
				int num = SignatureUtil.ExtractInt(sig, ref index);
				bool flag = GenericContext.IsNullOrEmptyTypeArgs(context);
				if (flag)
				{
					throw new ArgumentException(Resources.TypeArgumentCannotBeResolved);
				}
				typeSignatureDescriptor.Type = context.TypeArgs[num];
				return typeSignatureDescriptor;
			}
			case CorElementType.Array:
			{
				Type type = SignatureUtil.ExtractType(sig, ref index, resolver, context);
				int rank = SignatureUtil.ExtractInt(sig, ref index);
				int num2 = SignatureUtil.ExtractInt(sig, ref index);
				for (int i = 0; i < num2; i++)
				{
					SignatureUtil.ExtractInt(sig, ref index);
				}
				int num3 = SignatureUtil.ExtractInt(sig, ref index);
				for (int j = 0; j < num3; j++)
				{
					SignatureUtil.ExtractInt(sig, ref index);
				}
				typeSignatureDescriptor.Type = type.MakeArrayType(rank);
				return typeSignatureDescriptor;
			}
			case CorElementType.GenericInstantiation:
			{
				int num4 = index;
				Type type2 = SignatureUtil.ExtractType(sig, ref index, resolver, null);
				int num5 = SignatureUtil.ExtractInt(sig, ref index);
				Type[] array = new Type[num5];
				for (int k = 0; k < array.Length; k++)
				{
					array[k] = SignatureUtil.ExtractType(sig, ref index, resolver, context);
				}
				typeSignatureDescriptor.Type = type2.MakeGenericType(array);
				return typeSignatureDescriptor;
			}
			case CorElementType.TypedByRef:
				typeSignatureDescriptor.Type = resolver.AssemblyResolver.GetTypeXFromName("System.TypedReference");
				return typeSignatureDescriptor;
			case CorElementType.FnPtr:
			{
				SignatureUtil.ExtractCallingConvention(sig, ref index);
				int num6 = SignatureUtil.ExtractInt(sig, ref index);
				SignatureUtil.ExtractType(sig, ref index, resolver, context);
				for (int l = 0; l < num6; l++)
				{
					SignatureUtil.ExtractType(sig, ref index, resolver, context);
				}
				typeSignatureDescriptor.Type = resolver.AssemblyResolver.GetBuiltInType(CorElementType.IntPtr);
				return typeSignatureDescriptor;
			}
			case CorElementType.SzArray:
				typeSignatureDescriptor.Type = SignatureUtil.ExtractType(sig, ref index, resolver, context).MakeArrayType();
				return typeSignatureDescriptor;
			case CorElementType.MethodVar:
			{
				int num7 = SignatureUtil.ExtractInt(sig, ref index);
				bool flag2 = GenericContext.IsNullOrEmptyMethodArgs(context);
				if (flag2)
				{
					throw new ArgumentException(Resources.TypeArgumentCannotBeResolved);
				}
				typeSignatureDescriptor.Type = context.MethodArgs[num7];
				return typeSignatureDescriptor;
			}
			case CorElementType.CModReqd:
			{
				Token token2 = SignatureUtil.ExtractToken(sig, ref index);
				resolver.ResolveTypeTokenInternal(token2, context);
				typeSignatureDescriptor.Type = SignatureUtil.ExtractType(sig, ref index, resolver, context);
				return typeSignatureDescriptor;
			}
			case CorElementType.CModOpt:
			{
				Token token3 = SignatureUtil.ExtractToken(sig, ref index);
				resolver.ResolveTypeTokenInternal(token3, context);
				typeSignatureDescriptor.Type = SignatureUtil.ExtractType(sig, ref index, resolver, context);
				return typeSignatureDescriptor;
			}
			case CorElementType.Pinned:
				typeSignatureDescriptor.IsPinned = true;
				typeSignatureDescriptor.Type = SignatureUtil.ExtractType(sig, ref index, resolver, context);
				return typeSignatureDescriptor;
			}
			throw new ArgumentException(Resources.IncorrectElementTypeValue);
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x0000E044 File Offset: 0x0000C244
		internal static void ExtractCustomAttributeArgumentType(ITypeUniverse universe, Module module, byte[] customAttributeBlob, ref int index, out CorElementType argumentTypeId, out Type argumentType)
		{
			argumentTypeId = SignatureUtil.ExtractElementType(customAttributeBlob, ref index);
			SignatureUtil.VerifyElementType(argumentTypeId);
			bool flag = argumentTypeId == CorElementType.SzArray;
			if (flag)
			{
				CorElementType corElementType = SignatureUtil.ExtractElementType(customAttributeBlob, ref index);
				SignatureUtil.VerifyElementType(corElementType);
				bool flag2 = corElementType == (CorElementType)81;
				if (flag2)
				{
					argumentType = universe.GetBuiltInType(CorElementType.Object).MakeArrayType();
				}
				else
				{
					bool flag3 = corElementType == CorElementType.Enum;
					if (flag3)
					{
						argumentType = SignatureUtil.ExtractTypeValue(universe, module, customAttributeBlob, ref index);
						argumentType = argumentType.MakeArrayType();
					}
					else
					{
						argumentType = universe.GetBuiltInType(corElementType).MakeArrayType();
					}
				}
			}
			else
			{
				bool flag4 = argumentTypeId == CorElementType.Enum;
				if (flag4)
				{
					argumentType = SignatureUtil.ExtractTypeValue(universe, module, customAttributeBlob, ref index);
					bool flag5 = argumentType == null;
					if (flag5)
					{
						throw new ArgumentException(Resources.InvalidCustomAttributeFormatForEnum);
					}
				}
				else
				{
					bool flag6 = argumentTypeId == (CorElementType)81;
					if (flag6)
					{
						argumentType = null;
					}
					else
					{
						argumentType = universe.GetBuiltInType(argumentTypeId);
					}
				}
			}
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x0000E130 File Offset: 0x0000C330
		internal static bool IsVarArg(CorCallingConvention conv)
		{
			CorCallingConvention corCallingConvention = conv & CorCallingConvention.Mask;
			return corCallingConvention == CorCallingConvention.VarArg;
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x0000E14C File Offset: 0x0000C34C
		internal static int ExtractInt(byte[] sig, ref int index)
		{
			bool flag = (sig[index] & 128) == 0;
			int result;
			if (flag)
			{
				result = (int)sig[index];
				index++;
			}
			else
			{
				bool flag2 = (sig[index] & 192) == 128;
				if (flag2)
				{
					result = ((int)(sig[index] & 63) << 8 | (int)sig[index + 1]);
					index += 2;
				}
				else
				{
					bool flag3 = (sig[index] & 224) == 192;
					if (!flag3)
					{
						throw new ArgumentException(Resources.InvalidMetadataSignature);
					}
					result = ((int)(sig[index] & 31) << 24 | (int)sig[index + 1] << 16 | (int)sig[index + 2] << 8 | (int)sig[index + 3]);
					index += 4;
				}
			}
			return result;
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0000E204 File Offset: 0x0000C404
		internal static Token ExtractToken(byte[] sig, ref int index)
		{
			uint num = (uint)SignatureUtil.ExtractInt(sig, ref index);
			uint tkType = SignatureUtil.s_tkCorEncodeToken[(int)(num & 3U)];
			uint value = SignatureUtil.TokenFromRid(num >> 2, tkType);
			return new Token(value);
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x0000E238 File Offset: 0x0000C438
		internal static CorElementType GetTypeId(Type type)
		{
			bool isEnum = type.IsEnum;
			CorElementType result;
			if (isEnum)
			{
				result = SignatureUtil.GetTypeId(MetadataOnlyModule.GetUnderlyingType(type));
			}
			else
			{
				bool isArray = type.IsArray;
				if (isArray)
				{
					result = CorElementType.SzArray;
				}
				else
				{
					CorElementType corElementType;
					bool flag = SignatureUtil.TypeMapForAttributes.LookupPrimitive(type, out corElementType);
					bool flag2 = flag;
					if (!flag2)
					{
						throw new ArgumentException(Resources.UnsupportedTypeInAttributeSignature);
					}
					result = corElementType;
				}
			}
			return result;
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x0000E294 File Offset: 0x0000C494
		internal static string ExtractStringValue(byte[] blob, ref int index)
		{
			return (string)SignatureUtil.ExtractValue(CorElementType.String, blob, ref index);
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x0000E2B4 File Offset: 0x0000C4B4
		internal static uint ExtractUIntValue(byte[] blob, ref int index)
		{
			return (uint)SignatureUtil.ExtractValue(CorElementType.UInt, blob, ref index);
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x0000E2D4 File Offset: 0x0000C4D4
		internal static Type ExtractTypeValue(ITypeUniverse universe, Module module, byte[] blob, ref int index)
		{
			Type type = null;
			string text = SignatureUtil.ExtractStringValue(blob, ref index);
			bool flag = !string.IsNullOrEmpty(text);
			if (flag)
			{
				bool throwOnError = false;
				type = TypeNameParser.ParseTypeName(universe, module, text, throwOnError);
				bool flag2 = type == null;
				if (flag2)
				{
					module = universe.GetSystemAssembly().ManifestModule;
					type = TypeNameParser.ParseTypeName(universe, module, text);
				}
			}
			return type;
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x0000E330 File Offset: 0x0000C530
		internal static object ExtractValue(CorElementType typeId, byte[] blob, ref int index)
		{
			object result;
			switch (typeId)
			{
			case CorElementType.Bool:
			{
				object obj = BitConverter.ToBoolean(blob, index);
				index++;
				result = obj;
				break;
			}
			case CorElementType.Char:
			{
				object obj = BitConverter.ToChar(blob, index);
				index += 2;
				result = obj;
				break;
			}
			case CorElementType.SByte:
			{
				object obj = (sbyte)blob[index];
				index++;
				result = obj;
				break;
			}
			case CorElementType.Byte:
			{
				object obj = blob[index];
				index++;
				result = obj;
				break;
			}
			case CorElementType.Short:
			{
				object obj = BitConverter.ToInt16(blob, index);
				index += 2;
				result = obj;
				break;
			}
			case CorElementType.UShort:
			{
				object obj = BitConverter.ToUInt16(blob, index);
				index += 2;
				result = obj;
				break;
			}
			case CorElementType.Int:
			{
				object obj = BitConverter.ToInt32(blob, index);
				index += 4;
				result = obj;
				break;
			}
			case CorElementType.UInt:
			{
				object obj = BitConverter.ToUInt32(blob, index);
				index += 4;
				result = obj;
				break;
			}
			case CorElementType.Long:
			{
				object obj = BitConverter.ToInt64(blob, index);
				index += 8;
				result = obj;
				break;
			}
			case CorElementType.ULong:
			{
				object obj = BitConverter.ToUInt64(blob, index);
				index += 8;
				result = obj;
				break;
			}
			case CorElementType.Float:
			{
				object obj = BitConverter.ToSingle(blob, index);
				index += 4;
				result = obj;
				break;
			}
			case CorElementType.Double:
			{
				object obj = BitConverter.ToDouble(blob, index);
				index += 8;
				result = obj;
				break;
			}
			case CorElementType.String:
			{
				bool flag = blob[index] == byte.MaxValue;
				object obj;
				if (flag)
				{
					index++;
					obj = null;
				}
				else
				{
					int num = SignatureUtil.ExtractInt(blob, ref index);
					obj = Encoding.UTF8.GetString(blob, index, num);
					index += num;
				}
				result = obj;
				break;
			}
			default:
				throw new InvalidOperationException(Resources.IncorrectElementTypeValue);
			}
			return result;
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x0000E504 File Offset: 0x0000C704
		internal static IList<CustomAttributeTypedArgument> ExtractListOfValues(Type elementType, ITypeUniverse universe, Module module, uint size, byte[] blob, ref int index)
		{
			CorElementType typeId = SignatureUtil.GetTypeId(elementType);
			List<CustomAttributeTypedArgument> list = new List<CustomAttributeTypedArgument>((int)size);
			bool flag = typeId == CorElementType.Object;
			if (flag)
			{
				int num = 0;
				while ((long)num < (long)((ulong)size))
				{
					CorElementType corElementType = SignatureUtil.ExtractElementType(blob, ref index);
					SignatureUtil.VerifyElementType(corElementType);
					bool flag2 = corElementType == CorElementType.SzArray;
					if (flag2)
					{
						throw new NotImplementedException(Resources.ArrayInsideArrayInAttributeNotSupported);
					}
					bool flag3 = corElementType == CorElementType.Enum;
					Type type;
					object value;
					if (flag3)
					{
						type = SignatureUtil.ExtractTypeValue(universe, module, blob, ref index);
						bool flag4 = type != null;
						if (!flag4)
						{
							throw new ArgumentException(Resources.InvalidCustomAttributeFormatForEnum);
						}
						Type underlyingType = MetadataOnlyModule.GetUnderlyingType(type);
						CorElementType typeId2 = SignatureUtil.GetTypeId(underlyingType);
						value = SignatureUtil.ExtractValue(typeId2, blob, ref index);
					}
					else
					{
						type = universe.GetBuiltInType(corElementType);
						value = SignatureUtil.ExtractValue(corElementType, blob, ref index);
					}
					list.Add(new CustomAttributeTypedArgument(type, value));
					num++;
				}
			}
			else
			{
				bool flag5 = typeId == CorElementType.Type;
				if (flag5)
				{
					int num2 = 0;
					while ((long)num2 < (long)((ulong)size))
					{
						object value2 = SignatureUtil.ExtractTypeValue(universe, module, blob, ref index);
						list.Add(new CustomAttributeTypedArgument(elementType, value2));
						num2++;
					}
				}
				else
				{
					bool flag6 = typeId == CorElementType.SzArray;
					if (flag6)
					{
						throw new ArgumentException(Resources.JaggedArrayInAttributeNotSupported);
					}
					int num3 = 0;
					while ((long)num3 < (long)((ulong)size))
					{
						object value3 = SignatureUtil.ExtractValue(typeId, blob, ref index);
						list.Add(new CustomAttributeTypedArgument(elementType, value3));
						num3++;
					}
				}
			}
			return list.AsReadOnly();
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x0000E6A0 File Offset: 0x0000C8A0
		internal static bool IsValidCustomAttributeElementType(CorElementType elementType)
		{
			return SignatureUtil.TypeMapForAttributes.IsValidCustomAttributeElementType(elementType);
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x0000E6B8 File Offset: 0x0000C8B8
		internal static void VerifyElementType(CorElementType elementType)
		{
			bool flag = elementType == CorElementType.Bool || elementType == CorElementType.Char || elementType == CorElementType.SByte || elementType == CorElementType.Byte || elementType == CorElementType.Short || elementType == CorElementType.UShort || elementType == CorElementType.Int || elementType == CorElementType.UInt || elementType == CorElementType.Long || elementType == CorElementType.ULong || elementType == CorElementType.Float || elementType == CorElementType.Double || elementType == CorElementType.String || elementType == CorElementType.Type || elementType == CorElementType.SzArray || elementType == CorElementType.Enum || elementType == (CorElementType)81;
			if (flag)
			{
				return;
			}
			throw new ArgumentException(Resources.InvalidElementTypeInAttribute);
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x0000E72C File Offset: 0x0000C92C
		internal static NamedArgumentType ExtractNamedArgumentType(byte[] customAttributeBlob, ref int index)
		{
			byte b = (byte)SignatureUtil.ExtractValue(CorElementType.Byte, customAttributeBlob, ref index);
			bool flag = b == 84;
			NamedArgumentType result;
			if (flag)
			{
				result = NamedArgumentType.Property;
			}
			else
			{
				bool flag2 = b == 83;
				if (!flag2)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
					{
						Resources.InvalidCustomAttributeFormat,
						Resources.ExpectedPropertyOrFieldId
					}));
				}
				result = NamedArgumentType.Field;
			}
			return result;
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x0000E790 File Offset: 0x0000C990
		internal static MethodSignatureDescriptor ExtractMethodSignature(SignatureBlob methodSignatureBlob, MetadataOnlyModule resolver, GenericContext context)
		{
			byte[] signatureAsByteArray = methodSignatureBlob.GetSignatureAsByteArray();
			int num = 0;
			MethodSignatureDescriptor methodSignatureDescriptor = new MethodSignatureDescriptor();
			methodSignatureDescriptor.ReturnParameter = new TypeSignatureDescriptor();
			methodSignatureDescriptor.GenericParameterCount = 0;
			methodSignatureDescriptor.CallingConvention = SignatureUtil.ExtractCallingConvention(signatureAsByteArray, ref num);
			bool flag = (methodSignatureDescriptor.CallingConvention & CorCallingConvention.ExplicitThis) > CorCallingConvention.Default;
			bool flag2 = (methodSignatureDescriptor.CallingConvention & CorCallingConvention.Generic) > CorCallingConvention.Default;
			bool flag3 = flag2;
			if (flag3)
			{
				int num2 = SignatureUtil.ExtractInt(signatureAsByteArray, ref num);
				bool flag4 = num2 <= 0;
				if (flag4)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
					{
						Resources.InvalidMetadataSignature,
						Resources.ExpectedPositiveNumberOfGenericParameters
					}));
				}
				context = context.VerifyAndUpdateMethodArguments(num2);
				methodSignatureDescriptor.GenericParameterCount = num2;
			}
			int num3 = SignatureUtil.ExtractInt(signatureAsByteArray, ref num);
			bool fAllowPinned = false;
			CustomModifiers customModifiers = SignatureUtil.ExtractCustomModifiers(signatureAsByteArray, ref num, resolver, context);
			methodSignatureDescriptor.ReturnParameter = SignatureUtil.ExtractType(signatureAsByteArray, ref num, resolver, context, fAllowPinned);
			methodSignatureDescriptor.ReturnParameter.CustomModifiers = customModifiers;
			bool flag5 = flag;
			if (flag5)
			{
				SignatureUtil.ExtractType(signatureAsByteArray, ref num, resolver, context);
				num3--;
			}
			methodSignatureDescriptor.Parameters = new TypeSignatureDescriptor[num3];
			for (int i = 0; i < num3; i++)
			{
				customModifiers = SignatureUtil.ExtractCustomModifiers(signatureAsByteArray, ref num, resolver, context);
				methodSignatureDescriptor.Parameters[i] = SignatureUtil.ExtractType(signatureAsByteArray, ref num, resolver, context, fAllowPinned);
				methodSignatureDescriptor.Parameters[i].CustomModifiers = customModifiers;
			}
			bool flag6 = num != signatureAsByteArray.Length;
			if (flag6)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
				{
					Resources.InvalidMetadataSignature,
					Resources.ExtraInformationAfterLastParameter
				}));
			}
			return methodSignatureDescriptor;
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x0000E93C File Offset: 0x0000CB3C
		internal static CallingConventions GetReflectionCallingConvention(CorCallingConvention callConvention)
		{
			CallingConventions callingConventions = (CallingConventions)0;
			bool flag = (callConvention & CorCallingConvention.Mask) == CorCallingConvention.HasThis;
			if (flag)
			{
				callingConventions |= CallingConventions.HasThis;
			}
			else
			{
				bool flag2 = (callConvention & CorCallingConvention.Mask) == CorCallingConvention.ExplicitThis;
				if (flag2)
				{
					callingConventions |= CallingConventions.ExplicitThis;
				}
			}
			bool flag3 = SignatureUtil.IsVarArg(callConvention);
			if (flag3)
			{
				callingConventions |= CallingConventions.VarArgs;
			}
			else
			{
				callingConventions |= CallingConventions.Standard;
			}
			return callingConventions;
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x0000E994 File Offset: 0x0000CB94
		internal static bool IsCallingConventionMatch(MethodBase method, CallingConventions callConvention)
		{
			bool flag = (callConvention & CallingConventions.Any) == (CallingConventions)0;
			if (flag)
			{
				bool flag2 = (callConvention & CallingConventions.VarArgs) != (CallingConventions)0 && (method.CallingConvention & CallingConventions.VarArgs) == (CallingConventions)0;
				if (flag2)
				{
					return false;
				}
				bool flag3 = (callConvention & CallingConventions.Standard) != (CallingConventions)0 && (method.CallingConvention & CallingConventions.Standard) == (CallingConventions)0;
				if (flag3)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x0000E9EC File Offset: 0x0000CBEC
		internal static bool IsGenericParametersCountMatch(MethodInfo method, int expectedGenericParameterCount)
		{
			int num = 0;
			bool isGenericMethod = method.IsGenericMethod;
			if (isGenericMethod)
			{
				num = method.GetGenericArguments().Length;
			}
			return num == expectedGenericParameterCount;
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x0000EA1C File Offset: 0x0000CC1C
		internal static bool IsParametersTypeMatch(MethodBase method, Type[] parameterTypes)
		{
			bool flag = parameterTypes == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				ParameterInfo[] parameters = method.GetParameters();
				bool flag2 = parameters.Length != parameterTypes.Length;
				if (flag2)
				{
					result = false;
				}
				else
				{
					int num = parameters.Length;
					for (int i = 0; i < num; i++)
					{
						bool flag3 = !parameters[i].ParameterType.Equals(parameterTypes[i]);
						if (flag3)
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x0000EA94 File Offset: 0x0000CC94
		private static uint TokenFromRid(uint rid, uint tkType)
		{
			return rid | tkType;
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x0000EAAC File Offset: 0x0000CCAC
		internal static StringComparison GetStringComparison(BindingFlags flags)
		{
			bool flag = (flags & BindingFlags.IgnoreCase) > BindingFlags.Default;
			StringComparison result;
			if (flag)
			{
				result = StringComparison.OrdinalIgnoreCase;
			}
			else
			{
				result = StringComparison.Ordinal;
			}
			return result;
		}

		// Token: 0x040000D7 RID: 215
		private static readonly uint[] s_tkCorEncodeToken = new uint[]
		{
			33554432U,
			16777216U,
			452984832U,
			1912602624U
		};

		// Token: 0x040000D8 RID: 216
		private const byte FieldId = 83;

		// Token: 0x040000D9 RID: 217
		private const byte PropertyId = 84;

		// Token: 0x040000DA RID: 218
		private const CorElementType BoxedValue = (CorElementType)81;

		// Token: 0x02000066 RID: 102
		public static class TypeMapForAttributes
		{
			// Token: 0x06000558 RID: 1368 RVA: 0x000119BC File Offset: 0x0000FBBC
			public static bool IsValidCustomAttributeElementType(CorElementType elementType)
			{
				return SignatureUtil.TypeMapForAttributes.s_typeNameMapForAttributes.ContainsValue(elementType);
			}

			// Token: 0x06000559 RID: 1369 RVA: 0x000119DC File Offset: 0x0000FBDC
			public static bool LookupPrimitive(Type type, out CorElementType result)
			{
				result = CorElementType.End;
				ITypeUniverse typeUniverse = Helpers.Universe(type);
				bool flag = typeUniverse != null;
				if (flag)
				{
					bool flag2 = typeUniverse.GetSystemAssembly().Equals(type.Assembly);
					bool flag3 = !flag2;
					if (flag3)
					{
						return false;
					}
				}
				return SignatureUtil.TypeMapForAttributes.s_typeNameMapForAttributes.TryGetValue(type.FullName, out result);
			}

			// Token: 0x040001BF RID: 447
			private static readonly Dictionary<string, CorElementType> s_typeNameMapForAttributes = new Dictionary<string, CorElementType>
			{
				{
					"System.Boolean",
					CorElementType.Bool
				},
				{
					"System.Char",
					CorElementType.Char
				},
				{
					"System.SByte",
					CorElementType.SByte
				},
				{
					"System.Byte",
					CorElementType.Byte
				},
				{
					"System.Int16",
					CorElementType.Short
				},
				{
					"System.UInt16",
					CorElementType.UShort
				},
				{
					"System.Int32",
					CorElementType.Int
				},
				{
					"System.UInt32",
					CorElementType.UInt
				},
				{
					"System.Int64",
					CorElementType.Long
				},
				{
					"System.UInt64",
					CorElementType.ULong
				},
				{
					"System.Single",
					CorElementType.Float
				},
				{
					"System.Double",
					CorElementType.Double
				},
				{
					"System.IntPtr",
					CorElementType.IntPtr
				},
				{
					"System.UIntPtr",
					CorElementType.UIntPtr
				},
				{
					"System.Array",
					CorElementType.SzArray
				},
				{
					"System.String",
					CorElementType.String
				},
				{
					"System.Type",
					CorElementType.Type
				},
				{
					"System.Object",
					CorElementType.Object
				}
			};
		}
	}
}
