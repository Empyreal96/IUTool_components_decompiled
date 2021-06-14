using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200000B RID: 11
	[Serializable]
	public class ImagingException : Exception
	{
		// Token: 0x0600008D RID: 141 RVA: 0x00008292 File Offset: 0x00006492
		public ImagingException()
		{
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000829A File Offset: 0x0000649A
		public ImagingException(string message) : base(message)
		{
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000082A3 File Offset: 0x000064A3
		public ImagingException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000082AD File Offset: 0x000064AD
		protected ImagingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000082B8 File Offset: 0x000064B8
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
