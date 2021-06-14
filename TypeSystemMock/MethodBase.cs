using System;
using System.Globalization;

namespace System.Reflection.Mock
{
	// Token: 0x02000010 RID: 16
	internal abstract class MethodBase : MemberInfo
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000106 RID: 262
		public abstract MethodAttributes Attributes { get; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000107 RID: 263
		public abstract CallingConventions CallingConvention { get; }

		// Token: 0x06000108 RID: 264
		public abstract ParameterInfo[] GetParameters();

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000109 RID: 265 RVA: 0x00003714 File Offset: 0x00001914
		public bool IsPublic
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600010A RID: 266 RVA: 0x00003734 File Offset: 0x00001934
		public bool IsPrivate
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600010B RID: 267 RVA: 0x00003754 File Offset: 0x00001954
		public bool IsFamily
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Family;
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00003774 File Offset: 0x00001974
		public bool IsAssembly
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Assembly;
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600010D RID: 269 RVA: 0x00003794 File Offset: 0x00001994
		public bool IsFamilyAndAssembly
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamANDAssem;
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600010E RID: 270 RVA: 0x000037B4 File Offset: 0x000019B4
		public bool IsFamilyOrAssembly
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamORAssem;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600010F RID: 271 RVA: 0x000037D4 File Offset: 0x000019D4
		public bool IsStatic
		{
			get
			{
				return (this.Attributes & MethodAttributes.Static) > MethodAttributes.PrivateScope;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000110 RID: 272 RVA: 0x000037F4 File Offset: 0x000019F4
		public bool IsFinal
		{
			get
			{
				return (this.Attributes & MethodAttributes.Final) > MethodAttributes.PrivateScope;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000111 RID: 273 RVA: 0x00003814 File Offset: 0x00001A14
		public bool IsVirtual
		{
			get
			{
				return (this.Attributes & MethodAttributes.Virtual) > MethodAttributes.PrivateScope;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000112 RID: 274 RVA: 0x00003834 File Offset: 0x00001A34
		public bool IsHideBySig
		{
			get
			{
				return (this.Attributes & MethodAttributes.HideBySig) > MethodAttributes.PrivateScope;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00003858 File Offset: 0x00001A58
		public bool IsAbstract
		{
			get
			{
				return (this.Attributes & MethodAttributes.Abstract) > MethodAttributes.PrivateScope;
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000114 RID: 276 RVA: 0x0000387C File Offset: 0x00001A7C
		public bool IsSpecialName
		{
			get
			{
				return (this.Attributes & MethodAttributes.SpecialName) > MethodAttributes.PrivateScope;
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000115 RID: 277 RVA: 0x000038A0 File Offset: 0x00001AA0
		public bool IsConstructor
		{
			get
			{
				return this is ConstructorInfo && !this.IsStatic && (this.Attributes & MethodAttributes.RTSpecialName) == MethodAttributes.RTSpecialName;
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000116 RID: 278
		public abstract bool IsGenericMethodDefinition { get; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000117 RID: 279 RVA: 0x000038D8 File Offset: 0x00001AD8
		public virtual bool IsGenericMethod
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000027FE File Offset: 0x000009FE
		public virtual Type[] GetGenericArguments()
		{
			throw new NotSupportedException("NotSupported_SubclassOverride");
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000119 RID: 281 RVA: 0x000038EC File Offset: 0x00001AEC
		public virtual bool ContainsGenericParameters
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600011A RID: 282
		public abstract MethodBody GetMethodBody();

		// Token: 0x0600011B RID: 283
		public abstract MethodImplAttributes GetMethodImplementationFlags();

		// Token: 0x0600011C RID: 284
		public abstract object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture);

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600011D RID: 285
		public abstract RuntimeMethodHandle MethodHandle { get; }
	}
}
