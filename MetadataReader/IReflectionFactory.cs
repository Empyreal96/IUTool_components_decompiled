using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200000F RID: 15
	internal interface IReflectionFactory
	{
		// Token: 0x0600005F RID: 95
		MetadataOnlyCommonType CreateSimpleType(MetadataOnlyModule scope, Token tokenTypeDef);

		// Token: 0x06000060 RID: 96
		MetadataOnlyCommonType CreateGenericType(MetadataOnlyModule scope, Token tokenTypeDef, Type[] typeArgs);

		// Token: 0x06000061 RID: 97
		MetadataOnlyCommonType CreateArrayType(MetadataOnlyCommonType elementType, int rank);

		// Token: 0x06000062 RID: 98
		MetadataOnlyCommonType CreateVectorType(MetadataOnlyCommonType elementType);

		// Token: 0x06000063 RID: 99
		MetadataOnlyCommonType CreateByRefType(MetadataOnlyCommonType type);

		// Token: 0x06000064 RID: 100
		MetadataOnlyCommonType CreatePointerType(MetadataOnlyCommonType type);

		// Token: 0x06000065 RID: 101
		MetadataOnlyTypeVariable CreateTypeVariable(MetadataOnlyModule resolver, Token typeVariableToken);

		// Token: 0x06000066 RID: 102
		MetadataOnlyFieldInfo CreateField(MetadataOnlyModule resolver, Token fieldDefToken, Type[] typeArgs, Type[] methodArgs);

		// Token: 0x06000067 RID: 103
		MetadataOnlyPropertyInfo CreatePropertyInfo(MetadataOnlyModule resolver, Token propToken, Type[] typeArgs, Type[] methodArgs);

		// Token: 0x06000068 RID: 104
		MetadataOnlyEventInfo CreateEventInfo(MetadataOnlyModule resolver, Token eventToken, Type[] typeArgs, Type[] methodArgs);

		// Token: 0x06000069 RID: 105
		MethodBase CreateMethodOrConstructor(MetadataOnlyModule resolver, Token methodToken, Type[] typeArgs, Type[] methodArgs);

		// Token: 0x0600006A RID: 106
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#")]
		bool TryCreateMethodBody(MetadataOnlyMethodInfo method, ref MethodBody body);

		// Token: 0x0600006B RID: 107
		Type CreateTypeRef(MetadataOnlyModule scope, Token tokenTypeRef);

		// Token: 0x0600006C RID: 108
		Type CreateTypeSpec(MetadataOnlyModule scope, Token tokenTypeRef, Type[] typeArgs, Type[] methodArgs);
	}
}
