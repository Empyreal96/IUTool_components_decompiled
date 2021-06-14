using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000056 RID: 86
	[XmlRoot(ElementName = "AppResource", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public class AppResourcePkgObject : OSComponentPkgObject
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000171 RID: 369 RVA: 0x000066DF File Offset: 0x000048DF
		// (set) Token: 0x06000172 RID: 370 RVA: 0x000066E7 File Offset: 0x000048E7
		[XmlAttribute("Name")]
		public string Name { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000173 RID: 371 RVA: 0x000066F0 File Offset: 0x000048F0
		// (set) Token: 0x06000174 RID: 372 RVA: 0x000066F8 File Offset: 0x000048F8
		[XmlAttribute("Suite")]
		public string Suite { get; set; }
	}
}
