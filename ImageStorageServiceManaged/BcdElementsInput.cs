using System;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000061 RID: 97
	[XmlType("Elements")]
	public class BcdElementsInput
	{
		// Token: 0x06000468 RID: 1128 RVA: 0x00013A14 File Offset: 0x00011C14
		private BcdElementsInput()
		{
			this.SaveKeyToRegistry = true;
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x00013A23 File Offset: 0x00011C23
		// (set) Token: 0x0600046A RID: 1130 RVA: 0x00013A2B File Offset: 0x00011C2B
		[XmlElement("Element")]
		public BcdElementInput[] Elements { get; set; }

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x0600046B RID: 1131 RVA: 0x00013A34 File Offset: 0x00011C34
		// (set) Token: 0x0600046C RID: 1132 RVA: 0x00013A3C File Offset: 0x00011C3C
		[XmlAttribute]
		public bool SaveKeyToRegistry { get; set; }

		// Token: 0x0600046D RID: 1133 RVA: 0x00013A48 File Offset: 0x00011C48
		public void SaveAsRegFile(StreamWriter writer, string path)
		{
			if (this.SaveKeyToRegistry)
			{
				writer.WriteLine("[{0}\\Elements]", path);
				writer.WriteLine();
			}
			BcdElementInput[] elements = this.Elements;
			for (int i = 0; i < elements.Length; i++)
			{
				elements[i].SaveAsRegFile(writer, path + "\\Elements");
			}
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x00013A98 File Offset: 0x00011C98
		public void SaveAsRegData(BcdRegData bcdRegData, string path)
		{
			if (this.SaveKeyToRegistry)
			{
				bcdRegData.AddRegKey(path);
			}
			string path2 = string.Format("{0}\\Elements", path);
			BcdElementInput[] elements = this.Elements;
			for (int i = 0; i < elements.Length; i++)
			{
				elements[i].SaveAsRegData(bcdRegData, path2);
			}
		}
	}
}
