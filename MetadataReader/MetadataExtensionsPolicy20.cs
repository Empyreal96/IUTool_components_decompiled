using System;
using System.Collections.Generic;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000036 RID: 54
	internal class MetadataExtensionsPolicy20 : IMetadataExtensionsPolicy
	{
		// Token: 0x0600041C RID: 1052 RVA: 0x0000D247 File Offset: 0x0000B447
		public MetadataExtensionsPolicy20(ITypeUniverse u)
		{
			this.m_universe = u;
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x0000D258 File Offset: 0x0000B458
		public virtual Type[] GetExtraArrayInterfaces(Type elementType)
		{
			bool isPointer = elementType.IsPointer;
			Type[] result;
			if (isPointer)
			{
				result = Type.EmptyTypes;
			}
			else
			{
				Type[] argTypes = new Type[]
				{
					elementType
				};
				Type[] array = new Type[]
				{
					this.m_universe.GetTypeXFromName("System.Collections.Generic.IList`1").MakeGenericType(argTypes),
					this.m_universe.GetTypeXFromName("System.Collections.Generic.ICollection`1").MakeGenericType(argTypes),
					this.m_universe.GetTypeXFromName("System.Collections.Generic.IEnumerable`1").MakeGenericType(argTypes)
				};
				result = array;
			}
			return result;
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x0000D2DC File Offset: 0x0000B4DC
		public virtual MethodInfo[] GetExtraArrayMethods(Type arrayType)
		{
			return new MethodInfo[]
			{
				new ArrayFabricatedGetMethodInfo(arrayType),
				new ArrayFabricatedSetMethodInfo(arrayType),
				new ArrayFabricatedAddressMethodInfo(arrayType)
			};
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x0000D310 File Offset: 0x0000B510
		public virtual ConstructorInfo[] GetExtraArrayConstructors(Type arrayType)
		{
			int arrayRank = arrayType.GetArrayRank();
			return new ConstructorInfo[]
			{
				new ArrayFabricatedConstructorInfo(arrayType, arrayRank),
				new ArrayFabricatedConstructorInfo(arrayType, arrayRank * 2)
			};
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x0000D348 File Offset: 0x0000B548
		public virtual ParameterInfo GetFakeParameterInfo(MemberInfo member, Type paramType, int position)
		{
			return new SimpleParameterInfo(member, paramType, position);
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x0000D364 File Offset: 0x0000B564
		public virtual IEnumerable<CustomAttributeData> GetPseudoCustomAttributes(MetadataOnlyModule module, Token token)
		{
			List<CustomAttributeData> list = new List<CustomAttributeData>();
			list.AddRange(PseudoCustomAttributes.GetTypeForwardedToAttributes(module, token));
			CustomAttributeData serializableAttribute = PseudoCustomAttributes.GetSerializableAttribute(module, token);
			bool flag = serializableAttribute != null;
			if (flag)
			{
				list.Add(serializableAttribute);
			}
			return list;
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x0000D3A8 File Offset: 0x0000B5A8
		public virtual Type TryTypeForwardResolution(MetadataOnlyAssembly assembly, string fullname, bool ignoreCase)
		{
			UnresolvedTypeName rawTypeForwardedToAttribute = PseudoCustomAttributes.GetRawTypeForwardedToAttribute(assembly, fullname, ignoreCase);
			bool flag = rawTypeForwardedToAttribute != null;
			Type result;
			if (flag)
			{
				Type type = rawTypeForwardedToAttribute.ConvertToType(assembly.TypeUniverse, assembly.ManifestModuleInternal);
				result = type;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x040000C5 RID: 197
		protected ITypeUniverse m_universe;
	}
}
