using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000004 RID: 4
	public class IUException : Exception
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00003B07 File Offset: 0x00001D07
		public IUException()
		{
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00003B0F File Offset: 0x00001D0F
		public IUException(string message) : base(message)
		{
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003B18 File Offset: 0x00001D18
		public IUException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003B22 File Offset: 0x00001D22
		public IUException(string message, params object[] args) : this(string.Format(CultureInfo.InvariantCulture, message, args))
		{
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003B36 File Offset: 0x00001D36
		protected IUException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00003B40 File Offset: 0x00001D40
		public IUException(Exception innerException, string message) : base(message, innerException)
		{
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00003B4A File Offset: 0x00001D4A
		public IUException(Exception innerException, string message, params object[] args) : this(innerException, string.Format(CultureInfo.InvariantCulture, message, args))
		{
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00003B60 File Offset: 0x00001D60
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
