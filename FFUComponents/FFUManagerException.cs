using System;
using System.Runtime.Serialization;

namespace FFUComponents
{
	// Token: 0x0200001F RID: 31
	[Serializable]
	public class FFUManagerException : Exception
	{
		// Token: 0x060000C0 RID: 192 RVA: 0x00003B04 File Offset: 0x00001D04
		public FFUManagerException()
		{
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00003B0C File Offset: 0x00001D0C
		public FFUManagerException(string message) : base(message)
		{
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00003B15 File Offset: 0x00001D15
		public FFUManagerException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00003CF3 File Offset: 0x00001EF3
		protected FFUManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
