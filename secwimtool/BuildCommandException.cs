using System;

namespace SecureWim
{
	// Token: 0x02000004 RID: 4
	internal class BuildCommandException : Exception
	{
		// Token: 0x0600000C RID: 12 RVA: 0x0000270C File Offset: 0x0000090C
		public BuildCommandException(string message, int errorCode) : base(message)
		{
			this.ErrorCode = errorCode;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000271C File Offset: 0x0000091C
		// (set) Token: 0x0600000E RID: 14 RVA: 0x00002724 File Offset: 0x00000924
		public int ErrorCode { get; private set; }
	}
}
