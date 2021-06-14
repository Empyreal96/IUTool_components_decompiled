using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x02000014 RID: 20
	public class CompDBResolution
	{
		// Token: 0x060000C7 RID: 199 RVA: 0x00004257 File Offset: 0x00002457
		public CompDBResolution()
		{
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000A3CB File Offset: 0x000085CB
		public CompDBResolution(string id)
		{
			this.Id = id;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000A3DA File Offset: 0x000085DA
		public CompDBResolution(CompDBResolution srcRes)
		{
			this.Id = srcRes.Id;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000A3EE File Offset: 0x000085EE
		public override string ToString()
		{
			return this.Id;
		}

		// Token: 0x040000A3 RID: 163
		[XmlAttribute]
		public string Id;
	}
}
