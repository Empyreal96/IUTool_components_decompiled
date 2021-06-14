using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000035 RID: 53
	internal class DataBlockSource
	{
		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000223 RID: 547 RVA: 0x00009934 File Offset: 0x00007B34
		// (set) Token: 0x06000224 RID: 548 RVA: 0x0000993C File Offset: 0x00007B3C
		public DataBlockSource.DataSource Source { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000225 RID: 549 RVA: 0x00009945 File Offset: 0x00007B45
		// (set) Token: 0x06000226 RID: 550 RVA: 0x0000994D File Offset: 0x00007B4D
		public ulong StorageOffset { get; set; }

		// Token: 0x06000227 RID: 551 RVA: 0x00009956 File Offset: 0x00007B56
		public void SetMemoryData(byte[] buffer, int bufferOffset, int blockSize)
		{
			this._memoryData = new byte[blockSize];
			Array.Copy(buffer, bufferOffset, this._memoryData, 0, blockSize);
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00009973 File Offset: 0x00007B73
		public void SetMemoryData(FileStream stream, int blockSize)
		{
			this._memoryData = new byte[blockSize];
			stream.Read(this._memoryData, 0, blockSize);
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00009990 File Offset: 0x00007B90
		public void CreateMemoryData(int blockSize)
		{
			this._memoryData = new byte[blockSize];
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000999E File Offset: 0x00007B9E
		public byte[] GetMemoryData()
		{
			return this._memoryData;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x000099A6 File Offset: 0x00007BA6
		public byte[] GetNewMemoryData(uint blockSize)
		{
			this._memoryData = new byte[blockSize];
			return this._memoryData;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x000099BC File Offset: 0x00007BBC
		public void LogInfo(IULogger logger, ushort indentLevel = 0)
		{
			string str = new StringBuilder().Append(' ', (int)indentLevel).ToString();
			logger.LogInfo(str + "Source           : {0}", new object[]
			{
				this.Source
			});
			if (this.Source == DataBlockSource.DataSource.Disk)
			{
				logger.LogInfo(str + "  : {0}", new object[]
				{
					this.StorageOffset
				});
			}
			logger.LogInfo("", new object[0]);
		}

		// Token: 0x0400015F RID: 351
		private byte[] _memoryData;

		// Token: 0x0200007B RID: 123
		public enum DataSource
		{
			// Token: 0x040002BA RID: 698
			Zero,
			// Token: 0x040002BB RID: 699
			Disk,
			// Token: 0x040002BC RID: 700
			Memory
		}
	}
}
