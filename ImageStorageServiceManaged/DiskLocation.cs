using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000032 RID: 50
	internal class DiskLocation
	{
		// Token: 0x060001FA RID: 506 RVA: 0x000091BA File Offset: 0x000073BA
		public DiskLocation()
		{
			this.BlockIndex = 0U;
			this.AccessMethod = DiskLocation.DiskAccessMethod.DiskBegin;
		}

		// Token: 0x060001FB RID: 507 RVA: 0x000091D0 File Offset: 0x000073D0
		public DiskLocation(uint blockIndex)
		{
			this.BlockIndex = blockIndex;
			this.AccessMethod = DiskLocation.DiskAccessMethod.DiskBegin;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x000091E6 File Offset: 0x000073E6
		public DiskLocation(uint blockIndex, DiskLocation.DiskAccessMethod accessMethod)
		{
			this.BlockIndex = blockIndex;
			this.AccessMethod = accessMethod;
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060001FE RID: 510 RVA: 0x00009205 File Offset: 0x00007405
		// (set) Token: 0x060001FD RID: 509 RVA: 0x000091FC File Offset: 0x000073FC
		public DiskLocation.DiskAccessMethod AccessMethod { get; set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000200 RID: 512 RVA: 0x00009216 File Offset: 0x00007416
		// (set) Token: 0x060001FF RID: 511 RVA: 0x0000920D File Offset: 0x0000740D
		public uint BlockIndex { get; set; }

		// Token: 0x06000201 RID: 513 RVA: 0x0000921E File Offset: 0x0000741E
		public void Write(BinaryWriter writer)
		{
			writer.Write((uint)this.AccessMethod);
			writer.Write(this.BlockIndex);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00009238 File Offset: 0x00007438
		public void Read(BinaryReader reader)
		{
			this.AccessMethod = (DiskLocation.DiskAccessMethod)reader.ReadUInt32();
			this.BlockIndex = reader.ReadUInt32();
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00009254 File Offset: 0x00007454
		public void LogInfo(IULogger logger, ushort indentLevel = 0)
		{
			string str = new StringBuilder().Append(' ', (int)indentLevel).ToString();
			logger.LogInfo(str + "Access Method: {0}", new object[]
			{
				this.AccessMethod
			});
			logger.LogInfo(str + "Block Index  : {0}", new object[]
			{
				this.BlockIndex
			});
			logger.LogInfo("", new object[0]);
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000204 RID: 516 RVA: 0x000092CF File Offset: 0x000074CF
		public static int SizeInBytes
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x02000079 RID: 121
		public enum DiskAccessMethod : uint
		{
			// Token: 0x040002B2 RID: 690
			DiskBegin,
			// Token: 0x040002B3 RID: 691
			DiskEnd = 2U
		}
	}
}
