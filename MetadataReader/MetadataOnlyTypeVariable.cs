using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000034 RID: 52
	internal class MetadataOnlyTypeVariable : MetadataOnlyCommonType
	{
		// Token: 0x060003E6 RID: 998 RVA: 0x0000CCCC File Offset: 0x0000AECC
		internal MetadataOnlyTypeVariable(MetadataOnlyModule resolver, Token token)
		{
			this._token = token.Value;
			this._resolver = resolver;
			this._resolver.GetGenericParameterProps(this._token, out this._ownerTypeToken, out this._ownerMethodToken, out this._name, out this._gpAttributes, out this._position);
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x060003E7 RID: 999 RVA: 0x0000CD28 File Offset: 0x0000AF28
		public override string FullName
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x060003E8 RID: 1000 RVA: 0x0000CD3C File Offset: 0x0000AF3C
		internal override MetadataOnlyModule Resolver
		{
			get
			{
				return this._resolver;
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x060003E9 RID: 1001 RVA: 0x0000CD54 File Offset: 0x0000AF54
		public override Type BaseType
		{
			get
			{
				Type[] genericParameterConstraints = this.GetGenericParameterConstraints();
				foreach (Type type in genericParameterConstraints)
				{
					bool isClass = type.IsClass;
					if (isClass)
					{
						return type;
					}
				}
				return this._resolver.AssemblyResolver.GetBuiltInType(CorElementType.Object);
			}
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0000CDAC File Offset: 0x0000AFAC
		public override bool Equals(Type txOther)
		{
			bool flag = txOther is MetadataOnlyTypeVariableRef;
			bool result;
			if (flag)
			{
				result = (this._ownerMethodToken != 0 && (ulong)this._position == (ulong)((long)txOther.GenericParameterPosition));
			}
			else
			{
				MetadataOnlyTypeVariable metadataOnlyTypeVariable = txOther as MetadataOnlyTypeVariable;
				bool flag2 = metadataOnlyTypeVariable == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = this.Name != metadataOnlyTypeVariable.Name;
					result = (!flag3 && (this._ownerTypeToken == metadataOnlyTypeVariable._ownerTypeToken && this._ownerMethodToken == metadataOnlyTypeVariable._ownerMethodToken) && this.Module.Equals(metadataOnlyTypeVariable.Module));
				}
			}
			return result;
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x0000CE4C File Offset: 0x0000B04C
		public override bool IsAssignableFrom(Type c)
		{
			return MetadataOnlyTypeDef.IsAssignableFromHelper(this, c);
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x060003EC RID: 1004 RVA: 0x0000CE68 File Offset: 0x0000B068
		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x0000CE7C File Offset: 0x0000B07C
		public override Type GetElementType()
		{
			return null;
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x060003EE RID: 1006 RVA: 0x0000CE90 File Offset: 0x0000B090
		public override int MetadataToken
		{
			get
			{
				return this._token;
			}
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x0000CEA8 File Offset: 0x0000B0A8
		public override MethodInfo[] GetMethods(BindingFlags flags)
		{
			return new MethodInfo[0];
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x0000CEC0 File Offset: 0x0000B0C0
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			return new ConstructorInfo[0];
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x0000CED8 File Offset: 0x0000B0D8
		public override FieldInfo[] GetFields(BindingFlags flags)
		{
			return new FieldInfo[0];
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0000CEF0 File Offset: 0x0000B0F0
		public override FieldInfo GetField(string name, BindingFlags flags)
		{
			return null;
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x0000CF04 File Offset: 0x0000B104
		public override PropertyInfo[] GetProperties(BindingFlags flags)
		{
			return new PropertyInfo[0];
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0000CF1C File Offset: 0x0000B11C
		public override EventInfo[] GetEvents(BindingFlags flags)
		{
			return new EventInfo[0];
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0000CF34 File Offset: 0x0000B134
		public override EventInfo GetEvent(string name, BindingFlags flags)
		{
			return null;
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00002390 File Offset: 0x00000590
		public override Type MakeGenericType(params Type[] argTypes)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0000CF48 File Offset: 0x0000B148
		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			return null;
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x0000CF5C File Offset: 0x0000B15C
		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			return Type.EmptyTypes;
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0000CF74 File Offset: 0x0000B174
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return TypeAttributes.Public;
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x0000CF88 File Offset: 0x0000B188
		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return null;
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0000CF9C File Offset: 0x0000B19C
		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return null;
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x060003FC RID: 1020 RVA: 0x0000CFB0 File Offset: 0x0000B1B0
		public override bool IsGenericParameter
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x0000CFC4 File Offset: 0x0000B1C4
		public override Type[] GetGenericArguments()
		{
			return Type.EmptyTypes;
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x0000CFDC File Offset: 0x0000B1DC
		public override Type[] GetGenericParameterConstraints()
		{
			List<Type> list = new List<Type>(this._resolver.GetConstraintTypes(this._token));
			return list.ToArray();
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x00002390 File Offset: 0x00000590
		public override Type GetGenericTypeDefinition()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x0000D00C File Offset: 0x0000B20C
		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return null;
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x0000D020 File Offset: 0x0000B220
		public override Type[] GetInterfaces()
		{
			return MetadataOnlyTypeDef.GetAllInterfacesHelper(this);
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0000D038 File Offset: 0x0000B238
		public override Type GetInterface(string name, bool ignoreCase)
		{
			return MetadataOnlyModule.GetInterfaceHelper(this.GetInterfaces(), name, ignoreCase);
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x0000D058 File Offset: 0x0000B258
		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			return MetadataOnlyTypeDef.GetMembersHelper(this, bindingAttr);
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000404 RID: 1028 RVA: 0x0000D074 File Offset: 0x0000B274
		public override Guid GUID
		{
			get
			{
				return Guid.Empty;
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x0000D08C File Offset: 0x0000B28C
		protected override bool HasElementTypeImpl()
		{
			return false;
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x0000240B File Offset: 0x0000060B
		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x0000D0A0 File Offset: 0x0000B2A0
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.Resolver.GetCustomAttributeData(this.MetadataToken);
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000408 RID: 1032 RVA: 0x0000D0C4 File Offset: 0x0000B2C4
		public override GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				return this._gpAttributes;
			}
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000409 RID: 1033 RVA: 0x0000D0DC File Offset: 0x0000B2DC
		public override int GenericParameterPosition
		{
			get
			{
				return (int)this._position;
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x0600040A RID: 1034 RVA: 0x0000D0F4 File Offset: 0x0000B2F4
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.TypeInfo;
			}
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x0600040B RID: 1035 RVA: 0x0000D108 File Offset: 0x0000B308
		public override Type DeclaringType
		{
			get
			{
				bool flag = this._ownerTypeToken != 0;
				Type result;
				if (flag)
				{
					result = this._resolver.ResolveType(this._ownerTypeToken);
				}
				else
				{
					bool flag2 = this.DeclaringMethod != null;
					if (flag2)
					{
						result = this.DeclaringMethod.DeclaringType;
					}
					else
					{
						result = null;
					}
				}
				return result;
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x0600040C RID: 1036 RVA: 0x0000D15C File Offset: 0x0000B35C
		public override MethodBase DeclaringMethod
		{
			get
			{
				bool flag = this._ownerMethodToken != 0;
				MethodBase result;
				if (flag)
				{
					result = this._resolver.ResolveMethod(this._ownerMethodToken);
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x0600040D RID: 1037 RVA: 0x0000D194 File Offset: 0x0000B394
		public override string Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x0000D1AC File Offset: 0x0000B3AC
		public override string Namespace
		{
			get
			{
				bool flag = this.DeclaringType != null;
				string result;
				if (flag)
				{
					result = this.DeclaringType.Namespace;
				}
				else
				{
					bool flag2 = this.DeclaringMethod != null;
					if (flag2)
					{
						result = this.DeclaringMethod.DeclaringType.Namespace;
					}
					else
					{
						result = null;
					}
				}
				return result;
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x0600040F RID: 1039 RVA: 0x0000D1FC File Offset: 0x0000B3FC
		public override Assembly Assembly
		{
			get
			{
				return this._resolver.Assembly;
			}
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x0000240B File Offset: 0x0000060B
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000413 RID: 1043 RVA: 0x0000240B File Offset: 0x0000060B
		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x0000D21C File Offset: 0x0000B41C
		public override string ToString()
		{
			return this.Name;
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x0000D234 File Offset: 0x0000B434
		protected override TypeCode GetTypeCodeImpl()
		{
			return TypeCode.Object;
		}

		// Token: 0x040000BE RID: 190
		private readonly int _ownerMethodToken;

		// Token: 0x040000BF RID: 191
		private readonly int _ownerTypeToken;

		// Token: 0x040000C0 RID: 192
		private readonly string _name;

		// Token: 0x040000C1 RID: 193
		private readonly uint _position;

		// Token: 0x040000C2 RID: 194
		private readonly MetadataOnlyModule _resolver;

		// Token: 0x040000C3 RID: 195
		private readonly int _token;

		// Token: 0x040000C4 RID: 196
		private readonly GenericParameterAttributes _gpAttributes;
	}
}
