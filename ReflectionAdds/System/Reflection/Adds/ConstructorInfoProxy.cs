using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection.Mock;

namespace System.Reflection.Adds
{
	// Token: 0x02000005 RID: 5
	internal abstract class ConstructorInfoProxy : ConstructorInfo
	{
		// Token: 0x06000034 RID: 52
		protected abstract ConstructorInfo GetResolvedWorker();

		// Token: 0x06000035 RID: 53 RVA: 0x000026D8 File Offset: 0x000008D8
		public ConstructorInfo GetResolvedConstructor()
		{
			bool flag = this._cachedResolved == null;
			if (flag)
			{
				this._cachedResolved = this.GetResolvedWorker();
			}
			return this._cachedResolved;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000270C File Offset: 0x0000090C
		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return this.GetResolvedConstructor().Invoke(invokeAttr, binder, parameters, culture);
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002730 File Offset: 0x00000930
		public override MethodAttributes Attributes
		{
			get
			{
				return this.GetResolvedConstructor().Attributes;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002750 File Offset: 0x00000950
		public override CallingConventions CallingConvention
		{
			get
			{
				return this.GetResolvedConstructor().CallingConvention;
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002770 File Offset: 0x00000970
		public override ParameterInfo[] GetParameters()
		{
			return this.GetResolvedConstructor().GetParameters();
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002790 File Offset: 0x00000990
		public override bool IsGenericMethodDefinition
		{
			get
			{
				return this.GetResolvedConstructor().IsGenericMethodDefinition;
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000027B0 File Offset: 0x000009B0
		public override Type[] GetGenericArguments()
		{
			return this.GetResolvedConstructor().GetGenericArguments();
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003C RID: 60 RVA: 0x000027D0 File Offset: 0x000009D0
		public override bool ContainsGenericParameters
		{
			get
			{
				return this.GetResolvedConstructor().ContainsGenericParameters;
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000027F0 File Offset: 0x000009F0
		public override MethodBody GetMethodBody()
		{
			return this.GetResolvedConstructor().GetMethodBody();
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002810 File Offset: 0x00000A10
		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return this.GetResolvedConstructor().GetMethodImplementationFlags();
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002830 File Offset: 0x00000A30
		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return this.GetResolvedConstructor().Invoke(obj, invokeAttr, binder, parameters, culture);
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00002854 File Offset: 0x00000A54
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000041 RID: 65 RVA: 0x0000285C File Offset: 0x00000A5C
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Constructor;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00002870 File Offset: 0x00000A70
		public override Type DeclaringType
		{
			get
			{
				return this.GetResolvedConstructor().DeclaringType;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00002890 File Offset: 0x00000A90
		public override string Name
		{
			get
			{
				return this.GetResolvedConstructor().Name;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000044 RID: 68 RVA: 0x000028B0 File Offset: 0x00000AB0
		public override int MetadataToken
		{
			get
			{
				return this.GetResolvedConstructor().MetadataToken;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000028D0 File Offset: 0x00000AD0
		public override Module Module
		{
			get
			{
				return this.GetResolvedConstructor().Module;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000046 RID: 70 RVA: 0x000028F0 File Offset: 0x00000AF0
		public override Type ReflectedType
		{
			get
			{
				return this.GetResolvedConstructor().ReflectedType;
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002910 File Offset: 0x00000B10
		public override object[] GetCustomAttributes(bool inherit)
		{
			return this.GetResolvedConstructor().GetCustomAttributes(inherit);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002930 File Offset: 0x00000B30
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return this.GetResolvedConstructor().GetCustomAttributes(attributeType, inherit);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002950 File Offset: 0x00000B50
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return this.GetResolvedConstructor().IsDefined(attributeType, inherit);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002970 File Offset: 0x00000B70
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.GetResolvedConstructor().GetCustomAttributesData();
		}

		// Token: 0x04000004 RID: 4
		private ConstructorInfo _cachedResolved;
	}
}
