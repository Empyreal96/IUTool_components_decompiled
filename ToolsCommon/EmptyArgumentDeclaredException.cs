using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200000B RID: 11
	[Serializable]
	public class EmptyArgumentDeclaredException : ParseException
	{
		// Token: 0x06000065 RID: 101 RVA: 0x000050D9 File Offset: 0x000032D9
		public EmptyArgumentDeclaredException() : base("You cannot define an argument with ID: \"\"")
		{
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000050E6 File Offset: 0x000032E6
		public EmptyArgumentDeclaredException(string message) : base("You cannot define an argument with ID: " + message)
		{
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000050BB File Offset: 0x000032BB
		public EmptyArgumentDeclaredException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000050CF File Offset: 0x000032CF
		protected EmptyArgumentDeclaredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
