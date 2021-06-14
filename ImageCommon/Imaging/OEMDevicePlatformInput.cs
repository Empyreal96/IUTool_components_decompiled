using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200001F RID: 31
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "OEMDevicePlatform", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class OEMDevicePlatformInput
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000142 RID: 322 RVA: 0x0000D760 File Offset: 0x0000B960
		// (set) Token: 0x06000143 RID: 323 RVA: 0x0000D7C1 File Offset: 0x0000B9C1
		[XmlArrayItem(ElementName = "ID")]
		[XmlArray("DevicePlatformIDs")]
		public string[] DevicePlatformIDs
		{
			get
			{
				if (this.DevicePlatformID != null && this._idArray != null)
				{
					throw new ImageCommonException("Please specify either a DevicePlatformID or a group of DevicePlatformIDs in the device platform package, but not both.");
				}
				if (this.DevicePlatformID == null && this._idArray == null)
				{
					throw new ImageCommonException("Please specify either a DevicePlatformID or a group of DevicePlatformIDs in the device platform package. No platform ID is currently present.");
				}
				if (this.DevicePlatformID != null)
				{
					return new string[]
					{
						this.DevicePlatformID
					};
				}
				return this._idArray;
			}
			set
			{
				this._idArray = value;
			}
		}

		// Token: 0x040000D6 RID: 214
		[XmlElement("DevicePlatformID")]
		public string DevicePlatformID;

		// Token: 0x040000D7 RID: 215
		private string[] _idArray;

		// Token: 0x040000D8 RID: 216
		[XmlArrayItem(ElementName = "Name")]
		[XmlArray("CompressedPartitions")]
		public string[] CompressedPartitions;

		// Token: 0x040000D9 RID: 217
		[XmlArrayItem(ElementName = "Name")]
		[XmlArray("UncompressedPartitions")]
		public string[] UncompressedPartitions;

		// Token: 0x040000DA RID: 218
		[CLSCompliant(false)]
		public uint MinSectorCount;

		// Token: 0x040000DB RID: 219
		[CLSCompliant(false)]
		public uint AdditionalMainOSFreeSectorsRequest;

		// Token: 0x040000DC RID: 220
		[CLSCompliant(false)]
		public uint MMOSPartitionTotalSectorsOverride;

		// Token: 0x040000DD RID: 221
		[CLSCompliant(false)]
		[XmlElement("MainOSRTCDataReservedSectors")]
		public uint MainOSRTCDataReservedSectors;

		// Token: 0x040000DE RID: 222
		[XmlElement("Rules")]
		public InputRules Rules;
	}
}
