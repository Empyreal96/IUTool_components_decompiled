using System;
using System.Reflection.Mock;

namespace System.Reflection.Adds
{
	// Token: 0x02000021 RID: 33
	internal interface ITypeReference : ITypeProxy
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000115 RID: 277
		Token TypeRefToken { get; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000116 RID: 278
		string RawName { get; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000117 RID: 279
		Token ResolutionScope { get; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000118 RID: 280
		string FullName { get; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000119 RID: 281
		Module DeclaringScope { get; }
	}
}
