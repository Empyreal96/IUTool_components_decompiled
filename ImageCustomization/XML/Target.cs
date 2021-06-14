using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x02000018 RID: 24
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class Target : IDefinedIn
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600016F RID: 367 RVA: 0x00008008 File Offset: 0x00006208
		// (set) Token: 0x06000170 RID: 368 RVA: 0x00008010 File Offset: 0x00006210
		[XmlIgnore]
		public string DefinedInFile { get; set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000171 RID: 369 RVA: 0x00008019 File Offset: 0x00006219
		// (set) Token: 0x06000172 RID: 370 RVA: 0x00008021 File Offset: 0x00006221
		[XmlAttribute]
		public string Id { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000173 RID: 371 RVA: 0x0000802A File Offset: 0x0000622A
		// (set) Token: 0x06000174 RID: 372 RVA: 0x00008032 File Offset: 0x00006232
		[XmlElement(ElementName = "TargetState")]
		public List<TargetState> TargetStates { get; set; }

		// Token: 0x06000175 RID: 373 RVA: 0x0000803B File Offset: 0x0000623B
		public Target()
		{
			this.TargetStates = new List<TargetState>();
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000804E File Offset: 0x0000624E
		public Target(string id) : this()
		{
			this.Id = id;
		}
	}
}
