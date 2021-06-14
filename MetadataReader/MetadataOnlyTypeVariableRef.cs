using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000030 RID: 48
	internal class MetadataOnlyTypeVariableRef : MetadataOnlyCommonType
	{
		// Token: 0x06000351 RID: 849 RVA: 0x0000BC92 File Offset: 0x00009E92
		internal MetadataOnlyTypeVariableRef(MetadataOnlyModule resolver, Token ownerToken, int position)
		{
			this._resolver = resolver;
			this._ownerToken = ownerToken;
			this._position = position;
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000352 RID: 850 RVA: 0x0000BCB4 File Offset: 0x00009EB4
		private bool IsMethodVar
		{
			get
			{
				return this._ownerToken.IsType(TokenType.MemberRef) || this._ownerToken.IsType(TokenType.MethodDef);
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000353 RID: 851 RVA: 0x0000BCF4 File Offset: 0x00009EF4
		public override string FullName
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000354 RID: 852 RVA: 0x0000BD08 File Offset: 0x00009F08
		internal override MetadataOnlyModule Resolver
		{
			get
			{
				return this._resolver;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000355 RID: 853 RVA: 0x00002390 File Offset: 0x00000590
		public override Type BaseType
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0000BD20 File Offset: 0x00009F20
		public override bool Equals(Type other)
		{
			MetadataOnlyTypeVariableRef metadataOnlyTypeVariableRef = other as MetadataOnlyTypeVariableRef;
			bool flag = metadataOnlyTypeVariableRef != null;
			bool result;
			if (flag)
			{
				result = (this.Resolver.Equals(metadataOnlyTypeVariableRef.Resolver) && this._ownerToken.Value == metadataOnlyTypeVariableRef._ownerToken.Value && this._position == metadataOnlyTypeVariableRef._position);
			}
			else
			{
				bool isGenericParameter = other.IsGenericParameter;
				if (isGenericParameter)
				{
					bool flag2 = this.IsMethodVar == (other.DeclaringMethod != null);
					result = (this._position == other.GenericParameterPosition && flag2);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06000357 RID: 855 RVA: 0x00002390 File Offset: 0x00000590
		public override bool IsAssignableFrom(Type c)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000358 RID: 856 RVA: 0x00002390 File Offset: 0x00000590
		public override Type UnderlyingSystemType
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x06000359 RID: 857 RVA: 0x00002390 File Offset: 0x00000590
		public override Type GetElementType()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600035A RID: 858 RVA: 0x00002390 File Offset: 0x00000590
		public override int MetadataToken
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x0600035B RID: 859 RVA: 0x00002390 File Offset: 0x00000590
		public override MethodInfo[] GetMethods(BindingFlags flags)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600035C RID: 860 RVA: 0x00002390 File Offset: 0x00000590
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600035D RID: 861 RVA: 0x00002390 File Offset: 0x00000590
		public override FieldInfo[] GetFields(BindingFlags flags)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600035E RID: 862 RVA: 0x00002390 File Offset: 0x00000590
		public override FieldInfo GetField(string name, BindingFlags flags)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600035F RID: 863 RVA: 0x00002390 File Offset: 0x00000590
		public override PropertyInfo[] GetProperties(BindingFlags flags)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000360 RID: 864 RVA: 0x00002390 File Offset: 0x00000590
		public override EventInfo[] GetEvents(BindingFlags flags)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000361 RID: 865 RVA: 0x00002390 File Offset: 0x00000590
		public override EventInfo GetEvent(string name, BindingFlags flags)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00002390 File Offset: 0x00000590
		public override Type MakeGenericType(params Type[] argTypes)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00002390 File Offset: 0x00000590
		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00002390 File Offset: 0x00000590
		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000365 RID: 869 RVA: 0x00002390 File Offset: 0x00000590
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000366 RID: 870 RVA: 0x00002390 File Offset: 0x00000590
		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000367 RID: 871 RVA: 0x00002390 File Offset: 0x00000590
		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000368 RID: 872 RVA: 0x0000BDBC File Offset: 0x00009FBC
		public override bool IsGenericParameter
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000369 RID: 873 RVA: 0x00002390 File Offset: 0x00000590
		public override Type[] GetGenericArguments()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00002390 File Offset: 0x00000590
		public override Type[] GetGenericParameterConstraints()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00002390 File Offset: 0x00000590
		public override Type GetGenericTypeDefinition()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600036C RID: 876 RVA: 0x00002390 File Offset: 0x00000590
		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00002390 File Offset: 0x00000590
		public override Type[] GetInterfaces()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600036E RID: 878 RVA: 0x00002390 File Offset: 0x00000590
		public override Type GetInterface(string name, bool ignoreCase)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x0600036F RID: 879 RVA: 0x00002390 File Offset: 0x00000590
		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000370 RID: 880 RVA: 0x00002390 File Offset: 0x00000590
		public override Guid GUID
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x06000371 RID: 881 RVA: 0x00002390 File Offset: 0x00000590
		protected override bool HasElementTypeImpl()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0000240B File Offset: 0x0000060B
		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000373 RID: 883 RVA: 0x00002390 File Offset: 0x00000590
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000374 RID: 884 RVA: 0x00002390 File Offset: 0x00000590
		public override GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000375 RID: 885 RVA: 0x0000BDD0 File Offset: 0x00009FD0
		public override int GenericParameterPosition
		{
			get
			{
				return this._position;
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000376 RID: 886 RVA: 0x0000BDE8 File Offset: 0x00009FE8
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.TypeInfo;
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000377 RID: 887 RVA: 0x0000BDFC File Offset: 0x00009FFC
		public override Type DeclaringType
		{
			get
			{
				bool flag = !this.IsMethodVar;
				Type result;
				if (flag)
				{
					bool flag2 = this._ownerToken.IsType(TokenType.TypeDef);
					if (flag2)
					{
						result = this._resolver.Factory.CreateSimpleType(this._resolver, this._ownerToken);
					}
					else
					{
						result = this._resolver.Factory.CreateTypeRef(this._resolver, this._ownerToken);
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000378 RID: 888 RVA: 0x0000BE74 File Offset: 0x0000A074
		public override MethodBase DeclaringMethod
		{
			get
			{
				bool isMethodVar = this.IsMethodVar;
				MethodBase result;
				if (isMethodVar)
				{
					result = this._resolver.Factory.CreateMethodOrConstructor(this._resolver, this._ownerToken, null, null);
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000379 RID: 889 RVA: 0x0000BEB4 File Offset: 0x0000A0B4
		public override string Name
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x0600037A RID: 890 RVA: 0x0000BEC8 File Offset: 0x0000A0C8
		public override string Namespace
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x0600037B RID: 891 RVA: 0x0000BEDC File Offset: 0x0000A0DC
		public override Assembly Assembly
		{
			get
			{
				return this._resolver.Assembly;
			}
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0000240B File Offset: 0x0000060B
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x0600037F RID: 895 RVA: 0x0000240B File Offset: 0x0000060B
		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0000BEFC File Offset: 0x0000A0FC
		public override string ToString()
		{
			bool isMethodVar = this.IsMethodVar;
			string result;
			if (isMethodVar)
			{
				result = "MVar!!" + this.GenericParameterPosition.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				result = "Var!" + this.GenericParameterPosition.ToString(CultureInfo.InvariantCulture);
			}
			return result;
		}

		// Token: 0x06000381 RID: 897 RVA: 0x00002390 File Offset: 0x00000590
		protected override TypeCode GetTypeCodeImpl()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x040000B5 RID: 181
		private readonly MetadataOnlyModule _resolver;

		// Token: 0x040000B6 RID: 182
		private readonly Token _ownerToken;

		// Token: 0x040000B7 RID: 183
		private readonly int _position;
	}
}
