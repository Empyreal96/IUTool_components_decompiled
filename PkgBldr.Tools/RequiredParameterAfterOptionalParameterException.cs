using System;
using System.Runtime.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200000A RID: 10
	[Serializable]
	public class RequiredParameterAfterOptionalParameterException : ParseException
	{
		// Token: 0x0600005D RID: 93 RVA: 0x000036B9 File Offset: 0x000018B9
		public RequiredParameterAfterOptionalParameterException() : base("An optional parameter can't be followed by a required one")
		{
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000036C6 File Offset: 0x000018C6
		public RequiredParameterAfterOptionalParameterException(string message) : base("Program error:" + message)
		{
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000035EF File Offset: 0x000017EF
		public RequiredParameterAfterOptionalParameterException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003603 File Offset: 0x00001803
		protected RequiredParameterAfterOptionalParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
