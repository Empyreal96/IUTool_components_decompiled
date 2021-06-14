using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200002D RID: 45
	internal class MetadataPartitionHeader
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x000083B2 File Offset: 0x000065B2
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x000083BA File Offset: 0x000065BA
		public uint Signature { get; private set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x000083C3 File Offset: 0x000065C3
		// (set) Token: 0x060001A5 RID: 421 RVA: 0x000083CB File Offset: 0x000065CB
		public uint MaxPartitionCount { get; private set; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x000083D4 File Offset: 0x000065D4
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x000083DC File Offset: 0x000065DC
		public uint PartitionCount { get; private set; }

		// Token: 0x060001A8 RID: 424 RVA: 0x000083E5 File Offset: 0x000065E5
		public void ReadFromStream(BinaryReader reader)
		{
			this.Signature = reader.ReadUInt32();
			this.MaxPartitionCount = reader.ReadUInt32();
			this.PartitionCount = reader.ReadUInt32();
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000840C File Offset: 0x0000660C
		public void LogInfo(IULogger logger, ushort indentLevel = 0)
		{
			string str = new StringBuilder().Append(' ', (int)indentLevel).ToString();
			logger.LogInfo(str + "Signature          : {0}", new object[]
			{
				this.Signature
			});
			logger.LogInfo(str + "Max Partition Count: {0}", new object[]
			{
				this.MaxPartitionCount
			});
			logger.LogInfo(str + "Partition Count    : {0}", new object[]
			{
				this.PartitionCount
			});
		}
	}
}
