using System;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000004 RID: 4
	internal class ArrayFabricatedSetMethodInfo : ArrayFabricatedMethodInfo
	{
		// Token: 0x0600000B RID: 11 RVA: 0x0000212C File Offset: 0x0000032C
		public ArrayFabricatedSetMethodInfo(Type arrayType) : base(arrayType)
		{
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002184 File Offset: 0x00000384
		public override string Name
		{
			get
			{
				return "Set";
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000219C File Offset: 0x0000039C
		public override ParameterInfo[] GetParameters()
		{
			ParameterInfo[] array = base.MakeParameterHelper(1);
			int rank = base.Rank;
			Type elementType = base.GetElementType();
			array[rank] = base.MakeParameterInfo(elementType, rank);
			return array;
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000021D0 File Offset: 0x000003D0
		public override Type ReturnType
		{
			get
			{
				return base.Universe.GetBuiltInType(CorElementType.Void);
			}
		}
	}
}
