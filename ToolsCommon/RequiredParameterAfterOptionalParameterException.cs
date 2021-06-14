using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200000E RID: 14
	[Serializable]
	public class RequiredParameterAfterOptionalParameterException : ParseException
	{
		// Token: 0x06000073 RID: 115 RVA: 0x00005185 File Offset: 0x00003385
		public RequiredParameterAfterOptionalParameterException() : base("An optional parameter can't be followed by a required one")
		{
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00005192 File Offset: 0x00003392
		public RequiredParameterAfterOptionalParameterException(string message) : base("Program error:" + message)
		{
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000050BB File Offset: 0x000032BB
		public RequiredParameterAfterOptionalParameterException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000050CF File Offset: 0x000032CF
		protected RequiredParameterAfterOptionalParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
