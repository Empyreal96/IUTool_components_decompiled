using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200004B RID: 75
	[Serializable]
	public class ImageStorageException : Exception
	{
		// Token: 0x06000392 RID: 914 RVA: 0x000110E7 File Offset: 0x0000F2E7
		public ImageStorageException()
		{
		}

		// Token: 0x06000393 RID: 915 RVA: 0x000110EF File Offset: 0x0000F2EF
		public ImageStorageException(string message) : base(message)
		{
		}

		// Token: 0x06000394 RID: 916 RVA: 0x000110F8 File Offset: 0x0000F2F8
		public ImageStorageException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000395 RID: 917 RVA: 0x00011102 File Offset: 0x0000F302
		protected ImageStorageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000396 RID: 918 RVA: 0x0001110C File Offset: 0x0000F30C
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
