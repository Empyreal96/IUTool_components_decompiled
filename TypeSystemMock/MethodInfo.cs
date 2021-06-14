using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Mock
{
	// Token: 0x0200000F RID: 15
	[ComVisible(true)]
	internal abstract class MethodInfo : MethodBase
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060000FF RID: 255
		public abstract Type ReturnType { get; }

		// Token: 0x06000100 RID: 256
		public abstract MethodInfo MakeGenericMethod(params Type[] types);

		// Token: 0x06000101 RID: 257 RVA: 0x000020E5 File Offset: 0x000002E5
		public virtual MethodInfo GetGenericMethodDefinition()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000102 RID: 258
		public abstract ICustomAttributeProvider ReturnTypeCustomAttributes { get; }

		// Token: 0x06000103 RID: 259
		public abstract MethodInfo GetBaseDefinition();

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000104 RID: 260
		public abstract ParameterInfo ReturnParameter { get; }
	}
}
