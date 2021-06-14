using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200005A RID: 90
	[Serializable]
	public class Win32ExportException : Exception
	{
		// Token: 0x06000402 RID: 1026 RVA: 0x000110E7 File Offset: 0x0000F2E7
		public Win32ExportException()
		{
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x000110EF File Offset: 0x0000F2EF
		public Win32ExportException(string message) : base(message)
		{
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x000110F8 File Offset: 0x0000F2F8
		public Win32ExportException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x00011102 File Offset: 0x0000F302
		protected Win32ExportException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x00012838 File Offset: 0x00010A38
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
