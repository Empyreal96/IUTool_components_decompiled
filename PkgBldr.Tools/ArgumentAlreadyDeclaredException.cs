using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000006 RID: 6
	[Serializable]
	public class ArgumentAlreadyDeclaredException : ParseException
	{
		// Token: 0x0600004B RID: 75 RVA: 0x000035C6 File Offset: 0x000017C6
		public ArgumentAlreadyDeclaredException(string id) : base(string.Format(CultureInfo.InvariantCulture, "Argument '{0}' was already defined", new object[]
		{
			id
		}))
		{
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000035E7 File Offset: 0x000017E7
		public ArgumentAlreadyDeclaredException()
		{
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000035EF File Offset: 0x000017EF
		public ArgumentAlreadyDeclaredException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003603 File Offset: 0x00001803
		protected ArgumentAlreadyDeclaredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
