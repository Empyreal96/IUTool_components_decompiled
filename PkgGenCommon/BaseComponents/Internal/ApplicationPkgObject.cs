using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000055 RID: 85
	[XmlRoot(ElementName = "Application", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public class ApplicationPkgObject : AppResourcePkgObject
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600016C RID: 364 RVA: 0x000066B5 File Offset: 0x000048B5
		// (set) Token: 0x0600016D RID: 365 RVA: 0x000066BD File Offset: 0x000048BD
		[XmlAnyElement("RequiredCapabilities")]
		public XElement RequiredCapabilities { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600016E RID: 366 RVA: 0x000066C6 File Offset: 0x000048C6
		// (set) Token: 0x0600016F RID: 367 RVA: 0x000066CE File Offset: 0x000048CE
		[XmlAnyElement("PrivateResources")]
		public XElement PrivateResources { get; set; }
	}
}
