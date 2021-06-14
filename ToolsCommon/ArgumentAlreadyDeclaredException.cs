using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200000A RID: 10
	[Serializable]
	public class ArgumentAlreadyDeclaredException : ParseException
	{
		// Token: 0x06000061 RID: 97 RVA: 0x00005092 File Offset: 0x00003292
		public ArgumentAlreadyDeclaredException(string id) : base(string.Format(CultureInfo.InvariantCulture, "Argument '{0}' was already defined", new object[]
		{
			id
		}))
		{
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000050B3 File Offset: 0x000032B3
		public ArgumentAlreadyDeclaredException()
		{
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000050BB File Offset: 0x000032BB
		public ArgumentAlreadyDeclaredException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000050CF File Offset: 0x000032CF
		protected ArgumentAlreadyDeclaredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
