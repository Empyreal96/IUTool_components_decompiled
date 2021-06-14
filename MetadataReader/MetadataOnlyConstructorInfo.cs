using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000023 RID: 35
	internal class MetadataOnlyConstructorInfo : ConstructorInfo
	{
		// Token: 0x060001BF RID: 447 RVA: 0x00004F4B File Offset: 0x0000314B
		public MetadataOnlyConstructorInfo(MethodBase method)
		{
			this._method = method;
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x00004F5C File Offset: 0x0000315C
		public override int MetadataToken
		{
			get
			{
				return this._method.MetadataToken;
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00004F7C File Offset: 0x0000317C
		public override string ToString()
		{
			return this._method.ToString();
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x00004F9C File Offset: 0x0000319C
		public override Module Module
		{
			get
			{
				return this._method.Module;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x00004FBC File Offset: 0x000031BC
		public override Type DeclaringType
		{
			get
			{
				return this._method.DeclaringType;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x00004FDC File Offset: 0x000031DC
		public override string Name
		{
			get
			{
				return this._method.Name;
			}
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000240B File Offset: 0x0000060B
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000240B File Offset: 0x0000060B
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x0000240B File Offset: 0x0000060B
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00004FFC File Offset: 0x000031FC
		public override ParameterInfo[] GetParameters()
		{
			return this._method.GetParameters();
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060001CA RID: 458 RVA: 0x0000501C File Offset: 0x0000321C
		public override MethodAttributes Attributes
		{
			get
			{
				return this._method.Attributes;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060001CB RID: 459 RVA: 0x0000503C File Offset: 0x0000323C
		public override CallingConventions CallingConvention
		{
			get
			{
				return this._method.CallingConvention;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060001CC RID: 460 RVA: 0x0000505C File Offset: 0x0000325C
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Constructor;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060001CD RID: 461 RVA: 0x00005070 File Offset: 0x00003270
		public override bool IsGenericMethodDefinition
		{
			get
			{
				return this._method.IsGenericMethodDefinition;
			}
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00005090 File Offset: 0x00003290
		public override MethodBody GetMethodBody()
		{
			return this._method.GetMethodBody();
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000240B File Offset: 0x0000060B
		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x000050B0 File Offset: 0x000032B0
		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return this._method.GetMethodImplementationFlags();
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060001D1 RID: 465 RVA: 0x0000240B File Offset: 0x0000060B
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000240B File Offset: 0x0000060B
		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x000050D0 File Offset: 0x000032D0
		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this._method.GetCustomAttributesData();
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x000050F0 File Offset: 0x000032F0
		public override bool Equals(object obj)
		{
			MetadataOnlyConstructorInfo metadataOnlyConstructorInfo = obj as MetadataOnlyConstructorInfo;
			bool flag = metadataOnlyConstructorInfo == null;
			return !flag && this._method.Equals(metadataOnlyConstructorInfo._method);
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00005128 File Offset: 0x00003328
		public override int GetHashCode()
		{
			return this._method.GetHashCode();
		}

		// Token: 0x04000059 RID: 89
		private readonly MethodBase _method;
	}
}
