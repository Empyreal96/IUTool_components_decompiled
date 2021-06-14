using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000009 RID: 9
	[Serializable]
	public class AmbiguousArgumentException : ParseException
	{
		// Token: 0x06000058 RID: 88 RVA: 0x00003673 File Offset: 0x00001873
		public AmbiguousArgumentException(string id1, string id2) : base(string.Format(CultureInfo.InvariantCulture, "Defined arguments '{0}' and '{1}' are ambiguous", new object[]
		{
			id1,
			id2
		}))
		{
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003698 File Offset: 0x00001898
		public AmbiguousArgumentException(string id1) : base(string.Format(CultureInfo.InvariantCulture, "Defined argument '{0}' is ambiguous", new object[]
		{
			id1
		}))
		{
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000035E7 File Offset: 0x000017E7
		public AmbiguousArgumentException()
		{
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000035EF File Offset: 0x000017EF
		public AmbiguousArgumentException(string message, Exception except) : base("Program error:" + message, except)
		{
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003603 File Offset: 0x00001803
		protected AmbiguousArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
