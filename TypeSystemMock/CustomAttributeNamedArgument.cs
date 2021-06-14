using System;

namespace System.Reflection.Mock
{
	// Token: 0x02000008 RID: 8
	internal struct CustomAttributeNamedArgument
	{
		// Token: 0x060000B9 RID: 185 RVA: 0x000032BE File Offset: 0x000014BE
		public CustomAttributeNamedArgument(MemberInfo memberInfo, CustomAttributeTypedArgument typedValue)
		{
			this.MemberInfo = memberInfo;
			this.TypedValue = typedValue;
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000BA RID: 186 RVA: 0x000032CF File Offset: 0x000014CF
		public MemberInfo MemberInfo { get; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000BB RID: 187 RVA: 0x000032D7 File Offset: 0x000014D7
		public CustomAttributeTypedArgument TypedValue { get; }
	}
}
