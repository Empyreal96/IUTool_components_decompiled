using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x02000013 RID: 19
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class Import : IDefinedIn
	{
		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00007816 File Offset: 0x00005A16
		// (set) Token: 0x0600013E RID: 318 RVA: 0x0000781E File Offset: 0x00005A1E
		[XmlIgnore]
		public string DefinedInFile { get; set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600013F RID: 319 RVA: 0x00007827 File Offset: 0x00005A27
		// (set) Token: 0x06000140 RID: 320 RVA: 0x0000782F File Offset: 0x00005A2F
		[XmlAttribute]
		public string Source { get; set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000141 RID: 321 RVA: 0x00007838 File Offset: 0x00005A38
		[XmlIgnore]
		public string ExpandedSourcePath
		{
			get
			{
				return ImageCustomizations.ExpandPath(this.Source);
			}
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00002230 File Offset: 0x00000430
		public Import()
		{
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00007845 File Offset: 0x00005A45
		public Import(string source)
		{
			this.Source = source;
		}

		// Token: 0x04000067 RID: 103
		[XmlIgnore]
		public const string SourceFieldName = "Import source";
	}
}
