using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000073 RID: 115
	[Serializable]
	public class BCDXsdValidatorException : Exception
	{
		// Token: 0x060004CB RID: 1227 RVA: 0x000110E7 File Offset: 0x0000F2E7
		public BCDXsdValidatorException()
		{
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x000110EF File Offset: 0x0000F2EF
		public BCDXsdValidatorException(string message) : base(message)
		{
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x000110F8 File Offset: 0x0000F2F8
		public BCDXsdValidatorException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x00011102 File Offset: 0x0000F302
		protected BCDXsdValidatorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x00014D78 File Offset: 0x00012F78
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
