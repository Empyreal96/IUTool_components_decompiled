using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000025 RID: 37
	internal class MetadataOnlyEventInfo : EventInfo
	{
		// Token: 0x060001DC RID: 476 RVA: 0x00005234 File Offset: 0x00003434
		public MetadataOnlyEventInfo(MetadataOnlyModule resolver, Token eventToken, Type[] typeArgs, Type[] methodArgs)
		{
			this._resolver = resolver;
			this._eventToken = eventToken;
			this._context = new GenericContext(typeArgs, methodArgs);
			IMetadataImport rawImport = this._resolver.RawImport;
			int num;
			int value;
			int value2;
			int value3;
			int num2;
			uint num3;
			rawImport.GetEventProps(this._eventToken, out this._declaringClassToken, null, 0, out this._nameLength, out num, out this._eventHandlerTypeToken, out value, out value2, out value3, out num2, 1U, out num3);
			this.Attributes = num;
			this._addMethodToken = new Token(value);
			this._removeMethodToken = new Token(value2);
			this._raiseMethodToken = new Token(value3);
		}

		// Token: 0x060001DD RID: 477 RVA: 0x000052D0 File Offset: 0x000034D0
		public override string ToString()
		{
			return this.DeclaringType + "." + this.Name;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x000052F8 File Offset: 0x000034F8
		private void InitializeName()
		{
			bool flag = string.IsNullOrEmpty(this._name);
			if (flag)
			{
				IMetadataImport rawImport = this._resolver.RawImport;
				StringBuilder stringBuilder = StringBuilderPool.Get(this._nameLength);
				int num;
				int num2;
				int num3;
				int num4;
				int num5;
				int num6;
				uint num7;
				rawImport.GetEventProps(this._eventToken, out this._declaringClassToken, stringBuilder, stringBuilder.Capacity, out num, out num2, out this._eventHandlerTypeToken, out num3, out num4, out num5, out num6, 1U, out num7);
				this._name = stringBuilder.ToString();
				StringBuilderPool.Release(ref stringBuilder);
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060001DF RID: 479 RVA: 0x00005376 File Offset: 0x00003576
		public override EventAttributes Attributes { get; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x00005380 File Offset: 0x00003580
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Event;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x00005394 File Offset: 0x00003594
		public override string Name
		{
			get
			{
				this.InitializeName();
				return this._name;
			}
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000240B File Offset: 0x0000060B
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060001E5 RID: 485 RVA: 0x0000240B File Offset: 0x0000060B
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x000053B4 File Offset: 0x000035B4
		public override Type EventHandlerType
		{
			get
			{
				return this._resolver.GetGenericType(new Token(this._eventHandlerTypeToken), this._context);
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001E7 RID: 487 RVA: 0x000053E4 File Offset: 0x000035E4
		public override Type DeclaringType
		{
			get
			{
				return this._resolver.GetGenericType(new Token(this._declaringClassToken), this._context);
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001E8 RID: 488 RVA: 0x00005414 File Offset: 0x00003614
		public override int MetadataToken
		{
			get
			{
				return this._eventToken;
			}
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000542C File Offset: 0x0000362C
		public override MethodInfo GetAddMethod(bool nonPublic)
		{
			bool isNil = this._addMethodToken.IsNil;
			MethodInfo result;
			if (isNil)
			{
				result = null;
			}
			else
			{
				MethodInfo genericMethodInfo = this._resolver.GetGenericMethodInfo(this._addMethodToken, this._context);
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

		// Token: 0x060001EA RID: 490 RVA: 0x00005480 File Offset: 0x00003680
		public override MethodInfo GetRemoveMethod(bool nonPublic)
		{
			bool isNil = this._removeMethodToken.IsNil;
			MethodInfo result;
			if (isNil)
			{
				result = null;
			}
			else
			{
				MethodInfo genericMethodInfo = this._resolver.GetGenericMethodInfo(this._removeMethodToken, this._context);
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

		// Token: 0x060001EB RID: 491 RVA: 0x000054D4 File Offset: 0x000036D4
		public override MethodInfo GetRaiseMethod(bool nonPublic)
		{
			bool isNil = this._raiseMethodToken.IsNil;
			MethodInfo result;
			if (isNil)
			{
				result = null;
			}
			else
			{
				MethodInfo genericMethodInfo = this._resolver.GetGenericMethodInfo(this._raiseMethodToken, this._context);
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

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001EC RID: 492 RVA: 0x00005528 File Offset: 0x00003728
		public override Module Module
		{
			get
			{
				return this._resolver;
			}
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00005540 File Offset: 0x00003740
		public override bool Equals(object obj)
		{
			MetadataOnlyEventInfo metadataOnlyEventInfo = obj as MetadataOnlyEventInfo;
			bool flag = metadataOnlyEventInfo != null;
			return flag && (metadataOnlyEventInfo._resolver.Equals(this._resolver) && metadataOnlyEventInfo._eventToken.Equals(this._eventToken)) && this.DeclaringType.Equals(metadataOnlyEventInfo.DeclaringType);
		}

		// Token: 0x060001EE RID: 494 RVA: 0x000055A4 File Offset: 0x000037A4
		public override int GetHashCode()
		{
			return this._resolver.GetHashCode() * 32767 + this._eventToken.GetHashCode();
		}

		// Token: 0x060001EF RID: 495 RVA: 0x000055D8 File Offset: 0x000037D8
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this._resolver.GetCustomAttributeData(this.MetadataToken);
		}

		// Token: 0x04000060 RID: 96
		private readonly MetadataOnlyModule _resolver;

		// Token: 0x04000061 RID: 97
		private readonly int _eventToken;

		// Token: 0x04000062 RID: 98
		private int _declaringClassToken;

		// Token: 0x04000063 RID: 99
		private int _eventHandlerTypeToken;

		// Token: 0x04000064 RID: 100
		private readonly GenericContext _context;

		// Token: 0x04000065 RID: 101
		private string _name;

		// Token: 0x04000066 RID: 102
		private readonly int _nameLength;

		// Token: 0x04000067 RID: 103
		private Token _addMethodToken;

		// Token: 0x04000068 RID: 104
		private Token _removeMethodToken;

		// Token: 0x04000069 RID: 105
		private Token _raiseMethodToken;
	}
}
