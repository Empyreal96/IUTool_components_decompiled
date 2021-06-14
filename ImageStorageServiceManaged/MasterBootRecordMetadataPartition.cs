using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200002C RID: 44
	internal class MasterBootRecordMetadataPartition
	{
		// Token: 0x0600019B RID: 411 RVA: 0x00008283 File Offset: 0x00006483
		private MasterBootRecordMetadataPartition()
		{
		}

		// Token: 0x0600019C RID: 412 RVA: 0x000082A1 File Offset: 0x000064A1
		public MasterBootRecordMetadataPartition(IULogger logger)
		{
			this._logger = logger;
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600019D RID: 413 RVA: 0x000082C6 File Offset: 0x000064C6
		public List<MetadataPartitionEntry> Entries
		{
			get
			{
				return this._entries;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600019E RID: 414 RVA: 0x000082CE File Offset: 0x000064CE
		public MetadataPartitionHeader Header
		{
			get
			{
				return this._header;
			}
		}

		// Token: 0x0600019F RID: 415 RVA: 0x000082D8 File Offset: 0x000064D8
		public void ReadFromStream(Stream stream)
		{
			BinaryReader reader = new BinaryReader(stream);
			this._header.ReadFromStream(reader);
			for (uint num = 0U; num < this._header.PartitionCount; num += 1U)
			{
				MetadataPartitionEntry metadataPartitionEntry = new MetadataPartitionEntry();
				metadataPartitionEntry.ReadFromStream(reader);
				this._entries.Add(metadataPartitionEntry);
			}
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000832C File Offset: 0x0000652C
		public void LogInfo(ushort indentLevel = 0)
		{
			this._header.LogInfo(this._logger, indentLevel + 2);
			foreach (MetadataPartitionEntry metadataPartitionEntry in this._entries)
			{
				metadataPartitionEntry.LogInfo(this._logger, indentLevel + 2);
			}
		}

		// Token: 0x0400012A RID: 298
		public static byte PartitonType = ImageConstants.MBR_METADATA_PARTITION_TYPE;

		// Token: 0x0400012B RID: 299
		public static uint HeaderSignature = 524289U;

		// Token: 0x0400012C RID: 300
		private IULogger _logger;

		// Token: 0x0400012D RID: 301
		private MetadataPartitionHeader _header = new MetadataPartitionHeader();

		// Token: 0x0400012E RID: 302
		private List<MetadataPartitionEntry> _entries = new List<MetadataPartitionEntry>();
	}
}
