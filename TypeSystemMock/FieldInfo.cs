using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection.Mock
{
	// Token: 0x0200000C RID: 12
	[ComVisible(true)]
	internal abstract class FieldInfo : MemberInfo
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000DA RID: 218
		public abstract FieldAttributes Attributes { get; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000DB RID: 219
		public abstract Type FieldType { get; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000DC RID: 220 RVA: 0x00003524 File Offset: 0x00001724
		public bool IsPublic
		{
			get
			{
				return (this.Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000DD RID: 221 RVA: 0x00003544 File Offset: 0x00001744
		public bool IsPrivate
		{
			get
			{
				return (this.Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Private;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000DE RID: 222 RVA: 0x00003564 File Offset: 0x00001764
		public bool IsFamily
		{
			get
			{
				return (this.Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Family;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000DF RID: 223 RVA: 0x00003584 File Offset: 0x00001784
		public bool IsAssembly
		{
			get
			{
				return (this.Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Assembly;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x000035A4 File Offset: 0x000017A4
		public bool IsFamilyAndAssembly
		{
			get
			{
				return (this.Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.FamANDAssem;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x000035C4 File Offset: 0x000017C4
		public bool IsFamilyOrAssembly
		{
			get
			{
				return (this.Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.FamORAssem;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x000035E4 File Offset: 0x000017E4
		public bool IsStatic
		{
			get
			{
				return (this.Attributes & FieldAttributes.Static) > FieldAttributes.PrivateScope;
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00003604 File Offset: 0x00001804
		public bool IsInitOnly
		{
			get
			{
				return (this.Attributes & FieldAttributes.InitOnly) > FieldAttributes.PrivateScope;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00003624 File Offset: 0x00001824
		public bool IsLiteral
		{
			get
			{
				return (this.Attributes & FieldAttributes.Literal) > FieldAttributes.PrivateScope;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060000E5 RID: 229 RVA: 0x00003644 File Offset: 0x00001844
		public bool IsNotSerialized
		{
			get
			{
				return (this.Attributes & FieldAttributes.NotSerialized) > FieldAttributes.PrivateScope;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00003668 File Offset: 0x00001868
		public bool IsSpecialName
		{
			get
			{
				return (this.Attributes & FieldAttributes.SpecialName) > FieldAttributes.PrivateScope;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x0000368C File Offset: 0x0000188C
		public bool IsPinvokeImpl
		{
			get
			{
				return (this.Attributes & FieldAttributes.PinvokeImpl) > FieldAttributes.PrivateScope;
			}
		}

		// Token: 0x060000E8 RID: 232
		public abstract object GetValue(object obj);

		// Token: 0x060000E9 RID: 233
		public abstract object GetRawConstantValue();

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060000EA RID: 234
		public abstract RuntimeFieldHandle FieldHandle { get; }

		// Token: 0x060000EB RID: 235
		public abstract void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture);

		// Token: 0x060000EC RID: 236
		public abstract Type[] GetOptionalCustomModifiers();

		// Token: 0x060000ED RID: 237
		public abstract Type[] GetRequiredCustomModifiers();
	}
}
