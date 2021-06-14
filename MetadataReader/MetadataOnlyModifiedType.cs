using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200002B RID: 43
	internal class MetadataOnlyModifiedType : MetadataOnlyCommonType
	{
		// Token: 0x0600024A RID: 586 RVA: 0x0000714B File Offset: 0x0000534B
		public MetadataOnlyModifiedType(MetadataOnlyCommonType type, string mod)
		{
			this._type = type;
			this._mod = mod;
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600024B RID: 587 RVA: 0x00007164 File Offset: 0x00005364
		public override string FullName
		{
			get
			{
				string fullName = this._type.FullName;
				bool flag = fullName == null || this._type.IsGenericTypeDefinition;
				string result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = fullName + this._mod;
				}
				return result;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600024C RID: 588 RVA: 0x000071A8 File Offset: 0x000053A8
		internal override MetadataOnlyModule Resolver
		{
			get
			{
				return this._type.Resolver;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600024D RID: 589 RVA: 0x000071C8 File Offset: 0x000053C8
		public override Type BaseType
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600024E RID: 590 RVA: 0x000071DC File Offset: 0x000053DC
		public override bool Equals(Type t)
		{
			bool flag = t == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool isByRef = base.IsByRef;
				if (isByRef)
				{
					bool flag2 = !t.IsByRef;
					if (flag2)
					{
						return false;
					}
				}
				else
				{
					bool isPointer = base.IsPointer;
					if (isPointer)
					{
						bool flag3 = !t.IsPointer;
						if (flag3)
						{
							return false;
						}
					}
				}
				Type elementType = t.GetElementType();
				result = this._type.Equals(elementType);
			}
			return result;
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00007254 File Offset: 0x00005454
		protected override bool IsByRefImpl()
		{
			return this._mod.Equals("&");
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00007278 File Offset: 0x00005478
		protected override bool IsPointerImpl()
		{
			return this._mod.Equals("*");
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000729C File Offset: 0x0000549C
		public override bool IsAssignableFrom(Type c)
		{
			bool flag = c == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = (base.IsPointer && c.IsPointer) || (base.IsByRef && c.IsByRef);
				if (flag2)
				{
					Type elementType = c.GetElementType();
					bool flag3 = this._type.IsAssignableFrom(elementType) && !elementType.IsValueType;
					if (flag3)
					{
						return true;
					}
				}
				result = MetadataOnlyTypeDef.IsAssignableFromHelper(this, c);
			}
			return result;
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000252 RID: 594 RVA: 0x00007318 File Offset: 0x00005518
		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000732C File Offset: 0x0000552C
		public override Type GetElementType()
		{
			return this._type;
		}

		// Token: 0x06000254 RID: 596 RVA: 0x00007344 File Offset: 0x00005544
		public override MethodInfo[] GetMethods(BindingFlags flags)
		{
			return new MethodInfo[0];
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000735C File Offset: 0x0000555C
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			return new ConstructorInfo[0];
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00007374 File Offset: 0x00005574
		public override FieldInfo[] GetFields(BindingFlags flags)
		{
			return new FieldInfo[0];
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000738C File Offset: 0x0000558C
		public override PropertyInfo[] GetProperties(BindingFlags flags)
		{
			return new PropertyInfo[0];
		}

		// Token: 0x06000258 RID: 600 RVA: 0x000073A4 File Offset: 0x000055A4
		public override EventInfo[] GetEvents(BindingFlags flags)
		{
			return new EventInfo[0];
		}

		// Token: 0x06000259 RID: 601 RVA: 0x000073BC File Offset: 0x000055BC
		public override EventInfo GetEvent(string name, BindingFlags flags)
		{
			return null;
		}

		// Token: 0x0600025A RID: 602 RVA: 0x000073D0 File Offset: 0x000055D0
		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			return null;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x000073E4 File Offset: 0x000055E4
		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			return null;
		}

		// Token: 0x0600025C RID: 604 RVA: 0x000073F8 File Offset: 0x000055F8
		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			return Type.EmptyTypes;
		}

		// Token: 0x0600025D RID: 605 RVA: 0x00007410 File Offset: 0x00005610
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return TypeAttributes.NotPublic;
		}

		// Token: 0x0600025E RID: 606 RVA: 0x00007424 File Offset: 0x00005624
		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return null;
		}

		// Token: 0x0600025F RID: 607 RVA: 0x00007438 File Offset: 0x00005638
		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return null;
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000744C File Offset: 0x0000564C
		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return null;
		}

		// Token: 0x06000261 RID: 609 RVA: 0x00007460 File Offset: 0x00005660
		public override Type[] GetInterfaces()
		{
			return new Type[0];
		}

		// Token: 0x06000262 RID: 610 RVA: 0x00007478 File Offset: 0x00005678
		public override Type GetInterface(string name, bool ignoreCase)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			return null;
		}

		// Token: 0x06000263 RID: 611 RVA: 0x000074A0 File Offset: 0x000056A0
		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			return MetadataOnlyTypeDef.GetMembersHelper(this, bindingAttr);
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000264 RID: 612 RVA: 0x000074BC File Offset: 0x000056BC
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override Guid GUID
		{
			get
			{
				return Guid.Empty;
			}
		}

		// Token: 0x06000265 RID: 613 RVA: 0x000074D4 File Offset: 0x000056D4
		protected override bool HasElementTypeImpl()
		{
			return true;
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000240B File Offset: 0x0000060B
		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000267 RID: 615 RVA: 0x000074E8 File Offset: 0x000056E8
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return new CustomAttributeData[0];
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00007500 File Offset: 0x00005700
		public override string ToString()
		{
			return this._type + this._mod;
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000269 RID: 617 RVA: 0x00007524 File Offset: 0x00005724
		public override int MetadataToken
		{
			get
			{
				return 33554432;
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000753C File Offset: 0x0000573C
		public override Type[] GetGenericArguments()
		{
			return this._type.GetGenericArguments();
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00002390 File Offset: 0x00000590
		public override Type GetGenericTypeDefinition()
		{
			throw new InvalidOperationException();
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x0600026C RID: 620 RVA: 0x000037BB File Offset: 0x000019BB
		public override GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				throw new InvalidOperationException(Resources.ValidOnGenericParameterTypeOnly);
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x0600026D RID: 621 RVA: 0x0000755C File Offset: 0x0000575C
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.TypeInfo;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600026E RID: 622 RVA: 0x00007570 File Offset: 0x00005770
		public override Type DeclaringType
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600026F RID: 623 RVA: 0x00007584 File Offset: 0x00005784
		public override string Name
		{
			get
			{
				return this._type.Name + this._mod;
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000270 RID: 624 RVA: 0x000075AC File Offset: 0x000057AC
		public override Assembly Assembly
		{
			get
			{
				return this._type.Assembly;
			}
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000240B File Offset: 0x0000060B
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000274 RID: 628 RVA: 0x0000240B File Offset: 0x0000060B
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000275 RID: 629 RVA: 0x000075CC File Offset: 0x000057CC
		public override string Namespace
		{
			get
			{
				return this._type.Namespace;
			}
		}

		// Token: 0x06000276 RID: 630 RVA: 0x000075EC File Offset: 0x000057EC
		protected override TypeCode GetTypeCodeImpl()
		{
			return TypeCode.Object;
		}

		// Token: 0x04000089 RID: 137
		private readonly MetadataOnlyCommonType _type;

		// Token: 0x0400008A RID: 138
		private readonly string _mod;
	}
}
