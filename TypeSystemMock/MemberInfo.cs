using System;
using System.Collections.Generic;

namespace System.Reflection.Mock
{
	// Token: 0x0200000E RID: 14
	internal abstract class MemberInfo
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060000F4 RID: 244
		public abstract MemberTypes MemberType { get; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060000F5 RID: 245
		public abstract Type DeclaringType { get; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060000F6 RID: 246
		public abstract string Name { get; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060000F7 RID: 247
		public abstract int MetadataToken { get; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060000F8 RID: 248
		public abstract Module Module { get; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060000F9 RID: 249
		public abstract Type ReflectedType { get; }

		// Token: 0x060000FA RID: 250
		public abstract object[] GetCustomAttributes(bool inherit);

		// Token: 0x060000FB RID: 251
		public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);

		// Token: 0x060000FC RID: 252
		public abstract bool IsDefined(Type attributeType, bool inherit);

		// Token: 0x060000FD RID: 253
		public abstract IList<CustomAttributeData> GetCustomAttributesData();
	}
}
