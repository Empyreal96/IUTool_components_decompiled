using System;
using Microsoft.Diagnostics.Tracing;

namespace Microsoft.Diagnostics.Telemetry
{
	// Token: 0x02000008 RID: 8
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	internal sealed class EventDescriptionAttribute : Attribute
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002F4B File Offset: 0x0000114B
		// (set) Token: 0x06000035 RID: 53 RVA: 0x00002F53 File Offset: 0x00001153
		public string Description { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002F5C File Offset: 0x0000115C
		// (set) Token: 0x06000037 RID: 55 RVA: 0x00002F64 File Offset: 0x00001164
		public EventKeywords Keywords { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002F6D File Offset: 0x0000116D
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002F75 File Offset: 0x00001175
		public EventLevel Level { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002F7E File Offset: 0x0000117E
		// (set) Token: 0x0600003B RID: 59 RVA: 0x00002F86 File Offset: 0x00001186
		public EventOpcode Opcode { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00002F8F File Offset: 0x0000118F
		// (set) Token: 0x0600003D RID: 61 RVA: 0x00002F97 File Offset: 0x00001197
		public EventTags Tags { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00002FA0 File Offset: 0x000011A0
		// (set) Token: 0x0600003F RID: 63 RVA: 0x00002FA8 File Offset: 0x000011A8
		public EventActivityOptions ActivityOptions { get; set; }
	}
}
