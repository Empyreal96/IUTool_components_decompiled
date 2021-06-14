using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000060 RID: 96
	[XmlRoot(ElementName = "Security", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public sealed class Security
	{
		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x00006DA7 File Offset: 0x00004FA7
		// (set) Token: 0x060001B7 RID: 439 RVA: 0x00006DAF File Offset: 0x00004FAF
		[XmlAttribute("InfSectionName")]
		public string InfSectionName { get; set; }

		// Token: 0x060001B8 RID: 440 RVA: 0x00003E08 File Offset: 0x00002008
		internal Security()
		{
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00006DB8 File Offset: 0x00004FB8
		internal Security(string infSectionName)
		{
			this.InfSectionName = infSectionName;
		}
	}
}
