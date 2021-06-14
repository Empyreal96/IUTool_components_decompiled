using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000068 RID: 104
	public class BcdElementObjectListInput
	{
		// Token: 0x17000114 RID: 276
		// (get) Token: 0x0600049C RID: 1180 RVA: 0x000143D5 File Offset: 0x000125D5
		// (set) Token: 0x0600049D RID: 1181 RVA: 0x000143DD File Offset: 0x000125DD
		[XmlArrayItem(ElementName = "StringValue", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		public string[] StringValues { get; set; }

		// Token: 0x0600049E RID: 1182 RVA: 0x000143E8 File Offset: 0x000125E8
		public void SaveAsRegFile(TextWriter writer, string elementName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.StringValues.Length; i++)
			{
				Guid guid = BcdObjects.IdFromName(this.StringValues[i]);
				stringBuilder.Append(string.Format("{{{0}}}", guid));
				stringBuilder.Append("\0");
			}
			stringBuilder.Append("\0");
			BcdElementValueTypeInput.WriteObjectsValue(writer, elementName, "\"Element\"=hex(7):", stringBuilder.ToString());
			foreach (string text in this.StringValues)
			{
				Guid guid2 = BcdObjects.IdFromName(text);
				writer.WriteLine(";Values={{{0}}}, \"{1}\"", guid2, text);
			}
			writer.WriteLine();
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x000144A0 File Offset: 0x000126A0
		public void SaveAsRegData(BcdRegData bcdRegData, string path)
		{
			string text = null;
			foreach (string objectName in this.StringValues)
			{
				text += "\"{";
				Guid guid = BcdObjects.IdFromName(objectName);
				text = text + guid + "}\",";
			}
			text = text.TrimEnd(new char[]
			{
				','
			});
			bcdRegData.AddRegValue(path, "Element", text, "REG_MULTI_SZ");
		}
	}
}
