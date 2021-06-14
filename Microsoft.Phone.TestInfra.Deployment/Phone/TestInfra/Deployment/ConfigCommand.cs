using System;
using System.ComponentModel;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200002F RID: 47
	public class ConfigCommand
	{
		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600022F RID: 559 RVA: 0x0000E5B4 File Offset: 0x0000C7B4
		// (set) Token: 0x06000230 RID: 560 RVA: 0x0000E5BC File Offset: 0x0000C7BC
		public string CommandLine { get; set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000231 RID: 561 RVA: 0x0000E5C5 File Offset: 0x0000C7C5
		// (set) Token: 0x06000232 RID: 562 RVA: 0x0000E5CD File Offset: 0x0000C7CD
		[DefaultValue(0)]
		public int SuccessExitCode { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000233 RID: 563 RVA: 0x0000E5D6 File Offset: 0x0000C7D6
		// (set) Token: 0x06000234 RID: 564 RVA: 0x0000E5DE File Offset: 0x0000C7DE
		[DefaultValue(false)]
		public bool IgnoreExitCode { get; set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000235 RID: 565 RVA: 0x0000E5E7 File Offset: 0x0000C7E7
		// (set) Token: 0x06000236 RID: 566 RVA: 0x0000E5EF File Offset: 0x0000C7EF
		[DefaultValue(3)]
		public int TimeOutInMins { get; set; }
	}
}
