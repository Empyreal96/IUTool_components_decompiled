using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000066 RID: 102
	public class BcdElementValueTypeInput
	{
		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000487 RID: 1159 RVA: 0x00013D34 File Offset: 0x00011F34
		// (set) Token: 0x06000488 RID: 1160 RVA: 0x00013D3C File Offset: 0x00011F3C
		[XmlChoiceIdentifier("ValueIdentifier")]
		[XmlElement("StringValue", typeof(string))]
		[XmlElement("BooleanValue", typeof(bool))]
		[XmlElement("ObjectValue", typeof(string))]
		[XmlElement("ObjectListValue", typeof(BcdElementObjectListInput))]
		[XmlElement("IntegerValue", typeof(string))]
		[XmlElement("IntegerListValue", typeof(BcdElementIntegerListInput))]
		[XmlElement("DeviceValue", typeof(BcdElementDeviceInput))]
		public object ValueType { get; set; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000489 RID: 1161 RVA: 0x00013D45 File Offset: 0x00011F45
		// (set) Token: 0x0600048A RID: 1162 RVA: 0x00013D4D File Offset: 0x00011F4D
		[XmlIgnore]
		public ValueTypeChoice ValueIdentifier { get; set; }

		// Token: 0x0600048B RID: 1163 RVA: 0x00013D58 File Offset: 0x00011F58
		private static bool StringToUlong(string valueAsString, out ulong value)
		{
			bool result = true;
			int num = 0;
			if (valueAsString.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
			{
				num = 2;
			}
			if (!ulong.TryParse(valueAsString.Substring(num, valueAsString.Length - num), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value))
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x00013DA0 File Offset: 0x00011FA0
		public static void WriteIntegerValue(StreamWriter writer, string elementName, string valueAsString)
		{
			ulong value = 0UL;
			if (!BcdElementValueTypeInput.StringToUlong(valueAsString, out value))
			{
				throw new ImageStorageException(string.Format("{0}: Unable to parse value for element '{1}'.", MethodBase.GetCurrentMethod().Name, elementName));
			}
			byte[] array = new byte[8];
			MemoryStream memoryStream = null;
			BinaryWriter binaryWriter = null;
			try
			{
				memoryStream = new MemoryStream(array);
				binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write(value);
				for (int i = 0; i < array.Length; i++)
				{
					writer.Write("{0:x2}{1}", array[i], (i < array.Length - 1) ? "," : "");
				}
			}
			finally
			{
				if (binaryWriter != null)
				{
					binaryWriter.Flush();
					binaryWriter = null;
				}
				if (memoryStream != null)
				{
					memoryStream.Flush();
					memoryStream.Close();
					memoryStream = null;
				}
			}
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x00013E60 File Offset: 0x00012060
		public void WriteIntegerValue(BcdRegData bcdRegData, string path, string valueAsString)
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			BcdElementValueTypeInput.WriteIntegerValue(streamWriter, "", valueAsString);
			streamWriter.Flush();
			memoryStream.Position = 0L;
			string value = new StreamReader(memoryStream).ReadToEnd();
			bcdRegData.AddRegValue(path, "Element", value, "REG_BINARY");
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x00013EB0 File Offset: 0x000120B0
		public static void WriteByteArray(TextWriter writer, string elementName, string elementHeader, byte[] value)
		{
			writer.Write(elementHeader);
			int num = elementHeader.Length;
			int i = 0;
			while (i < value.Length - 1)
			{
				while (num < 80 && i < value.Length - 1)
				{
					writer.Write("{0:x2},", value[i++]);
					num += 3;
				}
				if (num >= 80)
				{
					if (i < value.Length - 1)
					{
						writer.WriteLine("\\");
					}
					num = 0;
				}
			}
			writer.WriteLine("{0:x2}", value[value.Length - 1]);
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x00013F30 File Offset: 0x00012130
		public static void WriteByteArray(BcdRegData bcdRegData, string path, byte[] value)
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			BcdElementValueTypeInput.WriteByteArray(streamWriter, "", "", value);
			streamWriter.Flush();
			memoryStream.Position = 0L;
			string value2 = new StreamReader(memoryStream).ReadToEnd();
			bcdRegData.AddRegValue(path, "Element", value2, "REG_BINARY");
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x00013F84 File Offset: 0x00012184
		public static void WriteObjectsValue(TextWriter writer, string elementName, string elementHeader, string objectsAsStrings)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(objectsAsStrings);
			BcdElementValueTypeInput.WriteByteArray(writer, elementName, elementHeader, bytes);
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x00013FA8 File Offset: 0x000121A8
		public void SaveAsRegData(BcdRegData bcdRegData, string path)
		{
			switch (this.ValueIdentifier)
			{
			case ValueTypeChoice.StringValue:
			{
				string value = new StringBuilder(this.ValueType as string).ToString();
				bcdRegData.AddRegValue(path, "Element", value, "REG_SZ");
				return;
			}
			case ValueTypeChoice.BooleanValue:
				bcdRegData.AddRegValue(path, "Element", string.Format("{0}", ((bool)this.ValueType) ? "01" : "00"), "REG_BINARY");
				return;
			case ValueTypeChoice.ObjectValue:
			{
				string value2 = string.Format("{{{0}}}", BcdObjects.IdFromName(this.ValueType as string).ToString());
				bcdRegData.AddRegValue(path, "Element", value2, "REG_SZ");
				return;
			}
			case ValueTypeChoice.ObjectListValue:
				(this.ValueType as BcdElementObjectListInput).SaveAsRegData(bcdRegData, path);
				return;
			case ValueTypeChoice.IntegerValue:
				this.WriteIntegerValue(bcdRegData, path, this.ValueType as string);
				return;
			case ValueTypeChoice.IntegerListValue:
				(this.ValueType as BcdElementIntegerListInput).SaveAsRegData(bcdRegData, path);
				return;
			case ValueTypeChoice.DeviceValue:
				(this.ValueType as BcdElementDeviceInput).SaveAsRegData(bcdRegData, path);
				return;
			default:
				throw new ImageStorageException(string.Format("{0}: Invalid value type for element '{1}'.", MethodBase.GetCurrentMethod().Name, path));
			}
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x000140E4 File Offset: 0x000122E4
		public void SaveAsRegFile(StreamWriter writer, string elementName)
		{
			switch (this.ValueIdentifier)
			{
			case ValueTypeChoice.StringValue:
			{
				StringBuilder stringBuilder = new StringBuilder(this.ValueType as string);
				for (int i = 0; i < stringBuilder.Length; i++)
				{
					if (stringBuilder[i] == '\\')
					{
						stringBuilder.Insert(i++, '\\');
					}
				}
				writer.WriteLine("\"Element\"=\"{0}\"", stringBuilder.ToString());
				writer.WriteLine();
				return;
			}
			case ValueTypeChoice.BooleanValue:
				writer.WriteLine("\"Element\"=hex:{0}", ((bool)this.ValueType) ? "01" : "00");
				writer.WriteLine();
				return;
			case ValueTypeChoice.ObjectValue:
			{
				string arg = string.Format("\"Element\"=\"{{{0}}}\"", BcdObjects.IdFromName(this.ValueType as string).ToString());
				writer.WriteLine("{0}", arg);
				writer.WriteLine();
				writer.Flush();
				return;
			}
			case ValueTypeChoice.ObjectListValue:
				(this.ValueType as BcdElementObjectListInput).SaveAsRegFile(writer, elementName);
				return;
			case ValueTypeChoice.IntegerValue:
				writer.Write("\"Element\"=hex:");
				BcdElementValueTypeInput.WriteIntegerValue(writer, elementName, this.ValueType as string);
				writer.WriteLine();
				writer.WriteLine();
				return;
			case ValueTypeChoice.IntegerListValue:
				(this.ValueType as BcdElementIntegerListInput).SaveAsRegFile(writer, elementName);
				return;
			case ValueTypeChoice.DeviceValue:
				(this.ValueType as BcdElementDeviceInput).SaveAsRegFile(writer, elementName);
				return;
			default:
				throw new ImageStorageException(string.Format("{0}: Invalid value type for element '{1}'.", MethodBase.GetCurrentMethod().Name, elementName));
			}
		}
	}
}
