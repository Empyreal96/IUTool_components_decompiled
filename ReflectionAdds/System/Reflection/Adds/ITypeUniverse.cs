using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Mock;

namespace System.Reflection.Adds
{
	// Token: 0x02000013 RID: 19
	internal interface ITypeUniverse
	{
		// Token: 0x06000085 RID: 133
		Type GetBuiltInType(CorElementType elementType);

		// Token: 0x06000086 RID: 134
		Type GetTypeXFromName(string fullName);

		// Token: 0x06000087 RID: 135
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		Assembly GetSystemAssembly();

		// Token: 0x06000088 RID: 136
		Assembly ResolveAssembly(AssemblyName name);

		// Token: 0x06000089 RID: 137
		Assembly ResolveAssembly(Module scope, Token tokenAssemblyRef);

		// Token: 0x0600008A RID: 138
		Module ResolveModule(Assembly containingAssembly, string moduleName);
	}
}
