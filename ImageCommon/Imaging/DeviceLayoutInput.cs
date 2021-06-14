using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200001D RID: 29
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "DeviceLayout", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class DeviceLayoutInput
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600012B RID: 299 RVA: 0x0000D5E2 File Offset: 0x0000B7E2
		// (set) Token: 0x0600012C RID: 300 RVA: 0x0000D5EA File Offset: 0x0000B7EA
		[XmlElement("SectorSize")]
		[CLSCompliant(false)]
		public uint SectorSize
		{
			get
			{
				return this._sectorSize;
			}
			set
			{
				this._sectorSize = value;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600012D RID: 301 RVA: 0x0000D5F3 File Offset: 0x0000B7F3
		// (set) Token: 0x0600012E RID: 302 RVA: 0x0000D5FB File Offset: 0x0000B7FB
		[XmlElement("ChunkSize")]
		[CLSCompliant(false)]
		public uint ChunkSize
		{
			get
			{
				return this._chunkSize;
			}
			set
			{
				this._chunkSize = value;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600012F RID: 303 RVA: 0x0000D604 File Offset: 0x0000B804
		// (set) Token: 0x06000130 RID: 304 RVA: 0x0000D60C File Offset: 0x0000B80C
		[CLSCompliant(false)]
		[XmlIgnore]
		public uint DefaultPartitionByteAlignment { get; set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000131 RID: 305 RVA: 0x0000D615 File Offset: 0x0000B815
		// (set) Token: 0x06000132 RID: 306 RVA: 0x0000D61D File Offset: 0x0000B81D
		[XmlElement("VersionTag")]
		public string VersionTag { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000133 RID: 307 RVA: 0x0000D628 File Offset: 0x0000B828
		// (set) Token: 0x06000134 RID: 308 RVA: 0x0000D648 File Offset: 0x0000B848
		[XmlElement("DefaultPartitionByteAlignment")]
		public string DefaultPartitionByteAlignmentAsString
		{
			get
			{
				return this.DefaultPartitionByteAlignment.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				uint defaultPartitionByteAlignment = 0U;
				if (!InputHelpers.StringToUint(value, out defaultPartitionByteAlignment))
				{
					throw new ImageCommonException(string.Format("The default byte alignment cannot be parsed: {0}", value));
				}
				this.DefaultPartitionByteAlignment = defaultPartitionByteAlignment;
			}
		}

		// Token: 0x040000CC RID: 204
		[XmlArrayItem(ElementName = "Partition", Type = typeof(InputPartition), IsNullable = false)]
		[XmlArray]
		public InputPartition[] Partitions;

		// Token: 0x040000CD RID: 205
		private uint _sectorSize;

		// Token: 0x040000CE RID: 206
		private uint _chunkSize = 256U;
	}
}
