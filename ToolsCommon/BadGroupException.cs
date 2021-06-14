using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200000F RID: 15
	[Serializable]
	public class BadGroupException : ParseException
	{
		// Token: 0x06000077 RID: 119 RVA: 0x000051A5 File Offset: 0x000033A5
		public BadGroupException(string message) : base(message)
		{
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000050B3 File Offset: 0x000032B3
		public BadGroupException()
		{
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000050BB File Offset: 0x000032BB
		public BadGroupException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000050CF File Offset: 0x000032CF
		protected BadGroupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
