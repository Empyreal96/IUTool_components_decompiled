using System;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200001E RID: 30
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate/v2")]
	[XmlRoot(ElementName = "DeviceLayout", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate/v2", IsNullable = false)]
	public class DeviceLayoutInputv2
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000136 RID: 310 RVA: 0x0000D68C File Offset: 0x0000B88C
		// (set) Token: 0x06000137 RID: 311 RVA: 0x0000D694 File Offset: 0x0000B894
		[XmlElement("SectorSize")]
		public uint SectorSize { get; set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000138 RID: 312 RVA: 0x0000D69D File Offset: 0x0000B89D
		// (set) Token: 0x06000139 RID: 313 RVA: 0x0000D6A5 File Offset: 0x0000B8A5
		[XmlElement("ChunkSize")]
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

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600013A RID: 314 RVA: 0x0000D6AE File Offset: 0x0000B8AE
		// (set) Token: 0x0600013B RID: 315 RVA: 0x0000D6B6 File Offset: 0x0000B8B6
		[XmlIgnore]
		public uint DefaultPartitionByteAlignment { get; set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600013C RID: 316 RVA: 0x0000D6BF File Offset: 0x0000B8BF
		// (set) Token: 0x0600013D RID: 317 RVA: 0x0000D6C7 File Offset: 0x0000B8C7
		[XmlElement("VersionTag")]
		public string VersionTag { get; set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600013E RID: 318 RVA: 0x0000D6D0 File Offset: 0x0000B8D0
		// (set) Token: 0x0600013F RID: 319 RVA: 0x0000D6F0 File Offset: 0x0000B8F0
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

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000140 RID: 320 RVA: 0x0000D721 File Offset: 0x0000B921
		public InputStore MainOSStore
		{
			get
			{
				return this.Stores.FirstOrDefault((InputStore x) => x.IsMainOSStore());
			}
		}

		// Token: 0x040000D1 RID: 209
		[XmlArrayItem(ElementName = "Store", Type = typeof(InputStore), IsNullable = false)]
		[XmlArray]
		public InputStore[] Stores;

		// Token: 0x040000D3 RID: 211
		private uint _chunkSize = 256U;
	}
}
