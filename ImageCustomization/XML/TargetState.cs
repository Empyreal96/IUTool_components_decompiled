using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x02000019 RID: 25
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class TargetState
	{
		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000177 RID: 375 RVA: 0x0000805D File Offset: 0x0000625D
		// (set) Token: 0x06000178 RID: 376 RVA: 0x00008065 File Offset: 0x00006265
		[XmlElement(ElementName = "Condition")]
		public List<Condition> Items { get; set; }

		// Token: 0x06000179 RID: 377 RVA: 0x0000806E File Offset: 0x0000626E
		public TargetState()
		{
			this.Items = new List<Condition>();
		}
	}
}
