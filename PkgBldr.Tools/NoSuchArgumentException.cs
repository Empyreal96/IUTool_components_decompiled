using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000008 RID: 8
	[Serializable]
	public class NoSuchArgumentException : ParseException
	{
		// Token: 0x06000053 RID: 83 RVA: 0x0000362D File Offset: 0x0000182D
		public NoSuchArgumentException(string type, string id) : base(string.Format(CultureInfo.InvariantCulture, "The {0} '{1}' was not defined", new object[]
		{
			type,
			id
		}))
		{
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003652 File Offset: 0x00001852
		public NoSuchArgumentException(string id) : base(string.Format(CultureInfo.InvariantCulture, "The '{0}' was not defined", new object[]
		{
			id
		}))
		{
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000035E7 File Offset: 0x000017E7
		public NoSuchArgumentException()
		{
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000035EF File Offset: 0x000017EF
		public NoSuchArgumentException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003603 File Offset: 0x00001803
		protected NoSuchArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
