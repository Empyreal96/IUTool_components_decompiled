using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000004 RID: 4
	public class IUException : Exception
	{
		// Token: 0x0600003F RID: 63 RVA: 0x000034F1 File Offset: 0x000016F1
		public IUException()
		{
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000034F9 File Offset: 0x000016F9
		public IUException(string message) : base(message)
		{
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003502 File Offset: 0x00001702
		public IUException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000042 RID: 66 RVA: 0x0000350C File Offset: 0x0000170C
		public IUException(string message, params object[] args) : this(string.Format(CultureInfo.InvariantCulture, message, args))
		{
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003520 File Offset: 0x00001720
		protected IUException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000352A File Offset: 0x0000172A
		public IUException(Exception innerException, string message) : base(message, innerException)
		{
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003534 File Offset: 0x00001734
		public IUException(Exception innerException, string message, params object[] args) : this(innerException, string.Format(CultureInfo.InvariantCulture, message, args))
		{
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000046 RID: 70 RVA: 0x0000354C File Offset: 0x0000174C
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
