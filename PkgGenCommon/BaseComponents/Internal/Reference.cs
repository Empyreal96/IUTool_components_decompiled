using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x0200005F RID: 95
	[XmlRoot(ElementName = "Reference", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public sealed class Reference
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x00006D6F File Offset: 0x00004F6F
		// (set) Token: 0x060001B1 RID: 433 RVA: 0x00006D77 File Offset: 0x00004F77
		[XmlAttribute("Source")]
		public string Source { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x00006D80 File Offset: 0x00004F80
		// (set) Token: 0x060001B3 RID: 435 RVA: 0x00006D88 File Offset: 0x00004F88
		[XmlAttribute("StagingSubDir")]
		public string StagingSubDir { get; set; }

		// Token: 0x060001B4 RID: 436 RVA: 0x00003E08 File Offset: 0x00002008
		internal Reference()
		{
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00006D91 File Offset: 0x00004F91
		internal Reference(string source, string stagingSubDir)
		{
			this.Source = source;
			this.StagingSubDir = stagingSubDir;
		}
	}
}
