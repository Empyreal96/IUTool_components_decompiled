using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000064 RID: 100
	public class BcdElementDataTypeInput
	{
		// Token: 0x1700010D RID: 269
		// (get) Token: 0x0600047F RID: 1151 RVA: 0x00013C69 File Offset: 0x00011E69
		// (set) Token: 0x06000480 RID: 1152 RVA: 0x00013C71 File Offset: 0x00011E71
		[XmlChoiceIdentifier("TypeIdentifier")]
		[XmlElement("WellKnownType", typeof(string))]
		[XmlElement("RawType", typeof(string))]
		public object DataType { get; set; }

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000481 RID: 1153 RVA: 0x00013C7A File Offset: 0x00011E7A
		// (set) Token: 0x06000482 RID: 1154 RVA: 0x00013C82 File Offset: 0x00011E82
		[XmlIgnore]
		public DataTypeChoice TypeIdentifier { get; set; }

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000483 RID: 1155 RVA: 0x00013C8C File Offset: 0x00011E8C
		[XmlIgnore]
		public BcdElementDataType Type
		{
			get
			{
				if (this.TypeIdentifier != DataTypeChoice.WellKnownType)
				{
					throw new ImageStorageException(string.Format("{0}: Only WellKnownTypes are currently supported.", MethodBase.GetCurrentMethod().Name));
				}
				BcdElementDataType wellKnownDataType = BcdElementDataTypes.GetWellKnownDataType(this.DataType as string);
				if (wellKnownDataType == null)
				{
					throw new ImageStorageException(string.Format("{0}: The element for well known type '{1}' cannot be translated.", MethodBase.GetCurrentMethod().Name, this.DataType as string));
				}
				return wellKnownDataType;
			}
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x00013CF3 File Offset: 0x00011EF3
		public void SaveAsRegFile(TextWriter writer, string path)
		{
			writer.WriteLine("[{0}\\{1:x8}]", path, this.Type.RawValue);
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x00013D11 File Offset: 0x00011F11
		public void SaveAsRegData(BcdRegData bcdRegData, string path)
		{
			bcdRegData.AddRegKey(string.Format("{0}\\{1:x8}", path, this.Type.RawValue));
		}
	}
}
