using System;
using Microsoft.Diagnostics.Tracing;

namespace Microsoft.Diagnostics.Telemetry
{
	// Token: 0x02000006 RID: 6
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	internal sealed class EventDescriptionAttribute : Attribute
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002849 File Offset: 0x00000A49
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002851 File Offset: 0x00000A51
		public string Description { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000021 RID: 33 RVA: 0x0000285A File Offset: 0x00000A5A
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002862 File Offset: 0x00000A62
		public EventKeywords Keywords { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000023 RID: 35 RVA: 0x0000286B File Offset: 0x00000A6B
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002873 File Offset: 0x00000A73
		public EventLevel Level { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000025 RID: 37 RVA: 0x0000287C File Offset: 0x00000A7C
		// (set) Token: 0x06000026 RID: 38 RVA: 0x00002884 File Offset: 0x00000A84
		public EventOpcode Opcode { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000027 RID: 39 RVA: 0x0000288D File Offset: 0x00000A8D
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00002895 File Offset: 0x00000A95
		public EventTags Tags { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000029 RID: 41 RVA: 0x0000289E File Offset: 0x00000A9E
		// (set) Token: 0x0600002A RID: 42 RVA: 0x000028A6 File Offset: 0x00000AA6
		public EventActivityOptions ActivityOptions { get; set; }
	}
}
