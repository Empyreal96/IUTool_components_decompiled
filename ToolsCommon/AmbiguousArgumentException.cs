using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200000D RID: 13
	[Serializable]
	public class AmbiguousArgumentException : ParseException
	{
		// Token: 0x0600006E RID: 110 RVA: 0x0000513F File Offset: 0x0000333F
		public AmbiguousArgumentException(string id1, string id2) : base(string.Format(CultureInfo.InvariantCulture, "Defined arguments '{0}' and '{1}' are ambiguous", new object[]
		{
			id1,
			id2
		}))
		{
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00005164 File Offset: 0x00003364
		public AmbiguousArgumentException(string id1) : base(string.Format(CultureInfo.InvariantCulture, "Defined argument '{0}' is ambiguous", new object[]
		{
			id1
		}))
		{
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000050B3 File Offset: 0x000032B3
		public AmbiguousArgumentException()
		{
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000050BB File Offset: 0x000032BB
		public AmbiguousArgumentException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000050CF File Offset: 0x000032CF
		protected AmbiguousArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
