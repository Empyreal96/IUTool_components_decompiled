using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection.Mock
{
	// Token: 0x02000014 RID: 20
	[ComVisible(true)]
	internal abstract class PropertyInfo : MemberInfo
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x0600015B RID: 347
		public abstract PropertyAttributes Attributes { get; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x0600015C RID: 348
		public abstract Type PropertyType { get; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600015D RID: 349
		public abstract bool CanRead { get; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600015E RID: 350
		public abstract bool CanWrite { get; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600015F RID: 351 RVA: 0x00003BEC File Offset: 0x00001DEC
		public bool IsSpecialName
		{
			get
			{
				return (this.Attributes & PropertyAttributes.SpecialName) > PropertyAttributes.None;
			}
		}

		// Token: 0x06000160 RID: 352
		public abstract object GetConstantValue();

		// Token: 0x06000161 RID: 353 RVA: 0x00003C10 File Offset: 0x00001E10
		public MethodInfo[] GetAccessors()
		{
			return this.GetAccessors(false);
		}

		// Token: 0x06000162 RID: 354
		public abstract MethodInfo[] GetAccessors(bool nonPublic);

		// Token: 0x06000163 RID: 355 RVA: 0x00003C2C File Offset: 0x00001E2C
		public MethodInfo GetGetMethod()
		{
			return this.GetGetMethod(false);
		}

		// Token: 0x06000164 RID: 356
		public abstract MethodInfo GetGetMethod(bool nonPublic);

		// Token: 0x06000165 RID: 357 RVA: 0x00003C48 File Offset: 0x00001E48
		public MethodInfo GetSetMethod()
		{
			return this.GetSetMethod(false);
		}

		// Token: 0x06000166 RID: 358
		public abstract MethodInfo GetSetMethod(bool nonPublic);

		// Token: 0x06000167 RID: 359
		public abstract ParameterInfo[] GetIndexParameters();

		// Token: 0x06000168 RID: 360
		public abstract object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);

		// Token: 0x06000169 RID: 361
		public abstract void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);
	}
}
