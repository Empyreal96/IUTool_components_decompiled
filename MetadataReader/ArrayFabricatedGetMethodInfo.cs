using System;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000003 RID: 3
	internal class ArrayFabricatedGetMethodInfo : ArrayFabricatedMethodInfo
	{
		// Token: 0x06000007 RID: 7 RVA: 0x0000212C File Offset: 0x0000032C
		public ArrayFabricatedGetMethodInfo(Type arrayType) : base(arrayType)
		{
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002138 File Offset: 0x00000338
		public override string Name
		{
			get
			{
				return "Get";
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002150 File Offset: 0x00000350
		public override ParameterInfo[] GetParameters()
		{
			return base.MakeParameterHelper(0);
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x0000216C File Offset: 0x0000036C
		public override Type ReturnType
		{
			get
			{
				return base.GetElementType();
			}
		}
	}
}
