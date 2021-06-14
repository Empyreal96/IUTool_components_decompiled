using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x0200000E RID: 14
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class Applications
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060000EE RID: 238 RVA: 0x0000635B File Offset: 0x0000455B
		// (set) Token: 0x060000EF RID: 239 RVA: 0x00006363 File Offset: 0x00004563
		[XmlElement(ElementName = "Application")]
		public List<Application> Items { get; set; }

		// Token: 0x060000F0 RID: 240 RVA: 0x0000636C File Offset: 0x0000456C
		public Applications()
		{
			this.Items = new List<Application>();
		}
	}
}
