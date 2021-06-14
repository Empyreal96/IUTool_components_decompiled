using System;
using System.Runtime.Serialization;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x02000028 RID: 40
	[Serializable]
	public class OptionSpecificationException : CommandException
	{
		// Token: 0x060000F7 RID: 247 RVA: 0x0000513F File Offset: 0x0000333F
		public OptionSpecificationException()
		{
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00005149 File Offset: 0x00003349
		public OptionSpecificationException(string message) : base(message)
		{
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00005154 File Offset: 0x00003354
		public OptionSpecificationException(string message, Exception exception) : base(message, exception)
		{
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00005160 File Offset: 0x00003360
		protected OptionSpecificationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
