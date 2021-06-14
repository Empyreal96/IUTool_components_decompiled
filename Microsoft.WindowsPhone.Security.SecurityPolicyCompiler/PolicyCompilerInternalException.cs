using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000005 RID: 5
	[Serializable]
	public class PolicyCompilerInternalException : Exception
	{
		// Token: 0x06000004 RID: 4 RVA: 0x000022BD File Offset: 0x000004BD
		public PolicyCompilerInternalException()
		{
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000022C5 File Offset: 0x000004C5
		public PolicyCompilerInternalException(string errMsg) : base(errMsg)
		{
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000022CE File Offset: 0x000004CE
		protected PolicyCompilerInternalException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
		{
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000022D8 File Offset: 0x000004D8
		public PolicyCompilerInternalException(string errMsg, Exception originalException) : base(errMsg, originalException)
		{
		}
	}
}
