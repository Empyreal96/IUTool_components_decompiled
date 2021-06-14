using System;

namespace System.Reflection.Mock
{
	// Token: 0x02000009 RID: 9
	internal struct CustomAttributeTypedArgument
	{
		// Token: 0x060000BC RID: 188 RVA: 0x000032DF File Offset: 0x000014DF
		public CustomAttributeTypedArgument(Type argumentType, object value)
		{
			this.ArgumentType = argumentType;
			this.Value = value;
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000BD RID: 189 RVA: 0x000032F0 File Offset: 0x000014F0
		public Type ArgumentType { get; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000BE RID: 190 RVA: 0x000032F8 File Offset: 0x000014F8
		public object Value { get; }
	}
}
