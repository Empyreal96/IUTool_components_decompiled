using System;
using System.Text;

namespace Microsoft.Composition.ToolBox
{
	// Token: 0x0200000F RID: 15
	public class PkgException : Exception
	{
		// Token: 0x06000044 RID: 68 RVA: 0x00002B84 File Offset: 0x00000D84
		public PkgException()
		{
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002B8C File Offset: 0x00000D8C
		public PkgException(string message) : base(message)
		{
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002B95 File Offset: 0x00000D95
		public PkgException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002B9F File Offset: 0x00000D9F
		public PkgException(string message, params object[] args) : this(string.Format(message, args))
		{
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002BAE File Offset: 0x00000DAE
		public PkgException(Exception innerException, string message) : base(message, innerException)
		{
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002BB8 File Offset: 0x00000DB8
		public PkgException(Exception innerException, string message, params object[] args) : this(innerException, string.Format(message, args))
		{
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00002BC8 File Offset: 0x00000DC8
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
