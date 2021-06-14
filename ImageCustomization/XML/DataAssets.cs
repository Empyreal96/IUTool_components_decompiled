using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x02000010 RID: 16
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class DataAssets
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000108 RID: 264 RVA: 0x00006D5D File Offset: 0x00004F5D
		// (set) Token: 0x06000109 RID: 265 RVA: 0x00006D65 File Offset: 0x00004F65
		[XmlAttribute]
		public CustomizationDataAssetType Type { get; set; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600010A RID: 266 RVA: 0x00006D6E File Offset: 0x00004F6E
		// (set) Token: 0x0600010B RID: 267 RVA: 0x00006D76 File Offset: 0x00004F76
		[XmlElement(ElementName = "DataAsset")]
		public List<DataAsset> Items { get; set; }

		// Token: 0x0600010C RID: 268 RVA: 0x00006D7F File Offset: 0x00004F7F
		public DataAssets()
		{
			this.Items = new List<DataAsset>();
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00006D92 File Offset: 0x00004F92
		public DataAssets(CustomizationDataAssetType type) : this()
		{
			this.Type = type;
		}
	}
}
