using System;
using System.Diagnostics;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000018 RID: 24
	public class LogEventArgs : EventArgs
	{
		// Token: 0x06000113 RID: 275 RVA: 0x00006F65 File Offset: 0x00005165
		public LogEventArgs(string message, TraceLevel traceLevel)
		{
			this.Message = message;
			this.TimeStamp = DateTime.Now;
			this.TraceLevel = traceLevel;
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000114 RID: 276 RVA: 0x00006F8B File Offset: 0x0000518B
		// (set) Token: 0x06000115 RID: 277 RVA: 0x00006F93 File Offset: 0x00005193
		public DateTime TimeStamp { get; private set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000116 RID: 278 RVA: 0x00006F9C File Offset: 0x0000519C
		// (set) Token: 0x06000117 RID: 279 RVA: 0x00006FA4 File Offset: 0x000051A4
		public string Message { get; private set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000118 RID: 280 RVA: 0x00006FAD File Offset: 0x000051AD
		// (set) Token: 0x06000119 RID: 281 RVA: 0x00006FB5 File Offset: 0x000051B5
		public TraceLevel TraceLevel { get; private set; }
	}
}
