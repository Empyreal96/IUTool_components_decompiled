using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.MetadataReader.Internal;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200002F RID: 47
	internal class MetadataOnlyTypeDef : MetadataOnlyCommonType
	{
		// Token: 0x06000317 RID: 791 RVA: 0x0000A993 File Offset: 0x00008B93
		public MetadataOnlyTypeDef(MetadataOnlyModule scope, Token tokenTypeDef) : this(scope, tokenTypeDef, null)
		{
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0000A9A0 File Offset: 0x00008BA0
		public MetadataOnlyTypeDef(MetadataOnlyModule scope, Token tokenTypeDef, Type[] typeParameters)
		{
			MetadataOnlyTypeDef.ValidateConstructorArguments(scope, tokenTypeDef);
			this._resolver = scope;
			this._tokenTypeDef = tokenTypeDef;
			this._typeParameters = null;
			this._resolver.GetTypeAttributes(this._tokenTypeDef, out this._tokenExtends, out this._typeAttributes, out this._nameLength);
			int num = this._resolver.CountGenericParams(this._tokenTypeDef);
			bool flag = typeParameters != null && typeParameters.Length != 0;
			bool flag2 = num > 0;
			if (flag2)
			{
				bool flag3 = !flag;
				if (flag3)
				{
					this._typeParameters = new Type[num];
					int num2 = 0;
					foreach (int value in this._resolver.GetGenericParameterTokens(this._tokenTypeDef))
					{
						this._typeParameters[num2++] = this._resolver.Factory.CreateTypeVariable(this._resolver, new Token(value));
					}
				}
				else
				{
					bool flag4 = num == typeParameters.Length;
					if (!flag4)
					{
						throw new ArgumentException(Resources.WrongNumberOfGenericArguments);
					}
					this._typeParameters = typeParameters;
				}
			}
			else
			{
				this._typeParameters = Type.EmptyTypes;
			}
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0000AAF8 File Offset: 0x00008CF8
		private static void ValidateConstructorArguments(MetadataOnlyModule scope, Token tokenTypeDef)
		{
			bool flag = scope == null;
			if (flag)
			{
				throw new ArgumentNullException("scope");
			}
			bool flag2 = !tokenTypeDef.IsType(TokenType.TypeDef);
			if (flag2)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ExpectedTokenType, new object[]
				{
					TokenType.TypeDef
				}));
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x0600031A RID: 794 RVA: 0x0000AB58 File Offset: 0x00008D58
		private string LocalFullName
		{
			get
			{
				bool flag = string.IsNullOrEmpty(this._fullName);
				if (flag)
				{
					this._resolver.GetTypeName(this._tokenTypeDef, this._nameLength, out this._fullName);
				}
				return this._fullName;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x0600031B RID: 795 RVA: 0x0000ABA0 File Offset: 0x00008DA0
		internal override MetadataOnlyModule Resolver
		{
			get
			{
				return this._resolver;
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x0600031C RID: 796 RVA: 0x0000ABB8 File Offset: 0x00008DB8
		public override int MetadataToken
		{
			get
			{
				return this._tokenTypeDef.Value;
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600031D RID: 797 RVA: 0x0000ABD8 File Offset: 0x00008DD8
		public override string FullName
		{
			get
			{
				bool flag = (!this.IsGenericType || this.IsGenericTypeDefinition) && this.DeclaringType == null;
				string result;
				if (flag)
				{
					result = this.LocalFullName;
				}
				else
				{
					StringBuilder stringBuilder = StringBuilderPool.Get();
					this.GetSimpleName(stringBuilder);
					bool flag2 = !this.IsGenericType || this.IsGenericTypeDefinition;
					if (flag2)
					{
						string text = stringBuilder.ToString();
						StringBuilderPool.Release(ref stringBuilder);
						result = text;
					}
					else
					{
						stringBuilder.Append("[");
						Type[] genericArguments = this.GetGenericArguments();
						for (int i = 0; i < genericArguments.Length; i++)
						{
							bool flag3 = i > 0;
							if (flag3)
							{
								stringBuilder.Append(",");
							}
							stringBuilder.Append('[');
							bool flag4 = genericArguments[i].FullName == null || genericArguments[i].IsGenericTypeDefinition;
							if (flag4)
							{
								return null;
							}
							stringBuilder.Append(genericArguments[i].AssemblyQualifiedName);
							stringBuilder.Append(']');
						}
						stringBuilder.Append("]");
						string text2 = stringBuilder.ToString();
						StringBuilderPool.Release(ref stringBuilder);
						result = text2;
					}
				}
				return result;
			}
		}

		// Token: 0x0600031E RID: 798 RVA: 0x0000AD00 File Offset: 0x00008F00
		private void GetSimpleName(StringBuilder sb)
		{
			Type declaringType = this.DeclaringType;
			bool flag = declaringType != null;
			if (flag)
			{
				sb.Append(declaringType.FullName);
				sb.Append('+');
			}
			sb.Append(this.LocalFullName);
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x0600031F RID: 799 RVA: 0x0000AD44 File Offset: 0x00008F44
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
					result = Utility.GetNamespaceHelper(this.LocalFullName);
				}
				return result;
			}
		}

		// Token: 0x06000320 RID: 800 RVA: 0x0000AD80 File Offset: 0x00008F80
		public override string ToString()
		{
			bool flag = !this.IsGenericType;
			string result;
			if (flag)
			{
				result = this.FullName;
			}
			else
			{
				StringBuilder stringBuilder = StringBuilderPool.Get();
				this.GetSimpleName(stringBuilder);
				stringBuilder.Append("[");
				Type[] genericArguments = this.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					bool flag2 = i != 0;
					if (flag2)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(genericArguments[i]);
				}
				stringBuilder.Append("]");
				string text = stringBuilder.ToString();
				StringBuilderPool.Release(ref stringBuilder);
				result = text;
			}
			return result;
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000321 RID: 801 RVA: 0x0000AE28 File Offset: 0x00009028
		public override Type BaseType
		{
			get
			{
				bool flag = this._baseType == null;
				if (flag)
				{
					bool isNil = this._tokenExtends.IsNil;
					if (isNil)
					{
						return null;
					}
					this._baseType = this._resolver.ResolveTypeTokenInternal(this._tokenExtends, this.GenericContext);
				}
				return this._baseType;
			}
		}

		// Token: 0x06000322 RID: 802 RVA: 0x0000AE84 File Offset: 0x00009084
		public override bool Equals(Type other)
		{
			bool flag = other == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.Module.Equals(other.Module);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool isGenericType = this.IsGenericType;
					bool isGenericType2 = other.IsGenericType;
					bool flag3 = isGenericType != isGenericType2;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = this.MetadataToken != other.MetadataToken;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = !isGenericType && !isGenericType2;
							if (flag5)
							{
								result = true;
							}
							else
							{
								Type[] genericArguments = this.GetGenericArguments();
								Type[] genericArguments2 = other.GetGenericArguments();
								bool flag6 = genericArguments.Length != genericArguments2.Length;
								if (flag6)
								{
									result = false;
								}
								else
								{
									for (int i = 0; i < genericArguments.Length; i++)
									{
										bool flag7 = !genericArguments[i].Equals(genericArguments2[i]);
										if (flag7)
										{
											return false;
										}
									}
									result = true;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000323 RID: 803 RVA: 0x0000AF84 File Offset: 0x00009184
		public override Type MakeGenericType(params Type[] argTypes)
		{
			bool flag = argTypes == null;
			if (flag)
			{
				throw new ArgumentNullException("argTypes");
			}
			bool isGenericTypeDefinition = this.IsGenericTypeDefinition;
			if (!isGenericTypeDefinition)
			{
				throw new InvalidOperationException();
			}
			bool flag2 = argTypes.Length == this._typeParameters.Length;
			if (flag2)
			{
				return this.Resolver.Factory.CreateGenericType(this.Resolver, this._tokenTypeDef, argTypes);
			}
			throw new ArgumentException(Resources.WrongNumberOfGenericArguments);
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000324 RID: 804 RVA: 0x0000AFF8 File Offset: 0x000091F8
		public override bool IsEnum
		{
			get
			{
				Type typeXFromName = this._resolver.AssemblyResolver.GetTypeXFromName("System.Enum");
				return typeXFromName.Equals(this.BaseType);
			}
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0000B02C File Offset: 0x0000922C
		public override bool IsAssignableFrom(Type c)
		{
			return MetadataOnlyTypeDef.IsAssignableFromHelper(this, c);
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0000B048 File Offset: 0x00009248
		internal static bool IsAssignableFromHelper(Type current, Type target)
		{
			bool flag = target == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = current.Equals(target);
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = target.IsSubclassOf(current);
					if (flag3)
					{
						result = true;
					}
					else
					{
						Type[] interfaces = target.GetInterfaces();
						for (int i = 0; i < interfaces.Length; i++)
						{
							bool flag4 = interfaces[i].Equals(current);
							if (flag4)
							{
								return true;
							}
							bool flag5 = current.IsAssignableFrom(interfaces[i]);
							if (flag5)
							{
								return true;
							}
						}
						bool isGenericParameter = target.IsGenericParameter;
						if (isGenericParameter)
						{
							Type[] genericParameterConstraints = target.GetGenericParameterConstraints();
							for (int j = 0; j < genericParameterConstraints.Length; j++)
							{
								bool flag6 = MetadataOnlyTypeDef.IsAssignableFromHelper(current, genericParameterConstraints[j]);
								if (flag6)
								{
									return true;
								}
							}
						}
						ITypeUniverse typeUniverse = Helpers.Universe(current);
						bool flag7 = typeUniverse != null;
						if (flag7)
						{
							bool flag8 = current.Equals(typeUniverse.GetTypeXFromName("System.Object"));
							if (flag8)
							{
								return target.IsPointer || target.IsInterface || target.IsArray;
							}
						}
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000327 RID: 807 RVA: 0x0000B16C File Offset: 0x0000936C
		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0000B180 File Offset: 0x00009380
		protected override bool IsValueTypeImpl()
		{
			bool flag = this._fIsValueType == MetadataOnlyTypeDef.TriState.Maybe;
			if (flag)
			{
				bool flag2 = this.IsValueTypeHelper();
				bool flag3 = flag2;
				if (flag3)
				{
					this._fIsValueType = MetadataOnlyTypeDef.TriState.Yes;
				}
				else
				{
					this._fIsValueType = MetadataOnlyTypeDef.TriState.No;
				}
			}
			bool flag4 = this._fIsValueType == MetadataOnlyTypeDef.TriState.Yes;
			bool result;
			if (flag4)
			{
				result = true;
			}
			else
			{
				bool flag5 = this._fIsValueType == MetadataOnlyTypeDef.TriState.No;
				result = (flag5 && false);
			}
			return result;
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0000B1E8 File Offset: 0x000093E8
		private bool IsValueTypeHelper()
		{
			MetadataOnlyModule resolver = this.Resolver;
			Type typeXFromName = resolver.AssemblyResolver.GetTypeXFromName("System.Enum");
			bool flag = this.Equals(typeXFromName);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Type typeXFromName2 = resolver.AssemblyResolver.GetTypeXFromName("System.ValueType");
				result = (typeXFromName2.Equals(this.BaseType) || this.IsEnum);
			}
			return result;
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0000B250 File Offset: 0x00009450
		protected override bool IsPrimitiveImpl()
		{
			bool flag = !this._resolver.IsSystemModule();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string fullName = this.FullName;
				foreach (string text in MetadataOnlyTypeDef.s_primitiveTypeNames)
				{
					bool flag2 = text.Equals(fullName, StringComparison.Ordinal);
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x0600032B RID: 811 RVA: 0x0000B2B8 File Offset: 0x000094B8
		public override bool IsGenericType
		{
			get
			{
				return this._typeParameters.Length != 0;
			}
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0000B2D4 File Offset: 0x000094D4
		public override Type[] GetGenericArguments()
		{
			return (Type[])this._typeParameters.Clone();
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000B2F8 File Offset: 0x000094F8
		public override Type GetGenericTypeDefinition()
		{
			bool flag = !this.IsGenericType;
			if (flag)
			{
				throw new InvalidOperationException();
			}
			bool isGenericTypeDefinition = this.IsGenericTypeDefinition;
			Type result;
			if (isGenericTypeDefinition)
			{
				result = this;
			}
			else
			{
				result = this.Resolver.Factory.CreateSimpleType(this.Resolver, this._tokenTypeDef);
			}
			return result;
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600032E RID: 814 RVA: 0x0000B34C File Offset: 0x0000954C
		public override bool IsGenericTypeDefinition
		{
			get
			{
				bool flag = !this.IsGenericType;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					Type[] genericArguments = this.GetGenericArguments();
					foreach (Type type in genericArguments)
					{
						bool flag2 = !type.IsGenericParameter;
						if (flag2)
						{
							return false;
						}
						bool flag3 = !type.DeclaringType.Equals(this);
						if (flag3)
						{
							return false;
						}
					}
					result = true;
				}
				return result;
			}
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0000B3C8 File Offset: 0x000095C8
		public override Type GetElementType()
		{
			return null;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0000B3DC File Offset: 0x000095DC
		public override FieldInfo[] GetFields(BindingFlags flags)
		{
			return MetadataOnlyModule.GetFieldsOnType(this, flags);
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0000B3F8 File Offset: 0x000095F8
		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			StringComparison stringComparison = SignatureUtil.GetStringComparison(bindingAttr);
			FieldInfo[] fields = this.GetFields(bindingAttr);
			foreach (FieldInfo fieldInfo in fields)
			{
				bool flag2 = fieldInfo.Name.Equals(name, stringComparison);
				if (flag2)
				{
					return fieldInfo;
				}
			}
			return null;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0000B468 File Offset: 0x00009668
		internal static PropertyInfo GetPropertyImplHelper(MetadataOnlyCommonType type, string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			bool flag = binder != null;
			if (flag)
			{
				throw new NotSupportedException();
			}
			bool flag2 = modifiers != null && modifiers.Length != 0;
			if (flag2)
			{
				throw new NotSupportedException();
			}
			StringComparison stringComparison = SignatureUtil.GetStringComparison(bindingAttr);
			PropertyInfo[] properties = type.GetProperties(bindingAttr);
			foreach (PropertyInfo propertyInfo in properties)
			{
				bool flag3 = !propertyInfo.Name.Equals(name, stringComparison);
				if (!flag3)
				{
					bool flag4 = returnType != null;
					if (flag4)
					{
						bool flag5 = !propertyInfo.PropertyType.Equals(returnType);
						if (flag5)
						{
							goto IL_A4;
						}
					}
					bool flag6 = !MetadataOnlyTypeDef.PropertyParamTypesMatch(propertyInfo, types);
					if (!flag6)
					{
						return propertyInfo;
					}
				}
				IL_A4:;
			}
			return null;
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0000B530 File Offset: 0x00009730
		public override Type[] GetInterfaces()
		{
			return MetadataOnlyTypeDef.GetAllInterfacesHelper(this);
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0000B548 File Offset: 0x00009748
		internal static Type[] GetAllInterfacesHelper(MetadataOnlyCommonType type)
		{
			HashSet<Type> hashSet = new HashSet<Type>();
			bool flag = type.BaseType != null;
			if (flag)
			{
				Type[] interfaces = type.BaseType.GetInterfaces();
				hashSet.UnionWith(interfaces);
			}
			IEnumerable<Type> interfacesOnType = type.Resolver.GetInterfacesOnType(type);
			foreach (Type type2 in interfacesOnType)
			{
				bool flag2 = hashSet.Contains(type2);
				if (!flag2)
				{
					hashSet.Add(type2);
					Type[] interfaces2 = type2.GetInterfaces();
					hashSet.UnionWith(interfaces2);
				}
			}
			Type[] array = new Type[hashSet.Count];
			hashSet.CopyTo(array);
			return array;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0000B614 File Offset: 0x00009814
		public override Type GetInterface(string name, bool ignoreCase)
		{
			return MetadataOnlyModule.GetInterfaceHelper(this.GetInterfaces(), name, ignoreCase);
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0000B634 File Offset: 0x00009834
		private static bool PropertyParamTypesMatch(PropertyInfo p, Type[] types)
		{
			bool flag = types == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				ParameterInfo[] indexParameters = p.GetIndexParameters();
				bool flag2 = indexParameters.Length != types.Length;
				if (flag2)
				{
					result = false;
				}
				else
				{
					int num = indexParameters.Length;
					for (int i = 0; i < num; i++)
					{
						bool flag3 = !indexParameters[i].ParameterType.Equals(types[i]);
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

		// Token: 0x06000337 RID: 823 RVA: 0x0000B6AC File Offset: 0x000098AC
		public override EventInfo[] GetEvents(BindingFlags flags)
		{
			return MetadataOnlyModule.GetEventsOnType(this, flags);
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0000B6C8 File Offset: 0x000098C8
		public override EventInfo GetEvent(string name, BindingFlags flags)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			StringComparison stringComparison = SignatureUtil.GetStringComparison(flags);
			EventInfo[] events = this.GetEvents(flags);
			foreach (EventInfo eventInfo in events)
			{
				bool flag2 = eventInfo.Name.Equals(name, stringComparison);
				if (flag2)
				{
					return eventInfo;
				}
			}
			return null;
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0000B738 File Offset: 0x00009938
		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			StringComparison stringComparison = SignatureUtil.GetStringComparison(bindingAttr);
			Type[] nestedTypes = this.GetNestedTypes(bindingAttr);
			foreach (Type type in nestedTypes)
			{
				bool flag2 = type.Name.Equals(name, stringComparison);
				if (flag2)
				{
					return type;
				}
			}
			return null;
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0000B7A8 File Offset: 0x000099A8
		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			List<Type> list = new List<Type>(this._resolver.GetNestedTypesOnType(this, bindingAttr));
			return list.ToArray();
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0000B7D4 File Offset: 0x000099D4
		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			MemberInfo[] members = this.GetMembers(bindingAttr);
			List<MemberInfo> list = new List<MemberInfo>();
			StringComparison stringComparison = SignatureUtil.GetStringComparison(bindingAttr);
			foreach (MemberInfo memberInfo in members)
			{
				bool flag = !name.Equals(memberInfo.Name, stringComparison);
				if (!flag)
				{
					bool flag2 = type != memberInfo.MemberType && type != MemberTypes.All;
					if (!flag2)
					{
						list.Add(memberInfo);
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0000B864 File Offset: 0x00009A64
		internal static MemberInfo[] GetMembersHelper(Type type, BindingFlags bindingAttr)
		{
			List<MemberInfo> list = new List<MemberInfo>(type.GetMethods(bindingAttr));
			list.AddRange(type.GetConstructors(bindingAttr));
			list.AddRange(type.GetFields(bindingAttr));
			list.AddRange(type.GetProperties(bindingAttr));
			list.AddRange(type.GetEvents(bindingAttr));
			list.AddRange(type.GetNestedTypes(bindingAttr));
			return list.ToArray();
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x0600033D RID: 829 RVA: 0x0000B8D0 File Offset: 0x00009AD0
		public override Guid GUID
		{
			get
			{
				IList<CustomAttributeData> customAttributesData = this.GetCustomAttributesData();
				foreach (CustomAttributeData customAttributeData in customAttributesData)
				{
					bool flag = customAttributeData.Constructor.DeclaringType.FullName.Equals("System.Runtime.InteropServices.GuidAttribute");
					if (flag)
					{
						string g = (string)customAttributeData.ConstructorArguments[0].Value;
						return new Guid(g);
					}
				}
				return Guid.Empty;
			}
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0000B970 File Offset: 0x00009B70
		protected override bool HasElementTypeImpl()
		{
			return false;
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0000240B File Offset: 0x0000060B
		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000340 RID: 832 RVA: 0x0000232A File Offset: 0x0000052A
		protected override bool IsCOMObjectImpl()
		{
			throw new NotImplementedException();
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000341 RID: 833 RVA: 0x0000B984 File Offset: 0x00009B84
		public override StructLayoutAttribute StructLayoutAttribute
		{
			get
			{
				bool isInterface = base.IsInterface;
				StructLayoutAttribute result;
				if (isInterface)
				{
					result = null;
				}
				else
				{
					uint size = 0U;
					uint num;
					uint classLayout = this._resolver.RawImport.GetClassLayout(this._tokenTypeDef, out num, UnusedIntPtr.Zero, 0U, UnusedIntPtr.Zero, ref size);
					bool flag = num == 0U;
					if (flag)
					{
						num = 8U;
					}
					TypeAttributes typeAttributes = this._typeAttributes & TypeAttributes.LayoutMask;
					LayoutKind layoutKind;
					if (typeAttributes != TypeAttributes.NotPublic)
					{
						if (typeAttributes != TypeAttributes.SequentialLayout)
						{
							if (typeAttributes != TypeAttributes.ExplicitLayout)
							{
								throw new InvalidOperationException(Resources.IllegalLayoutMask);
							}
							layoutKind = LayoutKind.Explicit;
						}
						else
						{
							layoutKind = LayoutKind.Sequential;
						}
					}
					else
					{
						layoutKind = LayoutKind.Auto;
					}
					CharSet charSet = CharSet.None;
					TypeAttributes typeAttributes2 = this._typeAttributes & TypeAttributes.StringFormatMask;
					if (typeAttributes2 != TypeAttributes.NotPublic)
					{
						if (typeAttributes2 != TypeAttributes.UnicodeClass)
						{
							if (typeAttributes2 == TypeAttributes.AutoClass)
							{
								charSet = CharSet.Auto;
							}
						}
						else
						{
							charSet = CharSet.Unicode;
						}
					}
					else
					{
						charSet = CharSet.Ansi;
					}
					result = new StructLayoutAttribute(layoutKind)
					{
						Size = (int)size,
						Pack = (int)num,
						CharSet = charSet
					};
				}
				return result;
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000342 RID: 834 RVA: 0x0000BA80 File Offset: 0x00009C80
		public override MemberTypes MemberType
		{
			get
			{
				bool isNested = base.IsNested;
				MemberTypes result;
				if (isNested)
				{
					result = MemberTypes.NestedType;
				}
				else
				{
					result = MemberTypes.TypeInfo;
				}
				return result;
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000343 RID: 835 RVA: 0x0000BAA8 File Offset: 0x00009CA8
		public override Type DeclaringType
		{
			get
			{
				Type enclosingType = this._resolver.GetEnclosingType(new Token(this.MetadataToken));
				bool flag = enclosingType != null;
				Type result;
				if (flag)
				{
					result = enclosingType;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000344 RID: 836 RVA: 0x0000BAE0 File Offset: 0x00009CE0
		public override string Name
		{
			get
			{
				return Utility.GetTypeNameFromFullNameHelper(this.LocalFullName, base.IsNested);
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000345 RID: 837 RVA: 0x0000BB04 File Offset: 0x00009D04
		public override Assembly Assembly
		{
			get
			{
				return this._resolver.Assembly;
			}
		}

		// Token: 0x06000346 RID: 838 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000347 RID: 839 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000348 RID: 840 RVA: 0x0000240B File Offset: 0x0000060B
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000349 RID: 841 RVA: 0x0000BB24 File Offset: 0x00009D24
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return this._typeAttributes;
		}

		// Token: 0x0600034A RID: 842 RVA: 0x0000BB3C File Offset: 0x00009D3C
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.Resolver.GetCustomAttributeData(this.MetadataToken);
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600034B RID: 843 RVA: 0x000037BB File Offset: 0x000019BB
		public override GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				throw new InvalidOperationException(Resources.ValidOnGenericParameterTypeOnly);
			}
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0000BB60 File Offset: 0x00009D60
		protected override TypeCode GetTypeCodeImpl()
		{
			return this._resolver.GetTypeCode(this);
		}

		// Token: 0x0600034D RID: 845 RVA: 0x0000BB80 File Offset: 0x00009D80
		internal override IEnumerable<PropertyInfo> GetDeclaredProperties()
		{
			return this.Resolver.GetPropertiesOnDeclaredTypeOnly(this._tokenTypeDef, this.GenericContext);
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0000BBAC File Offset: 0x00009DAC
		internal override IEnumerable<MethodBase> GetDeclaredMethods()
		{
			return this.Resolver.GetMethodBasesOnDeclaredTypeOnly(this._tokenTypeDef, this.GenericContext, MetadataOnlyModule.EMethodKind.Methods);
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0000BBD8 File Offset: 0x00009DD8
		internal override IEnumerable<MethodBase> GetDeclaredConstructors()
		{
			return this.Resolver.GetMethodBasesOnDeclaredTypeOnly(this._tokenTypeDef, this.GenericContext, MetadataOnlyModule.EMethodKind.Constructor);
		}

		// Token: 0x040000AB RID: 171
		private readonly MetadataOnlyModule _resolver;

		// Token: 0x040000AC RID: 172
		private readonly Token _tokenTypeDef;

		// Token: 0x040000AD RID: 173
		private readonly Type[] _typeParameters;

		// Token: 0x040000AE RID: 174
		private readonly Token _tokenExtends;

		// Token: 0x040000AF RID: 175
		private string _fullName;

		// Token: 0x040000B0 RID: 176
		private readonly int _nameLength;

		// Token: 0x040000B1 RID: 177
		private readonly TypeAttributes _typeAttributes;

		// Token: 0x040000B2 RID: 178
		private Type _baseType;

		// Token: 0x040000B3 RID: 179
		private MetadataOnlyTypeDef.TriState _fIsValueType = MetadataOnlyTypeDef.TriState.Maybe;

		// Token: 0x040000B4 RID: 180
		private static readonly string[] s_primitiveTypeNames = new string[]
		{
			"System.Boolean",
			"System.Char",
			"System.SByte",
			"System.Byte",
			"System.Int16",
			"System.UInt16",
			"System.Int32",
			"System.UInt32",
			"System.Int64",
			"System.UInt64",
			"System.Single",
			"System.Double",
			"System.IntPtr",
			"System.UIntPtr"
		};

		// Token: 0x02000063 RID: 99
		private enum TriState
		{
			// Token: 0x040001A0 RID: 416
			Yes,
			// Token: 0x040001A1 RID: 417
			No,
			// Token: 0x040001A2 RID: 418
			Maybe
		}
	}
}
