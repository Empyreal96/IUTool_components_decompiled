using System;
using System.Collections.Generic;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000035 RID: 53
	internal interface IMetadataExtensionsPolicy
	{
		// Token: 0x06000416 RID: 1046
		Type[] GetExtraArrayInterfaces(Type elementType);

		// Token: 0x06000417 RID: 1047
		MethodInfo[] GetExtraArrayMethods(Type arrayType);

		// Token: 0x06000418 RID: 1048
		ConstructorInfo[] GetExtraArrayConstructors(Type arrayType);

		// Token: 0x06000419 RID: 1049
		ParameterInfo GetFakeParameterInfo(MemberInfo member, Type paramType, int position);

		// Token: 0x0600041A RID: 1050
		IEnumerable<CustomAttributeData> GetPseudoCustomAttributes(MetadataOnlyModule module, Token token);

		// Token: 0x0600041B RID: 1051
		Type TryTypeForwardResolution(MetadataOnlyAssembly assembly, string fullname, bool ignoreCase);
	}
}
