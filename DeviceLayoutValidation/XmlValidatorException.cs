using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200000A RID: 10
	[Serializable]
	public class XmlValidatorException : Exception
	{
		// Token: 0x0600005F RID: 95 RVA: 0x00004A85 File Offset: 0x00002C85
		public XmlValidatorException()
		{
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004A8D File Offset: 0x00002C8D
		public XmlValidatorException(string message) : base(message)
		{
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004A96 File Offset: 0x00002C96
		public XmlValidatorException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004AA0 File Offset: 0x00002CA0
		protected XmlValidatorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004AAC File Offset: 0x00002CAC
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
