using System;
using System.Runtime.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200000B RID: 11
	[Serializable]
	public class BadGroupException : ParseException
	{
		// Token: 0x06000061 RID: 97 RVA: 0x000036D9 File Offset: 0x000018D9
		public BadGroupException(string message) : base(message)
		{
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000035E7 File Offset: 0x000017E7
		public BadGroupException()
		{
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000035EF File Offset: 0x000017EF
		public BadGroupException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003603 File Offset: 0x00001803
		protected BadGroupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
