using System;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000069 RID: 105
	public class BcdElementIntegerListInput
	{
		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060004A1 RID: 1185 RVA: 0x0001450F File Offset: 0x0001270F
		// (set) Token: 0x060004A2 RID: 1186 RVA: 0x00014517 File Offset: 0x00012717
		[XmlArrayItem(ElementName = "StringValue", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		public string[] StringValues { get; set; }

		// Token: 0x060004A3 RID: 1187 RVA: 0x00014520 File Offset: 0x00012720
		public void SaveAsRegFile(StreamWriter writer, string elementName)
		{
			writer.Write("\"Element\"=hex:");
			for (int i = 0; i < this.StringValues.Length; i++)
			{
				BcdElementValueTypeInput.WriteIntegerValue(writer, elementName, this.StringValues[i]);
				if (i < this.StringValues.Length - 1)
				{
					writer.Write(",");
				}
			}
			writer.WriteLine();
			writer.WriteLine();
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x00014580 File Offset: 0x00012780
		public void SaveAsRegData(BcdRegData bcdRegData, string path)
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			for (int i = 0; i < this.StringValues.Length; i++)
			{
				BcdElementValueTypeInput.WriteIntegerValue(streamWriter, "", this.StringValues[i]);
			}
			streamWriter.Flush();
			memoryStream.Position = 0L;
			string value = new StreamReader(memoryStream).ReadToEnd();
			bcdRegData.AddRegValue(path, "Element", value, "REG_BINARY");
		}
	}
}
