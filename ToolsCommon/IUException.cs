using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000008 RID: 8
	public class IUException : Exception
	{
		// Token: 0x06000055 RID: 85 RVA: 0x00004FC8 File Offset: 0x000031C8
		public IUException()
		{
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004FD0 File Offset: 0x000031D0
		public IUException(string message) : base(message)
		{
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004FD9 File Offset: 0x000031D9
		public IUException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004FE3 File Offset: 0x000031E3
		public IUException(string message, params object[] args) : this(string.Format(message, args))
		{
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004FF2 File Offset: 0x000031F2
		protected IUException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004FFC File Offset: 0x000031FC
		public IUException(Exception innerException, string message) : base(message, innerException)
		{
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00005006 File Offset: 0x00003206
		public IUException(Exception innerException, string message, params object[] args) : this(innerException, string.Format(message, args))
		{
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00005018 File Offset: 0x00003218
		public string MessageTrace
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (Exception ex = this; ex != null; ex = ex.InnerException)
				{
					if (!string.IsNullOrEmpty(ex.Message))
					{
						stringBuilder.AppendLine(ex.Message);
					}
				}
				return stringBuilder.ToString();
			}
		}
	}
}
