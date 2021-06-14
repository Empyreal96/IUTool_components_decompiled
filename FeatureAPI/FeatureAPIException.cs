using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000002 RID: 2
	[Serializable]
	public class FeatureAPIException : Exception
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public FeatureAPIException()
		{
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public FeatureAPIException(string message) : base(message)
		{
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002061 File Offset: 0x00000261
		public FeatureAPIException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000206B File Offset: 0x0000026B
		protected FeatureAPIException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002078 File Offset: 0x00000278
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
