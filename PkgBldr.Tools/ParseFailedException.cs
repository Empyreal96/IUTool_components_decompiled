using System;
using System.Runtime.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200000C RID: 12
	[Serializable]
	public class ParseFailedException : ParseException
	{
		// Token: 0x06000065 RID: 101 RVA: 0x000036D9 File Offset: 0x000018D9
		public ParseFailedException(string message) : base(message)
		{
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000035E7 File Offset: 0x000017E7
		public ParseFailedException()
		{
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000035EF File Offset: 0x000017EF
		public ParseFailedException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003603 File Offset: 0x00001803
		protected ParseFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
