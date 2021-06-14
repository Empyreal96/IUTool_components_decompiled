using System;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200000D RID: 13
	internal class DefaultFactory : IReflectionFactory
	{
		// Token: 0x06000046 RID: 70 RVA: 0x000029A8 File Offset: 0x00000BA8
		public virtual MetadataOnlyCommonType CreateSimpleType(MetadataOnlyModule scope, Token tokenTypeDef)
		{
			return new MetadataOnlyTypeDef(scope, tokenTypeDef);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000029C4 File Offset: 0x00000BC4
		public virtual MetadataOnlyCommonType CreateGenericType(MetadataOnlyModule scope, Token tokenTypeDef, Type[] typeArgs)
		{
			return new MetadataOnlyTypeDef(scope, tokenTypeDef, typeArgs);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000029E0 File Offset: 0x00000BE0
		public virtual MetadataOnlyCommonType CreateArrayType(MetadataOnlyCommonType elementType, int rank)
		{
			return new MetadataOnlyArrayType(elementType, rank);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000029FC File Offset: 0x00000BFC
		public virtual MetadataOnlyCommonType CreateVectorType(MetadataOnlyCommonType elementType)
		{
			return new MetadataOnlyVectorType(elementType);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002A14 File Offset: 0x00000C14
		public virtual MetadataOnlyCommonType CreateByRefType(MetadataOnlyCommonType type)
		{
			return new MetadataOnlyModifiedType(type, "&");
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002A34 File Offset: 0x00000C34
		public virtual MetadataOnlyCommonType CreatePointerType(MetadataOnlyCommonType type)
		{
			return new MetadataOnlyModifiedType(type, "*");
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002A54 File Offset: 0x00000C54
		public virtual MetadataOnlyTypeVariable CreateTypeVariable(MetadataOnlyModule resolver, Token typeVariableToken)
		{
			return new MetadataOnlyTypeVariable(resolver, typeVariableToken);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002A70 File Offset: 0x00000C70
		public virtual MetadataOnlyFieldInfo CreateField(MetadataOnlyModule resolver, Token fieldDefToken, Type[] typeArgs, Type[] methodArgs)
		{
			return new MetadataOnlyFieldInfo(resolver, fieldDefToken, typeArgs, methodArgs);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002A8C File Offset: 0x00000C8C
		public virtual MetadataOnlyPropertyInfo CreatePropertyInfo(MetadataOnlyModule resolver, Token propToken, Type[] typeArgs, Type[] methodArgs)
		{
			return new MetadataOnlyPropertyInfo(resolver, propToken, typeArgs, methodArgs);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002AA8 File Offset: 0x00000CA8
		public virtual MetadataOnlyEventInfo CreateEventInfo(MetadataOnlyModule resolver, Token eventToken, Type[] typeArgs, Type[] methodArgs)
		{
			return new MetadataOnlyEventInfo(resolver, eventToken, typeArgs, methodArgs);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002AC4 File Offset: 0x00000CC4
		public virtual MetadataOnlyConstructorInfo CreateConstructorInfo(MethodBase method)
		{
			return new MetadataOnlyConstructorInfo(method);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002ADC File Offset: 0x00000CDC
		public virtual MetadataOnlyMethodInfo CreateMethodInfo(MetadataOnlyMethodInfo method)
		{
			return new MetadataOnlyMethodInfo(method);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002AF4 File Offset: 0x00000CF4
		public virtual MethodBase CreateMethodOrConstructor(MetadataOnlyModule resolver, Token methodDef, Type[] typeArgs, Type[] methodArgs)
		{
			MetadataOnlyMethodInfo metadataOnlyMethodInfo = new MetadataOnlyMethodInfo(resolver, methodDef, typeArgs, methodArgs);
			bool flag = DefaultFactory.IsRawConstructor(metadataOnlyMethodInfo);
			MethodBase result;
			if (flag)
			{
				MetadataOnlyConstructorInfo metadataOnlyConstructorInfo = this.CreateConstructorInfo(metadataOnlyMethodInfo);
				result = metadataOnlyConstructorInfo;
			}
			else
			{
				MetadataOnlyMethodInfo metadataOnlyMethodInfo2 = this.CreateMethodInfo(metadataOnlyMethodInfo);
				result = metadataOnlyMethodInfo2;
			}
			return result;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002B34 File Offset: 0x00000D34
		private static bool IsRawConstructor(MethodInfo m)
		{
			bool flag = (m.Attributes & MethodAttributes.RTSpecialName) == MethodAttributes.PrivateScope;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string name = m.Name;
				bool flag2 = name.Equals(ConstructorInfo.ConstructorName, StringComparison.Ordinal);
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = name.Equals(ConstructorInfo.TypeConstructorName, StringComparison.Ordinal);
					result = flag3;
				}
			}
			return result;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002B94 File Offset: 0x00000D94
		public virtual bool TryCreateMethodBody(MetadataOnlyMethodInfo method, ref MethodBody body)
		{
			return false;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002BA8 File Offset: 0x00000DA8
		public virtual Type CreateTypeRef(MetadataOnlyModule scope, Token tokenTypeRef)
		{
			return new MetadataOnlyTypeReference(scope, tokenTypeRef);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002BC4 File Offset: 0x00000DC4
		public virtual Type CreateTypeSpec(MetadataOnlyModule scope, Token tokenTypeSpec, Type[] typeArgs, Type[] methodArgs)
		{
			return new TypeSpec(scope, tokenTypeSpec, typeArgs, methodArgs);
		}
	}
}
