using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200001A RID: 26
	[Serializable]
	public class ImageCommonException : Exception
	{
		// Token: 0x0600011F RID: 287 RVA: 0x0000CEB9 File Offset: 0x0000B0B9
		public ImageCommonException()
		{
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000CEC1 File Offset: 0x0000B0C1
		public ImageCommonException(string message) : base(message)
		{
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000CECA File Offset: 0x0000B0CA
		public ImageCommonException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000CED4 File Offset: 0x0000B0D4
		protected ImageCommonException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000CEE0 File Offset: 0x0000B0E0
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
