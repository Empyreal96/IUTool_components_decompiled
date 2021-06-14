using System;
using System.Runtime.Serialization;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000002 RID: 2
	[Serializable]
	public class MtbfLogParserException : Exception
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public MtbfLogParserException()
		{
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public MtbfLogParserException(string message) : base(message)
		{
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002061 File Offset: 0x00000261
		public MtbfLogParserException(string message, Exception inner) : base(message, inner)
		{
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000206B File Offset: 0x0000026B
		protected MtbfLogParserException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
