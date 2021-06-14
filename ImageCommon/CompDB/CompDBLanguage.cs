using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x02000013 RID: 19
	public class CompDBLanguage
	{
		// Token: 0x060000C3 RID: 195 RVA: 0x00004257 File Offset: 0x00002457
		public CompDBLanguage()
		{
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000A3A0 File Offset: 0x000085A0
		public CompDBLanguage(string id)
		{
			this.Id = id;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0000A3AF File Offset: 0x000085AF
		public CompDBLanguage(CompDBLanguage srcLang)
		{
			this.Id = srcLang.Id;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000A3C3 File Offset: 0x000085C3
		public override string ToString()
		{
			return this.Id;
		}

		// Token: 0x040000A2 RID: 162
		[XmlAttribute]
		public string Id;
	}
}
