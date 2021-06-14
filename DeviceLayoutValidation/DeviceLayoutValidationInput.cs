using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000005 RID: 5
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "DeviceLayout", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class DeviceLayoutValidationInput
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002213 File Offset: 0x00000413
		[XmlArrayItem(ElementName = "Partition", Type = typeof(InputValidationPartition), IsNullable = false)]
		[XmlArray]
		public List<InputValidationPartition> Partitions
		{
			get
			{
				return this._partitions;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000033 RID: 51 RVA: 0x0000221B File Offset: 0x0000041B
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00002223 File Offset: 0x00000423
		public string Scope { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000035 RID: 53 RVA: 0x0000222C File Offset: 0x0000042C
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00002234 File Offset: 0x00000434
		public uint RulesSectorSize { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000037 RID: 55 RVA: 0x0000223D File Offset: 0x0000043D
		[XmlArrayItem(ElementName = "Scope", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		public List<string> ExcludedScopes
		{
			get
			{
				return this._excludedScopes;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002245 File Offset: 0x00000445
		// (set) Token: 0x06000039 RID: 57 RVA: 0x0000224D File Offset: 0x0000044D
		public string SectorSize { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002256 File Offset: 0x00000456
		// (set) Token: 0x0600003B RID: 59 RVA: 0x0000225E File Offset: 0x0000045E
		public string ChunkSize { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00002267 File Offset: 0x00000467
		// (set) Token: 0x0600003D RID: 61 RVA: 0x0000226F File Offset: 0x0000046F
		public string DefaultPartitionByteAlignment { get; set; }

		// Token: 0x04000025 RID: 37
		private List<InputValidationPartition> _partitions = new List<InputValidationPartition>();

		// Token: 0x04000026 RID: 38
		private List<string> _excludedScopes = new List<string>();
	}
}
