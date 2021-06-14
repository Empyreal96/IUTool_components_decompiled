using System;
using System.Runtime.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000007 RID: 7
	[Serializable]
	public class EmptyArgumentDeclaredException : ParseException
	{
		// Token: 0x0600004F RID: 79 RVA: 0x0000360D File Offset: 0x0000180D
		public EmptyArgumentDeclaredException() : base("You cannot define an argument with ID: \"\"")
		{
		}

		// Token: 0x06000050 RID: 80 RVA: 0x0000361A File Offset: 0x0000181A
		public EmptyArgumentDeclaredException(string message) : base("You cannot define an argument with ID: " + message)
		{
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000035EF File Offset: 0x000017EF
		public EmptyArgumentDeclaredException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003603 File Offset: 0x00001803
		protected EmptyArgumentDeclaredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
