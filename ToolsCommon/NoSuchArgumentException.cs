using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200000C RID: 12
	[Serializable]
	public class NoSuchArgumentException : ParseException
	{
		// Token: 0x06000069 RID: 105 RVA: 0x000050F9 File Offset: 0x000032F9
		public NoSuchArgumentException(string type, string id) : base(string.Format(CultureInfo.InvariantCulture, "The {0} '{1}' was not defined", new object[]
		{
			type,
			id
		}))
		{
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000511E File Offset: 0x0000331E
		public NoSuchArgumentException(string id) : base(string.Format(CultureInfo.InvariantCulture, "The '{0}' was not defined", new object[]
		{
			id
		}))
		{
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000050B3 File Offset: 0x000032B3
		public NoSuchArgumentException()
		{
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000050BB File Offset: 0x000032BB
		public NoSuchArgumentException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000050CF File Offset: 0x000032CF
		protected NoSuchArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
