using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x02000017 RID: 23
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class TargetRef : IDefinedIn
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000169 RID: 361 RVA: 0x00007FD7 File Offset: 0x000061D7
		// (set) Token: 0x0600016A RID: 362 RVA: 0x00007FDF File Offset: 0x000061DF
		[XmlIgnore]
		public string DefinedInFile { get; set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x0600016B RID: 363 RVA: 0x00007FE8 File Offset: 0x000061E8
		// (set) Token: 0x0600016C RID: 364 RVA: 0x00007FF0 File Offset: 0x000061F0
		[XmlAttribute]
		public string Id { get; set; }

		// Token: 0x0600016D RID: 365 RVA: 0x00002230 File Offset: 0x00000430
		public TargetRef()
		{
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00007FF9 File Offset: 0x000061F9
		public TargetRef(string id)
		{
			this.Id = id;
		}
	}
}
