using System;
using Microsoft.Diagnostics.Tracing;

namespace Microsoft.Diagnostics.Telemetry
{
	// Token: 0x02000005 RID: 5
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	internal sealed class EventDescriptionAttribute : Attribute
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002608 File Offset: 0x00000808
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002610 File Offset: 0x00000810
		public string Description { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002619 File Offset: 0x00000819
		// (set) Token: 0x0600001A RID: 26 RVA: 0x00002621 File Offset: 0x00000821
		public EventKeywords Keywords { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001B RID: 27 RVA: 0x0000262A File Offset: 0x0000082A
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002632 File Offset: 0x00000832
		public EventLevel Level { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600001D RID: 29 RVA: 0x0000263B File Offset: 0x0000083B
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002643 File Offset: 0x00000843
		public EventOpcode Opcode { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600001F RID: 31 RVA: 0x0000264C File Offset: 0x0000084C
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002654 File Offset: 0x00000854
		public EventTags Tags { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000021 RID: 33 RVA: 0x0000265D File Offset: 0x0000085D
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002665 File Offset: 0x00000865
		public EventActivityOptions ActivityOptions { get; set; }
	}
}
