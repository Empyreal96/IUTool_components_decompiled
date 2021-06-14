using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Mock;

namespace System.Reflection.Adds
{
	// Token: 0x02000010 RID: 16
	internal interface ITypeProxy
	{
		// Token: 0x06000080 RID: 128
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		Type GetResolvedType();

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000081 RID: 129
		ITypeUniverse TypeUniverse { get; }
	}
}
