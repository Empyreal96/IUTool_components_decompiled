using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200001E RID: 30
	internal class MetadataOnlyCommonArrayType : MetadataOnlyCommonType
	{
		// Token: 0x06000137 RID: 311 RVA: 0x00003490 File Offset: 0x00001690
		public MetadataOnlyCommonArrayType(MetadataOnlyCommonType elementType)
		{
			ITypeUniverse typeUniverse = Helpers.Universe(elementType);
			this._baseType = typeUniverse.GetTypeXFromName("System.Array");
			this._elementType = elementType;
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000138 RID: 312 RVA: 0x000034C4 File Offset: 0x000016C4
		public override string Namespace
		{
			get
			{
				return this._elementType.Namespace;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000139 RID: 313 RVA: 0x000034E4 File Offset: 0x000016E4
		internal override MetadataOnlyModule Resolver
		{
			get
			{
				return this._elementType.Resolver;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600013A RID: 314 RVA: 0x00003504 File Offset: 0x00001704
		public override Type BaseType
		{
			get
			{
				return this._baseType;
			}
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000351C File Offset: 0x0000171C
		protected override bool IsArrayImpl()
		{
			return true;
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600013C RID: 316 RVA: 0x00003530 File Offset: 0x00001730
		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00003544 File Offset: 0x00001744
		public override Type GetElementType()
		{
			return this._elementType;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000355C File Offset: 0x0000175C
		public override int GetHashCode()
		{
			return this._elementType.GetHashCode();
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600013F RID: 319 RVA: 0x0000357C File Offset: 0x0000177C
		public override int MetadataToken
		{
			get
			{
				return 33554432;
			}
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00003594 File Offset: 0x00001794
		public override Type[] GetGenericArguments()
		{
			return this._elementType.GetGenericArguments();
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00002390 File Offset: 0x00000590
		public override Type GetGenericTypeDefinition()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000142 RID: 322 RVA: 0x000035B4 File Offset: 0x000017B4
		internal override IEnumerable<MethodBase> GetDeclaredMethods()
		{
			return this.Resolver.Policy.GetExtraArrayMethods(this);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x000035D8 File Offset: 0x000017D8
		internal override IEnumerable<MethodBase> GetDeclaredConstructors()
		{
			return this.Resolver.Policy.GetExtraArrayConstructors(this);
		}

		// Token: 0x06000144 RID: 324 RVA: 0x000035FC File Offset: 0x000017FC
		public override FieldInfo[] GetFields(BindingFlags flags)
		{
			return new FieldInfo[0];
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00003614 File Offset: 0x00001814
		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			return null;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00003628 File Offset: 0x00001828
		public override EventInfo[] GetEvents(BindingFlags flags)
		{
			return new EventInfo[0];
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00003640 File Offset: 0x00001840
		public override EventInfo GetEvent(string name, BindingFlags flags)
		{
			return null;
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00003654 File Offset: 0x00001854
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			TypeAttributes typeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
			return typeAttributes | TypeAttributes.Serializable;
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00003678 File Offset: 0x00001878
		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			return null;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000368C File Offset: 0x0000188C
		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			return new Type[0];
		}

		// Token: 0x0600014B RID: 331 RVA: 0x000036A4 File Offset: 0x000018A4
		public override Type[] GetInterfaces()
		{
			List<Type> list = new List<Type>(this._baseType.GetInterfaces());
			list.AddRange(this.Resolver.Policy.GetExtraArrayInterfaces(this._elementType));
			return list.ToArray();
		}

		// Token: 0x0600014C RID: 332 RVA: 0x000036EC File Offset: 0x000018EC
		public override Type GetInterface(string name, bool ignoreCase)
		{
			return MetadataOnlyModule.GetInterfaceHelper(this.GetInterfaces(), name, ignoreCase);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00002390 File Offset: 0x00000590
		public override Type MakeGenericType(params Type[] argTypes)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600014E RID: 334 RVA: 0x0000370C File Offset: 0x0000190C
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.TypeInfo;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600014F RID: 335 RVA: 0x00003720 File Offset: 0x00001920
		public override Type DeclaringType
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000240B File Offset: 0x0000060B
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000153 RID: 339 RVA: 0x00003734 File Offset: 0x00001934
		public override Assembly Assembly
		{
			get
			{
				return this._elementType.Assembly;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00003754 File Offset: 0x00001954
		public override Guid GUID
		{
			get
			{
				return Guid.Empty;
			}
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000376C File Offset: 0x0000196C
		protected override bool HasElementTypeImpl()
		{
			return true;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000240B File Offset: 0x0000060B
		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00003780 File Offset: 0x00001980
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			CustomAttributeData serializableAttribute = PseudoCustomAttributes.GetSerializableAttribute(this.Resolver, false);
			bool flag = serializableAttribute != null;
			IList<CustomAttributeData> result;
			if (flag)
			{
				result = new CustomAttributeData[]
				{
					serializableAttribute
				};
			}
			else
			{
				result = new CustomAttributeData[0];
			}
			return result;
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000158 RID: 344 RVA: 0x000037BB File Offset: 0x000019BB
		public override GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				throw new InvalidOperationException(Resources.ValidOnGenericParameterTypeOnly);
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x000037C8 File Offset: 0x000019C8
		protected override TypeCode GetTypeCodeImpl()
		{
			return TypeCode.Object;
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600015A RID: 346 RVA: 0x00002390 File Offset: 0x00000590
		public override string FullName
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600015B RID: 347 RVA: 0x00002390 File Offset: 0x00000590
		public override string Name
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00002390 File Offset: 0x00000590
		public override bool IsAssignableFrom(Type c)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00002390 File Offset: 0x00000590
		public override bool Equals(Type o)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x04000052 RID: 82
		private readonly MetadataOnlyCommonType _elementType;

		// Token: 0x04000053 RID: 83
		private readonly Type _baseType;
	}
}
