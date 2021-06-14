using System;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000062 RID: 98
	public class BcdObjectInput
	{
		// Token: 0x0600046F RID: 1135 RVA: 0x00013ADF File Offset: 0x00011CDF
		private BcdObjectInput()
		{
			this.SaveKeyToRegistry = true;
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x00013AEE File Offset: 0x00011CEE
		// (set) Token: 0x06000471 RID: 1137 RVA: 0x00013AF6 File Offset: 0x00011CF6
		public string FriendlyName { get; set; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000472 RID: 1138 RVA: 0x00013AFF File Offset: 0x00011CFF
		// (set) Token: 0x06000473 RID: 1139 RVA: 0x00013B07 File Offset: 0x00011D07
		public int RawType { get; set; }

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000474 RID: 1140 RVA: 0x00013B10 File Offset: 0x00011D10
		// (set) Token: 0x06000475 RID: 1141 RVA: 0x00013B18 File Offset: 0x00011D18
		[XmlElement("Id")]
		public string IdAsString { get; set; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000476 RID: 1142 RVA: 0x00013B21 File Offset: 0x00011D21
		// (set) Token: 0x06000477 RID: 1143 RVA: 0x00013B29 File Offset: 0x00011D29
		public BcdElementsInput Elements { get; set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000478 RID: 1144 RVA: 0x00013B32 File Offset: 0x00011D32
		[CLSCompliant(false)]
		[XmlIgnore]
		public uint ObjectType
		{
			get
			{
				return BcdObjects.ObjectTypeFromName(this.FriendlyName);
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000479 RID: 1145 RVA: 0x00013B3F File Offset: 0x00011D3F
		[XmlIgnore]
		public Guid Id
		{
			get
			{
				return BcdObjects.IdFromName(this.FriendlyName);
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x0600047A RID: 1146 RVA: 0x00013B4C File Offset: 0x00011D4C
		// (set) Token: 0x0600047B RID: 1147 RVA: 0x00013B54 File Offset: 0x00011D54
		[XmlAttribute]
		public bool SaveKeyToRegistry { get; set; }

		// Token: 0x0600047C RID: 1148 RVA: 0x00013B60 File Offset: 0x00011D60
		public void SaveAsRegFile(StreamWriter writer, string path)
		{
			string text = string.Format("{0}\\{{{1}}}", path, this.Id);
			if (this.SaveKeyToRegistry)
			{
				writer.WriteLine("[{0}]", text);
				writer.WriteLine();
				writer.WriteLine("[{0}\\Description]", text);
				writer.WriteLine("\"Type\"=dword:{0:x8}", this.ObjectType);
				writer.WriteLine();
			}
			this.Elements.SaveAsRegFile(writer, text);
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x00013BD4 File Offset: 0x00011DD4
		public void SaveAsRegData(BcdRegData bcdRegData, string path)
		{
			string text = string.Format("{0}\\{{{1}}}", path, this.Id);
			if (this.SaveKeyToRegistry)
			{
				string regKey = string.Format("{0}\\Description", text);
				bcdRegData.AddRegKey(text);
				bcdRegData.AddRegKey(regKey);
				bcdRegData.AddRegValue(regKey, "Type", string.Format("{0:x8}", this.ObjectType), "REG_DWORD");
			}
			this.Elements.SaveAsRegData(bcdRegData, text);
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x00013C4D File Offset: 0x00011E4D
		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.FriendlyName))
			{
				return this.FriendlyName;
			}
			return base.ToString();
		}
	}
}
