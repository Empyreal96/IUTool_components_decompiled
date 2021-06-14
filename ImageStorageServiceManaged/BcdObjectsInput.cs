using System;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200005F RID: 95
	[XmlType("Objects")]
	public class BcdObjectsInput
	{
		// Token: 0x06000456 RID: 1110 RVA: 0x000138D6 File Offset: 0x00011AD6
		private BcdObjectsInput()
		{
			this.SaveKeyToRegistry = true;
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000457 RID: 1111 RVA: 0x000138E5 File Offset: 0x00011AE5
		// (set) Token: 0x06000458 RID: 1112 RVA: 0x000138ED File Offset: 0x00011AED
		[XmlElement("Object")]
		public BcdObjectInput[] Objects { get; set; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000459 RID: 1113 RVA: 0x000138F6 File Offset: 0x00011AF6
		// (set) Token: 0x0600045A RID: 1114 RVA: 0x000138FE File Offset: 0x00011AFE
		[XmlAttribute]
		public bool SaveKeyToRegistry { get; set; }

		// Token: 0x0600045B RID: 1115 RVA: 0x00013908 File Offset: 0x00011B08
		public void SaveAsRegFile(StreamWriter writer, string path)
		{
			string text = path + "\\Objects";
			if (this.SaveKeyToRegistry)
			{
				writer.WriteLine("[{0}]", text);
				writer.WriteLine();
			}
			BcdObjectInput[] objects = this.Objects;
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i].SaveAsRegFile(writer, text);
			}
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x0001395C File Offset: 0x00011B5C
		public void SaveAsRegData(BcdRegData bcdRegData, string path)
		{
			string text = path + "\\Objects";
			if (this.SaveKeyToRegistry)
			{
				bcdRegData.AddRegKey(text);
			}
			BcdObjectInput[] objects = this.Objects;
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i].SaveAsRegData(bcdRegData, text);
			}
		}
	}
}
