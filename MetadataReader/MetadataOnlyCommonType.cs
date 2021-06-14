using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000022 RID: 34
	[DebuggerDisplay("\\{Name = {Name} FullName = {FullName}\\}")]
	internal abstract class MetadataOnlyCommonType : Type
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000191 RID: 401
		internal abstract MetadataOnlyModule Resolver { get; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000192 RID: 402 RVA: 0x00004828 File Offset: 0x00002A28
		internal virtual GenericContext GenericContext
		{
			get
			{
				return new GenericContext(this.GetGenericArguments(), null);
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00004848 File Offset: 0x00002A48
		internal virtual IEnumerable<MethodBase> GetDeclaredMethods()
		{
			return new MethodInfo[0];
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00004860 File Offset: 0x00002A60
		internal virtual IEnumerable<MethodBase> GetDeclaredConstructors()
		{
			return new MethodInfo[0];
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00004878 File Offset: 0x00002A78
		internal virtual IEnumerable<PropertyInfo> GetDeclaredProperties()
		{
			return new PropertyInfo[0];
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00004890 File Offset: 0x00002A90
		public override PropertyInfo[] GetProperties(BindingFlags flags)
		{
			return MetadataOnlyModule.GetPropertiesOnType(this, flags);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x000048AC File Offset: 0x00002AAC
		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return MetadataOnlyTypeDef.GetPropertyImplHelper(this, name, bindingAttr, binder, returnType, types, modifiers);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x000048D0 File Offset: 0x00002AD0
		public override MethodInfo[] GetMethods(BindingFlags flags)
		{
			return MetadataOnlyModule.GetMethodsOnType(this, flags);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x000048EC File Offset: 0x00002AEC
		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return MetadataOnlyModule.GetMethodImplHelper(this, name, bindingAttr, binder, callConvention, types, modifiers);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00004910 File Offset: 0x00002B10
		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			return MetadataOnlyTypeDef.GetMembersHelper(this, bindingAttr);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000492C File Offset: 0x00002B2C
		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return MetadataOnlyModule.GetConstructorOnType(this, bindingAttr, binder, callConvention, types, modifiers);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000494C File Offset: 0x00002B4C
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			return MetadataOnlyModule.GetConstructorsOnType(this, bindingAttr);
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600019D RID: 413 RVA: 0x00004968 File Offset: 0x00002B68
		public override Module Module
		{
			get
			{
				return this.Resolver;
			}
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00004980 File Offset: 0x00002B80
		public override bool Equals(object objOther)
		{
			Type type = objOther as Type;
			bool flag = type == null;
			return !flag && this.Equals(type);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x000049AC File Offset: 0x00002BAC
		public override int GetHashCode()
		{
			return this.MetadataToken;
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x000049C4 File Offset: 0x00002BC4
		public override bool ContainsGenericParameters
		{
			get
			{
				bool hasElementType = base.HasElementType;
				bool result;
				if (hasElementType)
				{
					result = this.GetRootElementType().ContainsGenericParameters;
				}
				else
				{
					bool isGenericParameter = this.IsGenericParameter;
					if (isGenericParameter)
					{
						result = true;
					}
					else
					{
						bool flag = !this.IsGenericType;
						if (flag)
						{
							result = false;
						}
						else
						{
							Type[] genericArguments = this.GetGenericArguments();
							for (int i = 0; i < genericArguments.Length; i++)
							{
								bool containsGenericParameters = genericArguments[i].ContainsGenericParameters;
								if (containsGenericParameters)
								{
									return true;
								}
							}
							result = false;
						}
					}
				}
				return result;
			}
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00004A44 File Offset: 0x00002C44
		private Type GetRootElementType()
		{
			Type type = this;
			while (type.HasElementType)
			{
				type = type.GetElementType();
			}
			return type;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00004A6C File Offset: 0x00002C6C
		public override bool IsSubclassOf(Type c)
		{
			Type type = this;
			bool flag = type.Equals(c);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				while (type != null)
				{
					bool flag2 = type.Equals(c);
					if (flag2)
					{
						return true;
					}
					type = type.BaseType;
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00004AB4 File Offset: 0x00002CB4
		protected override bool IsContextfulImpl()
		{
			Type typeXFromName = this.Resolver.AssemblyResolver.GetTypeXFromName("System.ContextBoundObject");
			bool flag = typeXFromName != null;
			return flag && typeXFromName.IsAssignableFrom(this);
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00004AF0 File Offset: 0x00002CF0
		protected override bool IsMarshalByRefImpl()
		{
			Type typeXFromName = this.Resolver.AssemblyResolver.GetTypeXFromName("System.MarshalByRefObject");
			bool flag = typeXFromName != null;
			return flag && typeXFromName.IsAssignableFrom(this);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00004B2C File Offset: 0x00002D2C
		public override MemberInfo[] GetDefaultMembers()
		{
			Type typeXFromName = this.Resolver.AssemblyResolver.GetTypeXFromName("System.Reflection.DefaultMemberAttribute");
			bool flag = typeXFromName == null;
			MemberInfo[] result;
			if (flag)
			{
				result = new MemberInfo[0];
			}
			else
			{
				CustomAttributeData customAttributeData = null;
				for (Type type = this; type != null; type = type.BaseType)
				{
					IList<CustomAttributeData> customAttributesData = type.GetCustomAttributesData();
					for (int i = 0; i < customAttributesData.Count; i++)
					{
						bool flag2 = customAttributesData[i].Constructor.DeclaringType.Equals(typeXFromName);
						if (flag2)
						{
							customAttributeData = customAttributesData[i];
							break;
						}
					}
					bool flag3 = customAttributeData != null;
					if (flag3)
					{
						break;
					}
				}
				bool flag4 = customAttributeData == null;
				if (flag4)
				{
					result = new MemberInfo[0];
				}
				else
				{
					string name = customAttributeData.ConstructorArguments[0].Value as string;
					MemberInfo[] array = base.GetMember(name);
					bool flag5 = array == null;
					if (flag5)
					{
						array = new MemberInfo[0];
					}
					result = array;
				}
			}
			return result;
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00004C34 File Offset: 0x00002E34
		public override bool IsInstanceOfType(object o)
		{
			return false;
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x00004C48 File Offset: 0x00002E48
		public override string AssemblyQualifiedName
		{
			get
			{
				string fullName = this.FullName;
				bool flag = fullName == null;
				string result;
				if (flag)
				{
					result = null;
				}
				else
				{
					Assembly assembly = this.Assembly;
					string assemblyName = assembly.GetName().ToString();
					result = System.Reflection.Assembly.CreateQualifiedName(assemblyName, fullName);
				}
				return result;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x00004C8C File Offset: 0x00002E8C
		public override bool IsSerializable
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.Serializable) != TypeAttributes.NotPublic || this.QuickSerializationCastCheck();
			}
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00004CB8 File Offset: 0x00002EB8
		private bool QuickSerializationCastCheck()
		{
			ITypeUniverse typeUniverse = Helpers.Universe(this);
			Type typeXFromName = typeUniverse.GetTypeXFromName("System.Enum");
			Type typeXFromName2 = typeUniverse.GetTypeXFromName("System.Delegate");
			for (Type type = this.UnderlyingSystemType; type != null; type = type.BaseType)
			{
				bool flag = type.Equals(typeXFromName) || type.Equals(typeXFromName2);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060001AA RID: 426 RVA: 0x0000240B File Offset: 0x0000060B
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060001AB RID: 427 RVA: 0x00004D28 File Offset: 0x00002F28
		public override bool IsEnum
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00004D3C File Offset: 0x00002F3C
		protected override bool IsArrayImpl()
		{
			return false;
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00004D50 File Offset: 0x00002F50
		protected override bool IsByRefImpl()
		{
			return false;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00004D64 File Offset: 0x00002F64
		protected override bool IsPointerImpl()
		{
			return false;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00004D78 File Offset: 0x00002F78
		protected override bool IsPrimitiveImpl()
		{
			return false;
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x00004D8C File Offset: 0x00002F8C
		public override bool IsGenericType
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x00004DA0 File Offset: 0x00002FA0
		public override bool IsGenericParameter
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x000037BB File Offset: 0x000019BB
		public override MethodBase DeclaringMethod
		{
			get
			{
				throw new InvalidOperationException(Resources.ValidOnGenericParameterTypeOnly);
			}
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000232A File Offset: 0x0000052A
		protected override bool IsCOMObjectImpl()
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x00004DB4 File Offset: 0x00002FB4
		public override StructLayoutAttribute StructLayoutAttribute
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00004DC7 File Offset: 0x00002FC7
		public override int GetArrayRank()
		{
			throw new ArgumentException(Resources.OperationValidOnArrayTypeOnly);
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00002390 File Offset: 0x00000590
		public override Type MakeGenericType(params Type[] argTypes)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00004DD4 File Offset: 0x00002FD4
		public override Type MakeByRefType()
		{
			return this.Resolver.Factory.CreateByRefType(this);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00004DF8 File Offset: 0x00002FF8
		public override Type MakePointerType()
		{
			return this.Resolver.Factory.CreatePointerType(this);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00004E1C File Offset: 0x0000301C
		public override Type MakeArrayType()
		{
			return this.Resolver.Factory.CreateVectorType(this);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00004E40 File Offset: 0x00003040
		public override Type MakeArrayType(int rank)
		{
			return this.MakeArrayTypeHelper(rank);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00004E5C File Offset: 0x0000305C
		[SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
		private Type MakeArrayTypeHelper(int rank)
		{
			bool flag = rank <= 0;
			if (flag)
			{
				throw new IndexOutOfRangeException();
			}
			return this.Resolver.Factory.CreateArrayType(this, rank);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00004E94 File Offset: 0x00003094
		internal static string TypeSigToString(Type pThis)
		{
			StringBuilder stringBuilder = StringBuilderPool.Get();
			MetadataOnlyCommonType.TypeSigToString(pThis, stringBuilder);
			string result = stringBuilder.ToString();
			StringBuilderPool.Release(ref stringBuilder);
			return result;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00004EC4 File Offset: 0x000030C4
		internal static void TypeSigToString(Type pThis, StringBuilder sb)
		{
			Type type = pThis;
			while (type.HasElementType)
			{
				type = type.GetElementType();
			}
			bool isNested = type.IsNested;
			if (isNested)
			{
				sb.Append(pThis.Name);
			}
			else
			{
				string text = pThis.ToString();
				bool flag = type.IsPrimitive || type.FullName == "System.Void";
				if (flag)
				{
					text = text.Substring("System.".Length);
				}
				sb.Append(text);
			}
		}
	}
}
