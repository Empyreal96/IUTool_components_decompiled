using System;
using System.Collections.Generic;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Images
{
	// Token: 0x02000007 RID: 7
	public class WPStore : FullFlashUpdateImage.FullFlashUpdateStore
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00005920 File Offset: 0x00003B20
		public WPStore(WPImage image)
		{
			this._image = image;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x0000592F File Offset: 0x00003B2F
		public void Initialize(FullFlashUpdateImage.FullFlashUpdateStore store)
		{
			this.Initialize(store.MinSectorCount, store.SectorSize);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00005943 File Offset: 0x00003B43
		[CLSCompliant(false)]
		public void Initialize(uint minSectorCount, uint sectorSize)
		{
			base.MinSectorCount = minSectorCount;
			base.SectorSize = sectorSize;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00005953 File Offset: 0x00003B53
		public new int PartitionCount
		{
			get
			{
				if (this._image == null)
				{
					return 0;
				}
				return this._image.PartitionCount;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000052 RID: 82 RVA: 0x0000596A File Offset: 0x00003B6A
		public new List<WPPartition> Partitions
		{
			get
			{
				if (this._image == null)
				{
					return null;
				}
				return this._image.Partitions;
			}
		}

		// Token: 0x04000028 RID: 40
		private WPImage _image;
	}
}
