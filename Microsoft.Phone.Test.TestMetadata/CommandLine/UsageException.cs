using System;
using System.Runtime.Serialization;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x0200002B RID: 43
	[Serializable]
	public class UsageException : CommandException
	{
		// Token: 0x06000101 RID: 257 RVA: 0x0000513F File Offset: 0x0000333F
		public UsageException()
		{
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00005149 File Offset: 0x00003349
		public UsageException(string message) : base(message)
		{
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00005154 File Offset: 0x00003354
		public UsageException(string message, Exception exception) : base(message, exception)
		{
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00005160 File Offset: 0x00003360
		protected UsageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
