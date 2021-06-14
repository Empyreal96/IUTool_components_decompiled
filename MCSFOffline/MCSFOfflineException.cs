using System;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.MCSF.Offline
{
	// Token: 0x0200000C RID: 12
	[Serializable]
	public class MCSFOfflineException : IUException
	{
		// Token: 0x0600009A RID: 154 RVA: 0x00003DC5 File Offset: 0x00001FC5
		public MCSFOfflineException()
		{
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00003DCD File Offset: 0x00001FCD
		public MCSFOfflineException(string message) : base(message)
		{
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00003DD6 File Offset: 0x00001FD6
		public MCSFOfflineException(Exception innerException, string message) : base(innerException, message)
		{
		}
	}
}
