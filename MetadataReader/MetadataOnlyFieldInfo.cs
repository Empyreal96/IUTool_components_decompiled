using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000026 RID: 38
	internal class MetadataOnlyFieldInfo : FieldInfo, IFieldInfo2
	{
		// Token: 0x060001F0 RID: 496 RVA: 0x000055FC File Offset: 0x000037FC
		public MetadataOnlyFieldInfo(MetadataOnlyModule resolver, Token fieldDefToken, Type[] typeArgs, Type[] methodArgs)
		{
			this._resolver = resolver;
			this._fieldDefToken = fieldDefToken;
			bool flag = typeArgs != null || methodArgs != null;
			if (flag)
			{
				this._context = new GenericContext(typeArgs, methodArgs);
			}
			IMetadataImport rawImport = this._resolver.RawImport;
			FieldAttributes fieldAttributes;
			EmbeddedBlobPointer embeddedBlobPointer;
			int num;
			int num2;
			IntPtr intPtr;
			int num3;
			rawImport.GetFieldProps(this._fieldDefToken, out this._declaringClassToken, null, 0, out this._nameLength, out fieldAttributes, out embeddedBlobPointer, out num, out num2, out intPtr, out num3);
			this.Attributes = fieldAttributes;
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00005680 File Offset: 0x00003880
		private void InitializeName()
		{
			bool flag = string.IsNullOrEmpty(this._name);
			if (flag)
			{
				IMetadataImport rawImport = this._resolver.RawImport;
				StringBuilder stringBuilder = StringBuilderPool.Get(this._nameLength);
				int num;
				int num2;
				FieldAttributes fieldAttributes;
				EmbeddedBlobPointer embeddedBlobPointer;
				int num3;
				int num4;
				IntPtr intPtr;
				int num5;
				rawImport.GetFieldProps(this._fieldDefToken, out num, stringBuilder, stringBuilder.Capacity, out num2, out fieldAttributes, out embeddedBlobPointer, out num3, out num4, out intPtr, out num5);
				this._name = stringBuilder.ToString();
				StringBuilderPool.Release(ref stringBuilder);
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x000056F4 File Offset: 0x000038F4
		private void Initialize()
		{
			bool initialized = this._initialized;
			if (!initialized)
			{
				IMetadataImport rawImport = this._resolver.RawImport;
				int num;
				int num2;
				FieldAttributes fieldAttributes;
				EmbeddedBlobPointer pointer;
				int countBytes;
				int num3;
				IntPtr intPtr;
				int num4;
				rawImport.GetFieldProps(this._fieldDefToken, out num, null, 0, out num2, out fieldAttributes, out pointer, out countBytes, out num3, out intPtr, out num4);
				byte[] sig = this._resolver.ReadEmbeddedBlob(pointer, countBytes);
				int num5 = 0;
				CorCallingConvention corCallingConvention = SignatureUtil.ExtractCallingConvention(sig, ref num5);
				this._customModifiers = SignatureUtil.ExtractCustomModifiers(sig, ref num5, this._resolver, this._context);
				bool flag = this._resolver.RawImport.IsValidToken((uint)this._declaringClassToken);
				if (flag)
				{
					Type type = this._resolver.ResolveType(this._declaringClassToken);
					bool flag2 = type.IsGenericType && (this._context == null || this._context.TypeArgs == null || this._context.TypeArgs.Length == 0);
					if (flag2)
					{
						bool flag3 = this._context == null;
						if (flag3)
						{
							this._context = new GenericContext(type.GetGenericArguments(), null);
						}
						else
						{
							this._context = new GenericContext(type.GetGenericArguments(), this._context.MethodArgs);
						}
					}
				}
				this._fieldType = SignatureUtil.ExtractType(sig, ref num5, this._resolver, this._context);
				this._initialized = true;
			}
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000584C File Offset: 0x00003A4C
		public override string ToString()
		{
			return MetadataOnlyCommonType.TypeSigToString(this.FieldType) + " " + this.Name;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000587C File Offset: 0x00003A7C
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private object ParseDefaultValue()
		{
			this.Initialize();
			IMetadataImport rawImport = this._resolver.RawImport;
			int num;
			int num2;
			FieldAttributes fieldAttributes;
			EmbeddedBlobPointer pointer;
			int countBytes;
			int num3;
			IntPtr intPtr;
			int len;
			rawImport.GetFieldProps(this._fieldDefToken, out num, null, 0, out num2, out fieldAttributes, out pointer, out countBytes, out num3, out intPtr, out len);
			byte[] sig = this._resolver.ReadEmbeddedBlob(pointer, countBytes);
			int num4 = 0;
			CorCallingConvention corCallingConvention = SignatureUtil.ExtractCallingConvention(sig, ref num4);
			CorElementType corElementType = SignatureUtil.ExtractElementType(sig, ref num4);
			bool flag = corElementType == CorElementType.ValueType;
			if (flag)
			{
				SignatureUtil.ExtractToken(sig, ref num4);
				corElementType = (CorElementType)num3;
			}
			else
			{
				bool flag2 = corElementType == CorElementType.GenericInstantiation;
				if (flag2)
				{
					Type type = SignatureUtil.ExtractType(sig, ref num4, this._resolver, this._context);
					corElementType = (CorElementType)num3;
				}
			}
			switch (corElementType)
			{
			case CorElementType.Bool:
			{
				byte b = Marshal.ReadByte(intPtr);
				bool flag3 = b == 0;
				if (flag3)
				{
					return false;
				}
				return true;
			}
			case CorElementType.Char:
				return (char)Marshal.ReadInt16(intPtr);
			case CorElementType.SByte:
				return (sbyte)Marshal.ReadByte(intPtr);
			case CorElementType.Byte:
				return Marshal.ReadByte(intPtr);
			case CorElementType.Short:
				return Marshal.ReadInt16(intPtr);
			case CorElementType.UShort:
				return (ushort)Marshal.ReadInt16(intPtr);
			case CorElementType.Int:
				return Marshal.ReadInt32(intPtr);
			case CorElementType.UInt:
				return (uint)Marshal.ReadInt32(intPtr);
			case CorElementType.Long:
				return Marshal.ReadInt64(intPtr);
			case CorElementType.ULong:
				return (ulong)Marshal.ReadInt64(intPtr);
			case CorElementType.Float:
			{
				float[] array = new float[1];
				Marshal.Copy(intPtr, array, 0, 1);
				return array[0];
			}
			case CorElementType.Double:
			{
				double[] array2 = new double[1];
				Marshal.Copy(intPtr, array2, 0, 1);
				return array2[0];
			}
			case CorElementType.String:
				return Marshal.PtrToStringAuto(intPtr, len);
			case CorElementType.Class:
				return null;
			case CorElementType.IntPtr:
				return Marshal.ReadIntPtr(intPtr);
			}
			throw new InvalidOperationException(Resources.IncorrectElementTypeValue);
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001F5 RID: 501 RVA: 0x00005AE9 File Offset: 0x00003CE9
		public override FieldAttributes Attributes { get; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x00005AF4 File Offset: 0x00003CF4
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Field;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x00005B08 File Offset: 0x00003D08
		public override string Name
		{
			get
			{
				this.InitializeName();
				return this._name;
			}
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000240B File Offset: 0x0000060B
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001FB RID: 507 RVA: 0x0000240B File Offset: 0x0000060B
		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00005B28 File Offset: 0x00003D28
		public override Type[] GetOptionalCustomModifiers()
		{
			this.Initialize();
			bool flag = this._customModifiers == null;
			Type[] result;
			if (flag)
			{
				result = Type.EmptyTypes;
			}
			else
			{
				result = this._customModifiers.OptionalCustomModifiers;
			}
			return result;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00005B64 File Offset: 0x00003D64
		public override Type[] GetRequiredCustomModifiers()
		{
			this.Initialize();
			bool flag = this._customModifiers == null;
			Type[] result;
			if (flag)
			{
				result = Type.EmptyTypes;
			}
			else
			{
				result = this._customModifiers.RequiredCustomModifiers;
			}
			return result;
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001FE RID: 510 RVA: 0x00005BA0 File Offset: 0x00003DA0
		public override Type FieldType
		{
			get
			{
				this.Initialize();
				return this._fieldType;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001FF RID: 511 RVA: 0x00005BC0 File Offset: 0x00003DC0
		public override Type DeclaringType
		{
			get
			{
				this.Initialize();
				return this._resolver.GetGenericType(new Token(this._declaringClassToken), this._context);
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000240B File Offset: 0x0000060B
		public override object GetValue(object obj)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00005BF8 File Offset: 0x00003DF8
		public virtual byte[] GetRvaField()
		{
			bool flag = (this.Attributes & FieldAttributes.HasFieldRVA) == FieldAttributes.PrivateScope;
			if (flag)
			{
				throw new InvalidOperationException(Resources.OperationValidOnRVAFieldsOnly);
			}
			StructLayoutAttribute structLayoutAttribute = this.FieldType.StructLayoutAttribute;
			bool flag2 = structLayoutAttribute.Value == LayoutKind.Auto;
			if (flag2)
			{
				throw new InvalidOperationException(Resources.OperationInvalidOnAutoLayoutFields);
			}
			uint num;
			uint num2;
			this._resolver.RawImport.GetRVA(this.MetadataToken, out num, out num2);
			int num3 = structLayoutAttribute.Size;
			bool flag3 = num3 == 0;
			if (flag3)
			{
				TypeCode typeCode = Type.GetTypeCode(this.FieldType);
				if (typeCode != TypeCode.Int32)
				{
					if (typeCode == TypeCode.Int64)
					{
						num3 = 8;
					}
				}
				else
				{
					num3 = 4;
				}
			}
			return this._resolver.RawMetadata.ReadRva((long)((ulong)num), num3);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00005CC0 File Offset: 0x00003EC0
		public override object GetRawConstantValue()
		{
			bool flag = !base.IsLiteral;
			if (flag)
			{
				throw new InvalidOperationException(Resources.OperationValidOnLiteralFieldsOnly);
			}
			return this.ParseDefaultValue();
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000203 RID: 515 RVA: 0x0000240B File Offset: 0x0000060B
		public override RuntimeFieldHandle FieldHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000240B File Offset: 0x0000060B
		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000205 RID: 517 RVA: 0x00005CF4 File Offset: 0x00003EF4
		public override int MetadataToken
		{
			get
			{
				return this._fieldDefToken;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000206 RID: 518 RVA: 0x00005D0C File Offset: 0x00003F0C
		public override Module Module
		{
			get
			{
				return this._resolver;
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00005D24 File Offset: 0x00003F24
		public override bool Equals(object obj)
		{
			MetadataOnlyFieldInfo metadataOnlyFieldInfo = obj as MetadataOnlyFieldInfo;
			bool flag = metadataOnlyFieldInfo != null;
			return flag && (metadataOnlyFieldInfo._resolver.Equals(this._resolver) && metadataOnlyFieldInfo._fieldDefToken.Equals(this._fieldDefToken)) && this.DeclaringType.Equals(metadataOnlyFieldInfo.DeclaringType);
		}

		// Token: 0x06000208 RID: 520 RVA: 0x00005D88 File Offset: 0x00003F88
		public override int GetHashCode()
		{
			return this._resolver.GetHashCode() * 32767 + this._fieldDefToken.GetHashCode();
		}

		// Token: 0x06000209 RID: 521 RVA: 0x00005DBC File Offset: 0x00003FBC
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this._resolver.GetCustomAttributeData(this.MetadataToken);
		}

		// Token: 0x0400006B RID: 107
		private readonly MetadataOnlyModule _resolver;

		// Token: 0x0400006C RID: 108
		private readonly int _fieldDefToken;

		// Token: 0x0400006D RID: 109
		private readonly int _declaringClassToken;

		// Token: 0x0400006E RID: 110
		private Type _fieldType;

		// Token: 0x0400006F RID: 111
		private GenericContext _context;

		// Token: 0x04000070 RID: 112
		private string _name;

		// Token: 0x04000071 RID: 113
		private readonly int _nameLength;

		// Token: 0x04000072 RID: 114
		private CustomModifiers _customModifiers;

		// Token: 0x04000073 RID: 115
		private bool _initialized;
	}
}
