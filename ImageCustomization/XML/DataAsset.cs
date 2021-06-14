using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x02000011 RID: 17
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class DataAsset : IDefinedIn
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00006DA1 File Offset: 0x00004FA1
		// (set) Token: 0x0600010F RID: 271 RVA: 0x00006DA9 File Offset: 0x00004FA9
		[XmlIgnore]
		public string DefinedInFile { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000110 RID: 272 RVA: 0x00006DB2 File Offset: 0x00004FB2
		// (set) Token: 0x06000111 RID: 273 RVA: 0x00006DBA File Offset: 0x00004FBA
		[XmlAttribute]
		public string Source { get; set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000112 RID: 274 RVA: 0x00006DC3 File Offset: 0x00004FC3
		[XmlIgnore]
		public string ExpandedSourcePath
		{
			get
			{
				return ImageCustomizations.ExpandPath(this.Source);
			}
		}

		// Token: 0x04000059 RID: 89
		[XmlIgnore]
		public static readonly string SourceFieldName = Strings.txtDataAssetSource;
	}
}
