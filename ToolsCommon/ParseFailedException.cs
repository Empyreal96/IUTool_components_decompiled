using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000010 RID: 16
	[Serializable]
	public class ParseFailedException : ParseException
	{
		// Token: 0x0600007B RID: 123 RVA: 0x000051A5 File Offset: 0x000033A5
		public ParseFailedException(string message) : base(message)
		{
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000050B3 File Offset: 0x000032B3
		public ParseFailedException()
		{
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000050BB File Offset: 0x000032BB
		public ParseFailedException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000050CF File Offset: 0x000032CF
		protected ParseFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
