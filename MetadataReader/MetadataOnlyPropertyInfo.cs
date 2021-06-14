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
	// Token: 0x0200002E RID: 46
	internal class MetadataOnlyPropertyInfo : PropertyInfo
	{
		// Token: 0x060002FD RID: 765 RVA: 0x0000A514 File Offset: 0x00008714
		public MetadataOnlyPropertyInfo(MetadataOnlyModule resolver, Token propToken, Type[] typeArgs, Type[] methodArgs)
		{
			this._resolver = resolver;
			this._propertyToken = propToken;
			this._context = new GenericContext(typeArgs, methodArgs);
			IMetadataImport rawImport = this._resolver.RawImport;
			PropertyAttributes propertyAttributes;
			EmbeddedBlobPointer pointer;
			int countBytes;
			int num;
			UnusedIntPtr unusedIntPtr;
			int num2;
			Token setterToken;
			Token getterToken;
			Token token;
			uint num3;
			rawImport.GetPropertyProps(this._propertyToken, out this._declaringClassToken, null, 0, out this._nameLength, out propertyAttributes, out pointer, out countBytes, out num, out unusedIntPtr, out num2, out setterToken, out getterToken, out token, 1U, out num3);
			this.Attributes = propertyAttributes;
			byte[] sig = this._resolver.ReadEmbeddedBlob(pointer, countBytes);
			int num4 = 0;
			CorCallingConvention corCallingConvention = SignatureUtil.ExtractCallingConvention(sig, ref num4);
			SignatureUtil.ExtractInt(sig, ref num4);
			this._propertyType = SignatureUtil.ExtractType(sig, ref num4, this._resolver, this._context);
			this._setterToken = setterToken;
			this._getterToken = getterToken;
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0000A5DC File Offset: 0x000087DC
		private void InitializeName()
		{
			bool flag = string.IsNullOrEmpty(this._name);
			if (flag)
			{
				IMetadataImport rawImport = this._resolver.RawImport;
				StringBuilder stringBuilder = StringBuilderPool.Get(this._nameLength);
				Token token;
				PropertyAttributes propertyAttributes;
				EmbeddedBlobPointer embeddedBlobPointer;
				int num;
				int num2;
				UnusedIntPtr unusedIntPtr;
				int num3;
				Token token2;
				Token token3;
				Token token4;
				uint num4;
				rawImport.GetPropertyProps(this._propertyToken, out token, stringBuilder, stringBuilder.Capacity, out this._nameLength, out propertyAttributes, out embeddedBlobPointer, out num, out num2, out unusedIntPtr, out num3, out token2, out token3, out token4, 1U, out num4);
				this._name = stringBuilder.ToString();
				StringBuilderPool.Release(ref stringBuilder);
			}
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0000A65C File Offset: 0x0000885C
		public override string ToString()
		{
			return this.DeclaringType + "." + this.Name;
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000300 RID: 768 RVA: 0x0000A684 File Offset: 0x00008884
		public override PropertyAttributes Attributes { get; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000301 RID: 769 RVA: 0x0000A68C File Offset: 0x0000888C
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Property;
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000302 RID: 770 RVA: 0x0000A6A0 File Offset: 0x000088A0
		public override string Name
		{
			get
			{
				this.InitializeName();
				return this._name;
			}
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0000240B File Offset: 0x0000060B
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000306 RID: 774 RVA: 0x0000240B File Offset: 0x0000060B
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000307 RID: 775 RVA: 0x0000A6C0 File Offset: 0x000088C0
		public override Type PropertyType
		{
			get
			{
				return this._propertyType;
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000308 RID: 776 RVA: 0x0000A6D8 File Offset: 0x000088D8
		public override Type DeclaringType
		{
			get
			{
				return this._resolver.GetGenericType(this._declaringClassToken, this._context);
			}
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0000232A File Offset: 0x0000052A
		public override object GetConstantValue()
		{
			throw new NotImplementedException();
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600030A RID: 778 RVA: 0x0000A704 File Offset: 0x00008904
		public override int MetadataToken
		{
			get
			{
				return this._propertyToken;
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600030B RID: 779 RVA: 0x0000A724 File Offset: 0x00008924
		public override bool CanRead
		{
			get
			{
				return !this._getterToken.IsNil;
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x0600030C RID: 780 RVA: 0x0000A748 File Offset: 0x00008948
		public override bool CanWrite
		{
			get
			{
				return !this._setterToken.IsNil;
			}
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0000A76C File Offset: 0x0000896C
		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			MethodInfo getMethod = this.GetGetMethod(nonPublic);
			bool flag = getMethod != null;
			if (flag)
			{
				list.Add(getMethod);
			}
			MethodInfo setMethod = this.GetSetMethod(nonPublic);
			bool flag2 = setMethod != null;
			if (flag2)
			{
				list.Add(setMethod);
			}
			return list.ToArray();
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0000A7C4 File Offset: 0x000089C4
		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			bool isNil = this._getterToken.IsNil;
			MethodInfo result;
			if (isNil)
			{
				result = null;
			}
			else
			{
				MethodInfo genericMethodInfo = this._resolver.GetGenericMethodInfo(this._getterToken, this._context);
				bool flag = nonPublic || genericMethodInfo.IsPublic;
				if (flag)
				{
					result = genericMethodInfo;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0000A820 File Offset: 0x00008A20
		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			bool isNil = this._setterToken.IsNil;
			MethodInfo result;
			if (isNil)
			{
				result = null;
			}
			else
			{
				MethodInfo genericMethodInfo = this._resolver.GetGenericMethodInfo(this._setterToken, this._context);
				bool flag = nonPublic || genericMethodInfo.IsPublic;
				if (flag)
				{
					result = genericMethodInfo;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0000A87C File Offset: 0x00008A7C
		public override ParameterInfo[] GetIndexParameters()
		{
			MethodInfo getMethod = this.GetGetMethod(true);
			bool flag = getMethod != null;
			ParameterInfo[] result;
			if (flag)
			{
				result = getMethod.GetParameters();
			}
			else
			{
				result = new ParameterInfo[0];
			}
			return result;
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0000240B File Offset: 0x0000060B
		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0000240B File Offset: 0x0000060B
		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000313 RID: 787 RVA: 0x0000A8B0 File Offset: 0x00008AB0
		public override Module Module
		{
			get
			{
				return this._resolver;
			}
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0000A8C8 File Offset: 0x00008AC8
		public override bool Equals(object obj)
		{
			MetadataOnlyPropertyInfo metadataOnlyPropertyInfo = obj as MetadataOnlyPropertyInfo;
			bool flag = metadataOnlyPropertyInfo != null;
			return flag && (metadataOnlyPropertyInfo._resolver.Equals(this._resolver) && metadataOnlyPropertyInfo._propertyToken.Equals(this._propertyToken)) && this.DeclaringType.Equals(metadataOnlyPropertyInfo.DeclaringType);
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0000A938 File Offset: 0x00008B38
		public override int GetHashCode()
		{
			return this._resolver.GetHashCode() * 32767 + this._propertyToken.GetHashCode();
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0000A970 File Offset: 0x00008B70
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this._resolver.GetCustomAttributeData(this.MetadataToken);
		}

		// Token: 0x040000A2 RID: 162
		private readonly MetadataOnlyModule _resolver;

		// Token: 0x040000A3 RID: 163
		private readonly Token _propertyToken;

		// Token: 0x040000A4 RID: 164
		private readonly Token _declaringClassToken;

		// Token: 0x040000A5 RID: 165
		private readonly Type _propertyType;

		// Token: 0x040000A6 RID: 166
		private readonly GenericContext _context;

		// Token: 0x040000A7 RID: 167
		private string _name;

		// Token: 0x040000A8 RID: 168
		private int _nameLength;

		// Token: 0x040000A9 RID: 169
		private readonly Token _setterToken;

		// Token: 0x040000AA RID: 170
		private readonly Token _getterToken;
	}
}
