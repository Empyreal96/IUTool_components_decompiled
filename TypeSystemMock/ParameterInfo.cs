using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Reflection.Mock
{
	// Token: 0x02000013 RID: 19
	[ComVisible(true)]
	internal abstract class ParameterInfo
	{
		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600014A RID: 330
		public abstract ParameterAttributes Attributes { get; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600014B RID: 331
		public abstract string Name { get; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600014C RID: 332
		public abstract int Position { get; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600014D RID: 333
		public abstract Type ParameterType { get; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600014E RID: 334 RVA: 0x00003B38 File Offset: 0x00001D38
		public virtual int MetadataToken
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600014F RID: 335
		public abstract MemberInfo Member { get; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000150 RID: 336 RVA: 0x00003B4C File Offset: 0x00001D4C
		public bool IsIn
		{
			get
			{
				return (this.Attributes & ParameterAttributes.In) > ParameterAttributes.None;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000151 RID: 337 RVA: 0x00003B6C File Offset: 0x00001D6C
		public bool IsLcid
		{
			get
			{
				return (this.Attributes & ParameterAttributes.Lcid) > ParameterAttributes.None;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00003B8C File Offset: 0x00001D8C
		public bool IsOptional
		{
			get
			{
				return (this.Attributes & ParameterAttributes.Optional) > ParameterAttributes.None;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000153 RID: 339 RVA: 0x00003BAC File Offset: 0x00001DAC
		public bool IsOut
		{
			get
			{
				return (this.Attributes & ParameterAttributes.Out) > ParameterAttributes.None;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00003BCC File Offset: 0x00001DCC
		public bool IsRetval
		{
			get
			{
				return (this.Attributes & ParameterAttributes.Retval) > ParameterAttributes.None;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000155 RID: 341
		public abstract object DefaultValue { get; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000156 RID: 342
		public abstract object RawDefaultValue { get; }

		// Token: 0x06000157 RID: 343
		public abstract IList<CustomAttributeData> GetCustomAttributesData();

		// Token: 0x06000158 RID: 344
		public abstract Type[] GetOptionalCustomModifiers();

		// Token: 0x06000159 RID: 345
		public abstract Type[] GetRequiredCustomModifiers();
	}
}
