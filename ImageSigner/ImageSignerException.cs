using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.Imaging.ImageSignerApp
{
	// Token: 0x02000005 RID: 5
	[Serializable]
	public class ImageSignerException : Exception
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000028AB File Offset: 0x00000AAB
		public ImageSignerException()
		{
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002134 File Offset: 0x00000334
		public ImageSignerException(string message) : base(message)
		{
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000028B3 File Offset: 0x00000AB3
		public ImageSignerException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000028BD File Offset: 0x00000ABD
		protected ImageSignerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000028C8 File Offset: 0x00000AC8
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
