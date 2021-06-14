using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200002E RID: 46
	internal class MetadataPartitionEntry
	{
		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060001AB RID: 427 RVA: 0x0000849B File Offset: 0x0000669B
		// (set) Token: 0x060001AC RID: 428 RVA: 0x000084A3 File Offset: 0x000066A3
		public Guid PartitionId { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060001AD RID: 429 RVA: 0x000084AC File Offset: 0x000066AC
		// (set) Token: 0x060001AE RID: 430 RVA: 0x000084B4 File Offset: 0x000066B4
		public string Name { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060001AF RID: 431 RVA: 0x000084BD File Offset: 0x000066BD
		// (set) Token: 0x060001B0 RID: 432 RVA: 0x000084C5 File Offset: 0x000066C5
		public ulong DiskOffset { get; set; }

		// Token: 0x060001B1 RID: 433 RVA: 0x000084D0 File Offset: 0x000066D0
		public void ReadFromStream(BinaryReader reader)
		{
			this.DiskOffset = reader.ReadUInt64();
			byte[] bytes = reader.ReadBytes(MetadataPartitionEntry.PartitionNameLength * 2);
			string @string = Encoding.Unicode.GetString(bytes);
			this.Name = @string.Substring(0, @string.IndexOf('\0'));
			this.PartitionId = new Guid(reader.ReadBytes(MetadataPartitionEntry.BytesPerGuid));
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00008530 File Offset: 0x00006730
		public void LogInfo(IULogger logger, ushort indentLevel = 0)
		{
			new StringBuilder().Append(' ', (int)indentLevel).ToString();
			logger.LogInfo(indentLevel + "Name        : {0}", new object[]
			{
				this.Name
			});
			logger.LogInfo(indentLevel + "Partition Id: {0}", new object[]
			{
				this.PartitionId
			});
			logger.LogInfo(indentLevel + "Disk Offset : {0}", new object[]
			{
				this.DiskOffset
			});
		}

		// Token: 0x04000132 RID: 306
		public static int PartitionNameLength = 36;

		// Token: 0x04000133 RID: 307
		public static int BytesPerGuid = 16;
	}
}
