using System;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000038 RID: 56
	internal class MethodFilter
	{
		// Token: 0x0600042E RID: 1070 RVA: 0x0000D717 File Offset: 0x0000B917
		public MethodFilter(string name, int genericParameterCount, int parameterCount, CorCallingConvention callingConvention)
		{
			this.Name = name;
			this.GenericParameterCount = genericParameterCount;
			this.ParameterCount = parameterCount;
			this.CallingConvention = callingConvention;
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x0600042F RID: 1071 RVA: 0x0000D742 File Offset: 0x0000B942
		// (set) Token: 0x06000430 RID: 1072 RVA: 0x0000D74A File Offset: 0x0000B94A
		public string Name { get; set; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x0000D753 File Offset: 0x0000B953
		// (set) Token: 0x06000432 RID: 1074 RVA: 0x0000D75B File Offset: 0x0000B95B
		public int GenericParameterCount { get; set; }

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000433 RID: 1075 RVA: 0x0000D764 File Offset: 0x0000B964
		// (set) Token: 0x06000434 RID: 1076 RVA: 0x0000D76C File Offset: 0x0000B96C
		public int ParameterCount { get; set; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000435 RID: 1077 RVA: 0x0000D775 File Offset: 0x0000B975
		// (set) Token: 0x06000436 RID: 1078 RVA: 0x0000D77D File Offset: 0x0000B97D
		public CorCallingConvention CallingConvention { get; set; }
	}
}
