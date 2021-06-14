using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Runtime.InteropServices;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000033 RID: 51
	[DebuggerDisplay("TypeProxy")]
	internal abstract class TypeProxy : MetadataOnlyCommonType, ITypeProxy
	{
		// Token: 0x06000394 RID: 916 RVA: 0x0000C1D9 File Offset: 0x0000A3D9
		protected TypeProxy(MetadataOnlyModule resolver)
		{
			this.m_resolver = resolver;
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000395 RID: 917 RVA: 0x0000C1EC File Offset: 0x0000A3EC
		internal override MetadataOnlyModule Resolver
		{
			get
			{
				return this.m_resolver;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000396 RID: 918 RVA: 0x0000C204 File Offset: 0x0000A404
		public ITypeUniverse TypeUniverse
		{
			get
			{
				return this.m_resolver.AssemblyResolver;
			}
		}

		// Token: 0x06000397 RID: 919 RVA: 0x0000C224 File Offset: 0x0000A424
		public virtual Type GetResolvedType()
		{
			bool flag = this._cachedResolvedType == null;
			if (flag)
			{
				this._cachedResolvedType = this.GetResolvedTypeWorker();
			}
			return this._cachedResolvedType;
		}

		// Token: 0x06000398 RID: 920
		protected abstract Type GetResolvedTypeWorker();

		// Token: 0x06000399 RID: 921 RVA: 0x0000C258 File Offset: 0x0000A458
		public override string ToString()
		{
			return this.GetResolvedType().ToString();
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x0600039A RID: 922 RVA: 0x0000C278 File Offset: 0x0000A478
		public override string FullName
		{
			get
			{
				return this.GetResolvedType().FullName;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x0600039B RID: 923 RVA: 0x0000C298 File Offset: 0x0000A498
		public override string Namespace
		{
			get
			{
				return this.GetResolvedType().Namespace;
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x0600039C RID: 924 RVA: 0x0000C2B8 File Offset: 0x0000A4B8
		public override string Name
		{
			get
			{
				return this.GetResolvedType().Name;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x0600039D RID: 925 RVA: 0x0000C2D8 File Offset: 0x0000A4D8
		public override string AssemblyQualifiedName
		{
			get
			{
				return this.GetResolvedType().AssemblyQualifiedName;
			}
		}

		// Token: 0x0600039E RID: 926 RVA: 0x0000C2F8 File Offset: 0x0000A4F8
		public override int GetHashCode()
		{
			return this.GetResolvedType().GetHashCode();
		}

		// Token: 0x0600039F RID: 927 RVA: 0x0000C318 File Offset: 0x0000A518
		public override bool Equals(object objOther)
		{
			Type type = objOther as Type;
			bool flag = type == null;
			return !flag && this.Equals(type);
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x0000C348 File Offset: 0x0000A548
		public override bool Equals(Type t)
		{
			bool flag = t == null;
			return !flag && this.GetResolvedType().Equals(t);
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x0000C374 File Offset: 0x0000A574
		public override Type MakeByRefType()
		{
			return this.Resolver.Factory.CreateByRefType(this);
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0000C39C File Offset: 0x0000A59C
		public override Type MakePointerType()
		{
			return this.Resolver.Factory.CreatePointerType(this);
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x0000C3C0 File Offset: 0x0000A5C0
		public override int GetArrayRank()
		{
			return this.GetResolvedType().GetArrayRank();
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x0000C3E0 File Offset: 0x0000A5E0
		public override Type MakeGenericType(params Type[] args)
		{
			return new ProxyGenericType(this, args);
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x0000C3FC File Offset: 0x0000A5FC
		public override Type MakeArrayType()
		{
			return this.Resolver.Factory.CreateVectorType(this);
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x0000C420 File Offset: 0x0000A620
		public override Type MakeArrayType(int rank)
		{
			return this.Resolver.Factory.CreateArrayType(this, rank);
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060003A7 RID: 935 RVA: 0x0000C444 File Offset: 0x0000A644
		public override Module Module
		{
			get
			{
				return this.GetResolvedType().Module;
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x0000C464 File Offset: 0x0000A664
		public override Type BaseType
		{
			get
			{
				return this.GetResolvedType().BaseType;
			}
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x0000C484 File Offset: 0x0000A684
		protected override bool IsArrayImpl()
		{
			return this.GetResolvedType().IsArray;
		}

		// Token: 0x060003AA RID: 938 RVA: 0x0000C4A4 File Offset: 0x0000A6A4
		protected override bool IsByRefImpl()
		{
			return this.GetResolvedType().IsByRef;
		}

		// Token: 0x060003AB RID: 939 RVA: 0x0000C4C4 File Offset: 0x0000A6C4
		protected override bool IsPointerImpl()
		{
			return this.GetResolvedType().IsPointer;
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060003AC RID: 940 RVA: 0x0000C4E4 File Offset: 0x0000A6E4
		public override bool IsEnum
		{
			get
			{
				return this.GetResolvedType().IsEnum;
			}
		}

		// Token: 0x060003AD RID: 941 RVA: 0x0000C504 File Offset: 0x0000A704
		protected override bool IsValueTypeImpl()
		{
			return this.GetResolvedType().IsValueType;
		}

		// Token: 0x060003AE RID: 942 RVA: 0x0000C524 File Offset: 0x0000A724
		protected override bool IsPrimitiveImpl()
		{
			return this.GetResolvedType().IsPrimitive;
		}

		// Token: 0x060003AF RID: 943 RVA: 0x0000C544 File Offset: 0x0000A744
		public override Type GetElementType()
		{
			return this.GetResolvedType().GetElementType();
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060003B0 RID: 944 RVA: 0x0000C564 File Offset: 0x0000A764
		public override int MetadataToken
		{
			get
			{
				return this.GetResolvedType().MetadataToken;
			}
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0000C584 File Offset: 0x0000A784
		public override Type[] GetGenericArguments()
		{
			return this.GetResolvedType().GetGenericArguments();
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0000C5A4 File Offset: 0x0000A7A4
		public override Type GetGenericTypeDefinition()
		{
			return this.GetResolvedType().GetGenericTypeDefinition();
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x060003B3 RID: 947 RVA: 0x0000C5C4 File Offset: 0x0000A7C4
		public override bool ContainsGenericParameters
		{
			get
			{
				return this.GetResolvedType().ContainsGenericParameters;
			}
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x0000C5E4 File Offset: 0x0000A7E4
		public override MethodInfo[] GetMethods(BindingFlags flags)
		{
			return this.GetResolvedType().GetMethods(flags);
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0000C604 File Offset: 0x0000A804
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			return this.GetResolvedType().GetConstructors(bindingAttr);
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0000C624 File Offset: 0x0000A824
		public override bool IsAssignableFrom(Type c)
		{
			return this.GetResolvedType().IsAssignableFrom(c);
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x0000C644 File Offset: 0x0000A844
		protected override bool IsContextfulImpl()
		{
			return this.GetResolvedType().IsContextful;
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0000C664 File Offset: 0x0000A864
		protected override bool IsMarshalByRefImpl()
		{
			return this.GetResolvedType().IsMarshalByRef;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x0000C684 File Offset: 0x0000A884
		public override bool IsSubclassOf(Type c)
		{
			return this.GetResolvedType().IsSubclassOf(c);
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060003BA RID: 954 RVA: 0x0000C6A4 File Offset: 0x0000A8A4
		public override Type UnderlyingSystemType
		{
			get
			{
				return this.GetResolvedType().UnderlyingSystemType;
			}
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0000C6C4 File Offset: 0x0000A8C4
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return this.GetResolvedType().Attributes;
		}

		// Token: 0x060003BC RID: 956 RVA: 0x0000C6E4 File Offset: 0x0000A8E4
		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			bool flag = types == null && modifiers == null && binder == null && callConvention == CallingConventions.Any;
			MethodInfo method;
			if (flag)
			{
				method = this.GetResolvedType().GetMethod(name, bindingAttr);
			}
			else
			{
				method = this.GetResolvedType().GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
			}
			return method;
		}

		// Token: 0x060003BD RID: 957 RVA: 0x0000C734 File Offset: 0x0000A934
		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			bool flag = types == null && modifiers == null;
			PropertyInfo property;
			if (flag)
			{
				bool flag2 = returnType != null;
				if (flag2)
				{
					property = this.GetResolvedType().GetProperty(name, returnType);
				}
				else
				{
					property = this.GetResolvedType().GetProperty(name, bindingAttr);
				}
			}
			else
			{
				property = this.GetResolvedType().GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
			}
			return property;
		}

		// Token: 0x060003BE RID: 958 RVA: 0x0000C798 File Offset: 0x0000A998
		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetResolvedType().GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
		}

		// Token: 0x060003BF RID: 959 RVA: 0x0000C7BC File Offset: 0x0000A9BC
		public override FieldInfo[] GetFields(BindingFlags flags)
		{
			return this.GetResolvedType().GetFields(flags);
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0000C7DC File Offset: 0x0000A9DC
		public override PropertyInfo[] GetProperties(BindingFlags flags)
		{
			return this.GetResolvedType().GetProperties(flags);
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0000C7FC File Offset: 0x0000A9FC
		public override EventInfo[] GetEvents(BindingFlags flags)
		{
			return this.GetResolvedType().GetEvents(flags);
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x0000C81C File Offset: 0x0000AA1C
		public override EventInfo GetEvent(string name, BindingFlags flags)
		{
			return this.GetResolvedType().GetEvent(name, flags);
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x0000C83C File Offset: 0x0000AA3C
		public override FieldInfo GetField(string name, BindingFlags flags)
		{
			return this.GetResolvedType().GetField(name, flags);
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x0000C85C File Offset: 0x0000AA5C
		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			return this.GetResolvedType().GetNestedTypes(bindingAttr);
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x0000C87C File Offset: 0x0000AA7C
		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			return this.GetResolvedType().GetNestedType(name, bindingAttr);
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0000C89C File Offset: 0x0000AA9C
		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			return this.GetResolvedType().GetMember(name, type, bindingAttr);
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0000C8BC File Offset: 0x0000AABC
		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			return this.GetResolvedType().GetMembers(bindingAttr);
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0000C8DC File Offset: 0x0000AADC
		public override bool IsInstanceOfType(object o)
		{
			return this.GetResolvedType().IsInstanceOfType(o);
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x0000C8FC File Offset: 0x0000AAFC
		public override Type[] GetInterfaces()
		{
			return this.GetResolvedType().GetInterfaces();
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0000C91C File Offset: 0x0000AB1C
		public override Type GetInterface(string name, bool ignoreCase)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetResolvedType().GetInterface(name, ignoreCase);
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060003CB RID: 971 RVA: 0x0000C950 File Offset: 0x0000AB50
		public override bool IsGenericParameter
		{
			get
			{
				return this.GetResolvedType().IsGenericParameter;
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x060003CC RID: 972 RVA: 0x0000C970 File Offset: 0x0000AB70
		public override bool IsGenericType
		{
			get
			{
				return this.GetResolvedType().IsGenericType;
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x060003CD RID: 973 RVA: 0x0000C990 File Offset: 0x0000AB90
		public override bool IsGenericTypeDefinition
		{
			get
			{
				return this.GetResolvedType().IsGenericTypeDefinition;
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x060003CE RID: 974 RVA: 0x0000C9B0 File Offset: 0x0000ABB0
		public override Guid GUID
		{
			get
			{
				return this.GetResolvedType().GUID;
			}
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0000C9D0 File Offset: 0x0000ABD0
		protected override bool HasElementTypeImpl()
		{
			return base.IsArray || base.IsByRef || base.IsPointer;
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0000C9FC File Offset: 0x0000ABFC
		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			return this.GetResolvedType().InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x0000CA28 File Offset: 0x0000AC28
		protected override bool IsCOMObjectImpl()
		{
			return this.GetResolvedType().IsCOMObject;
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0000CA48 File Offset: 0x0000AC48
		public override MemberInfo[] GetDefaultMembers()
		{
			return this.GetResolvedType().GetDefaultMembers();
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0000CA68 File Offset: 0x0000AC68
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.GetResolvedType().GetCustomAttributesData();
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x060003D4 RID: 980 RVA: 0x0000CA88 File Offset: 0x0000AC88
		public override StructLayoutAttribute StructLayoutAttribute
		{
			get
			{
				return this.GetResolvedType().StructLayoutAttribute;
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x060003D5 RID: 981 RVA: 0x0000CAA8 File Offset: 0x0000ACA8
		public override GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				return this.GetResolvedType().GenericParameterAttributes;
			}
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x060003D6 RID: 982 RVA: 0x0000CAC8 File Offset: 0x0000ACC8
		public override MethodBase DeclaringMethod
		{
			get
			{
				return this.GetResolvedType().DeclaringMethod;
			}
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x060003D7 RID: 983 RVA: 0x0000CAE8 File Offset: 0x0000ACE8
		public override int GenericParameterPosition
		{
			get
			{
				return this.GetResolvedType().GenericParameterPosition;
			}
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x0000CB08 File Offset: 0x0000AD08
		public override Type[] GetGenericParameterConstraints()
		{
			return this.GetResolvedType().GetGenericParameterConstraints();
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x0000CB28 File Offset: 0x0000AD28
		public override InterfaceMapping GetInterfaceMap(Type interfaceType)
		{
			return this.GetResolvedType().GetInterfaceMap(interfaceType);
		}

		// Token: 0x060003DA RID: 986 RVA: 0x0000CB48 File Offset: 0x0000AD48
		public override Type[] FindInterfaces(TypeFilter filter, object filterCriteria)
		{
			return this.GetResolvedType().FindInterfaces(filter, filterCriteria);
		}

		// Token: 0x060003DB RID: 987 RVA: 0x0000CB68 File Offset: 0x0000AD68
		public override MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
		{
			return this.GetResolvedType().GetMember(name, bindingAttr);
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x060003DC RID: 988 RVA: 0x0000CB88 File Offset: 0x0000AD88
		public override bool IsSerializable
		{
			get
			{
				return this.GetResolvedType().IsSerializable;
			}
		}

		// Token: 0x060003DD RID: 989 RVA: 0x0000CBA8 File Offset: 0x0000ADA8
		public override MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
		{
			return this.GetResolvedType().FindMembers(memberType, bindingAttr, filter, filterCriteria);
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x060003DE RID: 990 RVA: 0x0000CBCC File Offset: 0x0000ADCC
		public override MemberTypes MemberType
		{
			get
			{
				return this.GetResolvedType().MemberType;
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x060003DF RID: 991 RVA: 0x0000CBEC File Offset: 0x0000ADEC
		public override Type DeclaringType
		{
			get
			{
				return this.GetResolvedType().DeclaringType;
			}
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x060003E0 RID: 992 RVA: 0x0000CC0C File Offset: 0x0000AE0C
		public override Assembly Assembly
		{
			get
			{
				return this.GetResolvedType().Assembly;
			}
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x0000CC2C File Offset: 0x0000AE2C
		public override object[] GetCustomAttributes(bool inherit)
		{
			return this.GetResolvedType().GetCustomAttributes(inherit);
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x0000CC4C File Offset: 0x0000AE4C
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return this.GetResolvedType().GetCustomAttributes(attributeType, inherit);
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x0000CC6C File Offset: 0x0000AE6C
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return this.GetResolvedType().IsDefined(attributeType, inherit);
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x060003E4 RID: 996 RVA: 0x0000CC8C File Offset: 0x0000AE8C
		public override Type ReflectedType
		{
			get
			{
				return this.GetResolvedType().ReflectedType;
			}
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x0000CCAC File Offset: 0x0000AEAC
		protected override TypeCode GetTypeCodeImpl()
		{
			return Type.GetTypeCode(this.GetResolvedType());
		}

		// Token: 0x040000BC RID: 188
		protected readonly MetadataOnlyModule m_resolver;

		// Token: 0x040000BD RID: 189
		private Type _cachedResolvedType;
	}
}
