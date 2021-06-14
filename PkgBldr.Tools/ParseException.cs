using System;
using System.Runtime.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000005 RID: 5
	[Serializable]
	public class ParseException : IUException
	{
		// Token: 0x06000047 RID: 71 RVA: 0x0000358D File Offset: 0x0000178D
		public ParseException(string message) : base("Program error:" + message)
		{
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000035A0 File Offset: 0x000017A0
		public ParseException()
		{
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000035A8 File Offset: 0x000017A8
		public ParseException(string message, Exception except) : base(except, "Program error:" + message)
		{
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000035BC File Offset: 0x000017BC
		protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
