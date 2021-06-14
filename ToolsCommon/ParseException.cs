using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000009 RID: 9
	[Serializable]
	public class ParseException : IUException
	{
		// Token: 0x0600005D RID: 93 RVA: 0x00005059 File Offset: 0x00003259
		public ParseException(string message) : base("Program error:" + message)
		{
		}

		// Token: 0x0600005E RID: 94 RVA: 0x0000506C File Offset: 0x0000326C
		public ParseException()
		{
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00005074 File Offset: 0x00003274
		public ParseException(string message, Exception except) : base(except, "Program error:" + message)
		{
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00005088 File Offset: 0x00003288
		protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
