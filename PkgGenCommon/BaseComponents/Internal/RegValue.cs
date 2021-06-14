using System;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000066 RID: 102
	[XmlRoot(ElementName = "RegValue", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public class RegValue
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x00007654 File Offset: 0x00005854
		// (set) Token: 0x060001D6 RID: 470 RVA: 0x0000765C File Offset: 0x0000585C
		[XmlAttribute("Name")]
		public string Name { get; set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x00007665 File Offset: 0x00005865
		// (set) Token: 0x060001D8 RID: 472 RVA: 0x0000766D File Offset: 0x0000586D
		[XmlAttribute("Type")]
		public RegValueType RegValType { get; set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x00007676 File Offset: 0x00005876
		// (set) Token: 0x060001DA RID: 474 RVA: 0x0000767E File Offset: 0x0000587E
		[XmlAttribute("Value")]
		public string Value { get; set; }
	}
}
