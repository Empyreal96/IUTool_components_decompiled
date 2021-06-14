using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection.Mock
{
	// Token: 0x02000004 RID: 4
	[ComVisible(true)]
	internal abstract class Type : MemberInfo
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000028 RID: 40
		public abstract string FullName { get; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000029 RID: 41
		public abstract string Namespace { get; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002A RID: 42
		public abstract string AssemblyQualifiedName { get; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002B RID: 43
		public abstract Assembly Assembly { get; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002C RID: 44
		public abstract override Module Module { get; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002D RID: 45
		public abstract Type BaseType { get; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002E RID: 46 RVA: 0x000021B4 File Offset: 0x000003B4
		public bool IsClass
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.ClassSemanticsMask) == TypeAttributes.NotPublic && !this.IsValueType;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600002F RID: 47 RVA: 0x000021E0 File Offset: 0x000003E0
		public bool IsInterface
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.ClassSemanticsMask) == TypeAttributes.ClassSemanticsMask;
			}
		}

		// Token: 0x06000030 RID: 48
		public abstract override bool Equals(object objOther);

		// Token: 0x06000031 RID: 49 RVA: 0x00002200 File Offset: 0x00000400
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06000032 RID: 50
		public abstract bool Equals(Type o);

		// Token: 0x06000033 RID: 51
		public abstract Type MakeArrayType();

		// Token: 0x06000034 RID: 52
		public abstract Type MakeArrayType(int rank);

		// Token: 0x06000035 RID: 53
		public abstract int GetArrayRank();

		// Token: 0x06000036 RID: 54
		public abstract Type MakeByRefType();

		// Token: 0x06000037 RID: 55
		public abstract Type MakePointerType();

		// Token: 0x06000038 RID: 56
		protected abstract bool IsArrayImpl();

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002218 File Offset: 0x00000418
		public bool IsArray
		{
			get
			{
				return this.IsArrayImpl();
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002230 File Offset: 0x00000430
		public bool IsByRef
		{
			get
			{
				return this.IsByRefImpl();
			}
		}

		// Token: 0x0600003B RID: 59
		protected abstract bool IsByRefImpl();

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00002248 File Offset: 0x00000448
		public virtual bool IsEnum
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600003D RID: 61 RVA: 0x0000225C File Offset: 0x0000045C
		public bool IsPointer
		{
			get
			{
				return this.IsPointerImpl();
			}
		}

		// Token: 0x0600003E RID: 62
		protected abstract bool IsPointerImpl();

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002274 File Offset: 0x00000474
		public bool IsValueType
		{
			get
			{
				return this.IsValueTypeImpl();
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x0000228C File Offset: 0x0000048C
		protected virtual bool IsValueTypeImpl()
		{
			return false;
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002087 File Offset: 0x00000287
		public virtual bool IsSerializable
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000042 RID: 66 RVA: 0x000022A0 File Offset: 0x000004A0
		public bool IsNested
		{
			get
			{
				return this.DeclaringType != null;
			}
		}

		// Token: 0x06000043 RID: 67
		public abstract Type GetElementType();

		// Token: 0x06000044 RID: 68
		public abstract Type[] GetGenericArguments();

		// Token: 0x06000045 RID: 69 RVA: 0x0000210A File Offset: 0x0000030A
		public virtual Type[] GetGenericParameterConstraints()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000020E5 File Offset: 0x000002E5
		public virtual Type GetGenericTypeDefinition()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000047 RID: 71 RVA: 0x000022BC File Offset: 0x000004BC
		public virtual bool IsGenericTypeDefinition
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000048 RID: 72 RVA: 0x000022D0 File Offset: 0x000004D0
		public virtual MethodBase DeclaringMethod
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06000049 RID: 73
		public abstract MethodInfo[] GetMethods(BindingFlags flags);

		// Token: 0x0600004A RID: 74 RVA: 0x000022E4 File Offset: 0x000004E4
		public MethodInfo[] GetMethods()
		{
			return this.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002300 File Offset: 0x00000500
		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = types == null;
			if (flag2)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				bool flag3 = types[i] == null;
				if (flag3)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002378 File Offset: 0x00000578
		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = types == null;
			if (flag2)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				bool flag3 = types[i] == null;
				if (flag3)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetMethodImpl(name, bindingAttr, binder, CallingConventions.Any, types, modifiers);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000023EC File Offset: 0x000005EC
		public MethodInfo GetMethod(string name, Type[] types, ParameterModifier[] modifiers)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = types == null;
			if (flag2)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				bool flag3 = types[i] == null;
				if (flag3)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, types, modifiers);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000245C File Offset: 0x0000065C
		public MethodInfo GetMethod(string name, Type[] types)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = types == null;
			if (flag2)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				bool flag3 = types[i] == null;
				if (flag3)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, types, null);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000024CC File Offset: 0x000006CC
		public MethodInfo GetMethod(string name, BindingFlags bindingAttr)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetMethodImpl(name, bindingAttr, null, CallingConventions.Any, null, null);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002500 File Offset: 0x00000700
		public MethodInfo GetMethod(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, null, null);
		}

		// Token: 0x06000051 RID: 81
		protected abstract MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);

		// Token: 0x06000052 RID: 82
		public abstract FieldInfo[] GetFields(BindingFlags flags);

		// Token: 0x06000053 RID: 83 RVA: 0x00002534 File Offset: 0x00000734
		public FieldInfo[] GetFields()
		{
			return this.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x06000054 RID: 84
		public abstract FieldInfo GetField(string name, BindingFlags bindingAttr);

		// Token: 0x06000055 RID: 85 RVA: 0x00002550 File Offset: 0x00000750
		public FieldInfo GetField(string name)
		{
			return this.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x06000056 RID: 86
		public abstract PropertyInfo[] GetProperties(BindingFlags flags);

		// Token: 0x06000057 RID: 87 RVA: 0x0000256C File Offset: 0x0000076C
		public PropertyInfo[] GetProperties()
		{
			return this.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002588 File Offset: 0x00000788
		public PropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = types == null;
			if (flag2)
			{
				throw new ArgumentNullException("types");
			}
			return this.GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000025D0 File Offset: 0x000007D0
		public PropertyInfo GetProperty(string name, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = types == null;
			if (flag2)
			{
				throw new ArgumentNullException("types");
			}
			return this.GetPropertyImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, types, modifiers);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002618 File Offset: 0x00000818
		public PropertyInfo GetProperty(string name, BindingFlags bindingAttr)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetPropertyImpl(name, bindingAttr, null, null, null, null);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000264C File Offset: 0x0000084C
		public PropertyInfo GetProperty(string name, Type returnType, Type[] types)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = types == null;
			if (flag2)
			{
				throw new ArgumentNullException("types");
			}
			return this.GetPropertyImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, types, null);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002694 File Offset: 0x00000894
		public PropertyInfo GetProperty(string name, Type[] types)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = types == null;
			if (flag2)
			{
				throw new ArgumentNullException("types");
			}
			return this.GetPropertyImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, null, types, null);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000026DC File Offset: 0x000008DC
		public PropertyInfo GetProperty(string name, Type returnType)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = returnType == null;
			if (flag2)
			{
				throw new ArgumentNullException("returnType");
			}
			return this.GetPropertyImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, null, null);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00002724 File Offset: 0x00000924
		public PropertyInfo GetProperty(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetPropertyImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, null, null, null);
		}

		// Token: 0x0600005F RID: 95
		protected abstract PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers);

		// Token: 0x06000060 RID: 96
		public abstract EventInfo[] GetEvents(BindingFlags flags);

		// Token: 0x06000061 RID: 97 RVA: 0x00002758 File Offset: 0x00000958
		public EventInfo[] GetEvents()
		{
			return this.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x06000062 RID: 98
		public abstract EventInfo GetEvent(string name, BindingFlags flags);

		// Token: 0x06000063 RID: 99 RVA: 0x00002774 File Offset: 0x00000974
		public EventInfo GetEvent(string name)
		{
			return this.GetEvent(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000064 RID: 100
		public abstract Type UnderlyingSystemType { get; }

		// Token: 0x06000065 RID: 101
		public abstract bool IsAssignableFrom(Type c);

		// Token: 0x06000066 RID: 102
		public abstract bool IsInstanceOfType(object o);

		// Token: 0x06000067 RID: 103
		public abstract Type MakeGenericType(params Type[] argTypes);

		// Token: 0x06000068 RID: 104 RVA: 0x00002790 File Offset: 0x00000990
		public Type[] GetNestedTypes()
		{
			return this.GetNestedTypes(BindingFlags.Public);
		}

		// Token: 0x06000069 RID: 105
		public abstract Type[] GetNestedTypes(BindingFlags bindingAttr);

		// Token: 0x0600006A RID: 106
		public abstract Type GetNestedType(string name, BindingFlags bindingAttr);

		// Token: 0x0600006B RID: 107 RVA: 0x000027AC File Offset: 0x000009AC
		public Type GetNestedType(string name)
		{
			return this.GetNestedType(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x0600006C RID: 108
		public abstract ConstructorInfo[] GetConstructors(BindingFlags bindingAttr);

		// Token: 0x0600006D RID: 109 RVA: 0x000027C8 File Offset: 0x000009C8
		public ConstructorInfo[] GetConstructors()
		{
			return this.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000027E4 File Offset: 0x000009E4
		public Type GetInterface(string name)
		{
			return this.GetInterface(name, false);
		}

		// Token: 0x0600006F RID: 111
		public abstract Type GetInterface(string name, bool ignoreCase);

		// Token: 0x06000070 RID: 112
		public abstract Type[] GetInterfaces();

		// Token: 0x06000071 RID: 113 RVA: 0x000027FE File Offset: 0x000009FE
		public virtual InterfaceMapping GetInterfaceMap(Type interfaceType)
		{
			throw new NotSupportedException("NotSupported_SubclassOverride");
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000280C File Offset: 0x00000A0C
		public virtual Type[] FindInterfaces(TypeFilter filter, object filterCriteria)
		{
			bool flag = filter == null;
			if (flag)
			{
				throw new ArgumentNullException("filter");
			}
			Type[] interfaces = this.GetInterfaces();
			int num = 0;
			for (int i = 0; i < interfaces.Length; i++)
			{
				bool flag2 = !filter(interfaces[i], filterCriteria);
				if (flag2)
				{
					interfaces[i] = null;
				}
				else
				{
					num++;
				}
			}
			bool flag3 = num == interfaces.Length;
			Type[] result;
			if (flag3)
			{
				result = interfaces;
			}
			else
			{
				Type[] array = new Type[num];
				num = 0;
				for (int j = 0; j < interfaces.Length; j++)
				{
					bool flag4 = interfaces[j] != null;
					if (flag4)
					{
						array[num++] = interfaces[j];
					}
				}
				result = array;
			}
			return result;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000028C4 File Offset: 0x00000AC4
		public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			bool flag = types == null;
			if (flag)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				bool flag2 = types[i] == null;
				if (flag2)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetConstructorImpl(bindingAttr, binder, callConvention, types, modifiers);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00002928 File Offset: 0x00000B28
		public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			bool flag = types == null;
			if (flag)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				bool flag2 = types[i] == null;
				if (flag2)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetConstructorImpl(bindingAttr, binder, CallingConventions.Any, types, modifiers);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00002988 File Offset: 0x00000B88
		public ConstructorInfo GetConstructor(Type[] types)
		{
			return this.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, types, null);
		}

		// Token: 0x06000076 RID: 118
		protected abstract ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);

		// Token: 0x06000077 RID: 119 RVA: 0x000029A8 File Offset: 0x00000BA8
		public MemberInfo[] GetMembers()
		{
			return this.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x06000078 RID: 120
		public abstract MemberInfo[] GetMembers(BindingFlags bindingAttr);

		// Token: 0x06000079 RID: 121 RVA: 0x000029C4 File Offset: 0x00000BC4
		public MemberInfo[] GetMember(string name)
		{
			return this.GetMember(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000029E0 File Offset: 0x00000BE0
		public virtual MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
		{
			return this.GetMember(name, MemberTypes.All, bindingAttr);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00002A00 File Offset: 0x00000C00
		public virtual MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			throw new NotSupportedException("NotSupported_SubclassOverride");
		}

		// Token: 0x0600007C RID: 124 RVA: 0x0000210A File Offset: 0x0000030A
		public virtual MemberInfo[] GetDefaultMembers()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00002A2C File Offset: 0x00000C2C
		public TypeAttributes Attributes
		{
			get
			{
				return this.GetAttributeFlagsImpl();
			}
		}

		// Token: 0x0600007E RID: 126
		protected abstract TypeAttributes GetAttributeFlagsImpl();

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00002A44 File Offset: 0x00000C44
		public bool IsAbstract
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.Abstract) > TypeAttributes.NotPublic;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00002A68 File Offset: 0x00000C68
		public bool IsSealed
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.Sealed) > TypeAttributes.NotPublic;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00002A8C File Offset: 0x00000C8C
		public bool IsSpecialName
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.SpecialName) > TypeAttributes.NotPublic;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00002AB0 File Offset: 0x00000CB0
		public bool IsImport
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.Import) > TypeAttributes.NotPublic;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00002AD4 File Offset: 0x00000CD4
		public bool IsAnsiClass
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.StringFormatMask) == TypeAttributes.NotPublic;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00002AF8 File Offset: 0x00000CF8
		public bool IsUnicodeClass
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.StringFormatMask) == TypeAttributes.UnicodeClass;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00002B20 File Offset: 0x00000D20
		public bool IsAutoClass
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.StringFormatMask) == TypeAttributes.AutoClass;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000086 RID: 134 RVA: 0x00002B48 File Offset: 0x00000D48
		public bool IsNotPublic
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NotPublic;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00002B68 File Offset: 0x00000D68
		public bool IsPublic
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.Public;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00002B88 File Offset: 0x00000D88
		public bool IsNestedPublic
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00002BA8 File Offset: 0x00000DA8
		public bool IsNestedPrivate
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPrivate;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600008A RID: 138 RVA: 0x00002BC8 File Offset: 0x00000DC8
		public bool IsNestedFamily
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamily;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00002BE8 File Offset: 0x00000DE8
		public bool IsNestedAssembly
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedAssembly;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00002C08 File Offset: 0x00000E08
		public bool IsNestedFamANDAssem
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamANDAssem;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00002C28 File Offset: 0x00000E28
		public bool IsNestedFamORAssem
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.VisibilityMask;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00002C48 File Offset: 0x00000E48
		public bool IsAutoLayout
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.LayoutMask) == TypeAttributes.NotPublic;
			}
		}

		// Token: 0x0600008F RID: 143
		protected abstract bool IsPrimitiveImpl();

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000090 RID: 144 RVA: 0x00002C68 File Offset: 0x00000E68
		public bool IsPrimitive
		{
			get
			{
				return this.IsPrimitiveImpl();
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000091 RID: 145 RVA: 0x00002C80 File Offset: 0x00000E80
		public virtual bool IsGenericType
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000092 RID: 146
		public abstract bool IsSubclassOf(Type c);

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000093 RID: 147 RVA: 0x00002C94 File Offset: 0x00000E94
		public virtual bool IsGenericParameter
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000094 RID: 148
		public abstract bool ContainsGenericParameters { get; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000095 RID: 149 RVA: 0x00002CA8 File Offset: 0x00000EA8
		public bool IsCOMObject
		{
			get
			{
				return this.IsCOMObjectImpl();
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000096 RID: 150
		public abstract Guid GUID { get; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00002CC0 File Offset: 0x00000EC0
		public bool HasElementType
		{
			get
			{
				return this.HasElementTypeImpl();
			}
		}

		// Token: 0x06000098 RID: 152
		protected abstract bool HasElementTypeImpl();

		// Token: 0x06000099 RID: 153
		protected abstract bool IsCOMObjectImpl();

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00002CD8 File Offset: 0x00000ED8
		public bool IsContextful
		{
			get
			{
				return this.IsContextfulImpl();
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00002CF0 File Offset: 0x00000EF0
		public bool IsMarshalByRef
		{
			get
			{
				return this.IsMarshalByRefImpl();
			}
		}

		// Token: 0x0600009C RID: 156
		protected abstract bool IsContextfulImpl();

		// Token: 0x0600009D RID: 157
		protected abstract bool IsMarshalByRefImpl();

		// Token: 0x0600009E RID: 158
		public abstract object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters);

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600009F RID: 159 RVA: 0x000020E5 File Offset: 0x000002E5
		public virtual StructLayoutAttribute StructLayoutAttribute
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00002D08 File Offset: 0x00000F08
		public static TypeCode GetTypeCode(Type type)
		{
			bool flag = type == null;
			TypeCode result;
			if (flag)
			{
				result = TypeCode.Empty;
			}
			else
			{
				result = type.GetTypeCodeImpl();
			}
			return result;
		}

		// Token: 0x060000A1 RID: 161
		protected abstract TypeCode GetTypeCodeImpl();

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x000020E5 File Offset: 0x000002E5
		public virtual RuntimeTypeHandle TypeHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00002D2C File Offset: 0x00000F2C
		public virtual MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
		{
			MethodInfo[] array = null;
			ConstructorInfo[] array2 = null;
			FieldInfo[] array3 = null;
			PropertyInfo[] array4 = null;
			EventInfo[] array5 = null;
			Type[] array6 = null;
			int num = 0;
			bool flag = (memberType & MemberTypes.Method) > (MemberTypes)0;
			if (flag)
			{
				array = this.GetMethods(bindingAttr);
				bool flag2 = filter != null;
				if (flag2)
				{
					for (int i = 0; i < array.Length; i++)
					{
						bool flag3 = !filter(array[i], filterCriteria);
						if (flag3)
						{
							array[i] = null;
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					num += array.Length;
				}
			}
			bool flag4 = (memberType & MemberTypes.Constructor) > (MemberTypes)0;
			if (flag4)
			{
				array2 = this.GetConstructors(bindingAttr);
				bool flag5 = filter != null;
				if (flag5)
				{
					for (int i = 0; i < array2.Length; i++)
					{
						bool flag6 = !filter(array2[i], filterCriteria);
						if (flag6)
						{
							array2[i] = null;
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					num += array2.Length;
				}
			}
			bool flag7 = (memberType & MemberTypes.Field) > (MemberTypes)0;
			if (flag7)
			{
				array3 = this.GetFields(bindingAttr);
				bool flag8 = filter != null;
				if (flag8)
				{
					for (int i = 0; i < array3.Length; i++)
					{
						bool flag9 = !filter(array3[i], filterCriteria);
						if (flag9)
						{
							array3[i] = null;
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					num += array3.Length;
				}
			}
			bool flag10 = (memberType & MemberTypes.Property) > (MemberTypes)0;
			if (flag10)
			{
				array4 = this.GetProperties(bindingAttr);
				bool flag11 = filter != null;
				if (flag11)
				{
					for (int i = 0; i < array4.Length; i++)
					{
						bool flag12 = !filter(array4[i], filterCriteria);
						if (flag12)
						{
							array4[i] = null;
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					num += array4.Length;
				}
			}
			bool flag13 = (memberType & MemberTypes.Event) > (MemberTypes)0;
			if (flag13)
			{
				array5 = this.GetEvents();
				bool flag14 = filter != null;
				if (flag14)
				{
					for (int i = 0; i < array5.Length; i++)
					{
						bool flag15 = !filter(array5[i], filterCriteria);
						if (flag15)
						{
							array5[i] = null;
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					num += array5.Length;
				}
			}
			bool flag16 = (memberType & MemberTypes.NestedType) > (MemberTypes)0;
			if (flag16)
			{
				array6 = this.GetNestedTypes(bindingAttr);
				bool flag17 = filter != null;
				if (flag17)
				{
					for (int i = 0; i < array6.Length; i++)
					{
						bool flag18 = !filter(array6[i], filterCriteria);
						if (flag18)
						{
							array6[i] = null;
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					num += array6.Length;
				}
			}
			MemberInfo[] array7 = new MemberInfo[num];
			num = 0;
			bool flag19 = array != null;
			if (flag19)
			{
				for (int i = 0; i < array.Length; i++)
				{
					bool flag20 = array[i] != null;
					if (flag20)
					{
						array7[num++] = array[i];
					}
				}
			}
			bool flag21 = array2 != null;
			if (flag21)
			{
				for (int i = 0; i < array2.Length; i++)
				{
					bool flag22 = array2[i] != null;
					if (flag22)
					{
						array7[num++] = array2[i];
					}
				}
			}
			bool flag23 = array3 != null;
			if (flag23)
			{
				for (int i = 0; i < array3.Length; i++)
				{
					bool flag24 = array3[i] != null;
					if (flag24)
					{
						array7[num++] = array3[i];
					}
				}
			}
			bool flag25 = array4 != null;
			if (flag25)
			{
				for (int i = 0; i < array4.Length; i++)
				{
					bool flag26 = array4[i] != null;
					if (flag26)
					{
						array7[num++] = array4[i];
					}
				}
			}
			bool flag27 = array5 != null;
			if (flag27)
			{
				for (int i = 0; i < array5.Length; i++)
				{
					bool flag28 = array5[i] != null;
					if (flag28)
					{
						array7[num++] = array5[i];
					}
				}
			}
			bool flag29 = array6 != null;
			if (flag29)
			{
				for (int i = 0; i < array6.Length; i++)
				{
					bool flag30 = array6[i] != null;
					if (flag30)
					{
						array7[num++] = array6[i];
					}
				}
			}
			return array7;
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00003164 File Offset: 0x00001364
		public bool IsExplicitLayout
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.LayoutMask) == TypeAttributes.ExplicitLayout;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00003184 File Offset: 0x00001384
		public bool IsLayoutSequential
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.LayoutMask) == TypeAttributes.SequentialLayout;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x000031A4 File Offset: 0x000013A4
		public bool IsVisible
		{
			get
			{
				bool isGenericParameter = this.IsGenericParameter;
				bool result;
				if (isGenericParameter)
				{
					result = true;
				}
				else
				{
					bool hasElementType = this.HasElementType;
					if (hasElementType)
					{
						result = this.GetElementType().IsVisible;
					}
					else
					{
						Type type = this;
						while (type.IsNested)
						{
							bool flag = !type.IsNestedPublic;
							if (flag)
							{
								return false;
							}
							type = type.DeclaringType;
						}
						bool flag2 = !type.IsPublic;
						if (flag2)
						{
							result = false;
						}
						else
						{
							bool flag3 = this.IsGenericType && !this.IsGenericTypeDefinition;
							if (flag3)
							{
								foreach (Type type2 in this.GetGenericArguments())
								{
									bool flag4 = !type2.IsVisible;
									if (flag4)
									{
										return false;
									}
								}
							}
							result = true;
						}
					}
				}
				return result;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x000020E5 File Offset: 0x000002E5
		public virtual GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x0000210A File Offset: 0x0000030A
		public virtual int GenericParameterPosition
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x04000001 RID: 1
		public static readonly Type[] EmptyTypes = new Type[0];

		// Token: 0x04000002 RID: 2
		public static readonly char Delimiter = '.';

		// Token: 0x04000003 RID: 3
		private const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
	}
}
