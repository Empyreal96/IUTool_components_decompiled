using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000056 RID: 86
	[Serializable]
	public class XsdValidatorException : Exception
	{
		// Token: 0x06000273 RID: 627 RVA: 0x00004FC8 File Offset: 0x000031C8
		public XsdValidatorException()
		{
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00004FD0 File Offset: 0x000031D0
		public XsdValidatorException(string message) : base(message)
		{
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00004FD9 File Offset: 0x000031D9
		public XsdValidatorException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00004FF2 File Offset: 0x000031F2
		protected XsdValidatorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000B9C0 File Offset: 0x00009BC0
		public override string ToString()
		{
			string text = this.Message;
			if (base.InnerException != null)
			{
				text += base.InnerException.ToString();
			}
			return text;
		}
	}
}
