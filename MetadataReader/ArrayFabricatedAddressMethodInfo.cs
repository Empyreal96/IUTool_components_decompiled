using System;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000005 RID: 5
	internal class ArrayFabricatedAddressMethodInfo : ArrayFabricatedMethodInfo
	{
		// Token: 0x0600000F RID: 15 RVA: 0x0000212C File Offset: 0x0000032C
		public ArrayFabricatedAddressMethodInfo(Type arrayType) : base(arrayType)
		{
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000021F0 File Offset: 0x000003F0
		public override string Name
		{
			get
			{
				return "Address";
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002208 File Offset: 0x00000408
		public override ParameterInfo[] GetParameters()
		{
			return base.MakeParameterHelper(0);
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002224 File Offset: 0x00000424
		public override Type ReturnType
		{
			get
			{
				return base.GetElementType().MakeByRefType();
			}
		}
	}
}
