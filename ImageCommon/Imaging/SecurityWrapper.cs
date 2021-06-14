using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000035 RID: 53
	public class SecurityWrapper : IPayloadWrapper
	{
		// Token: 0x06000229 RID: 553 RVA: 0x000136AF File Offset: 0x000118AF
		public SecurityWrapper(FullFlashUpdateImage ffuImage, IPayloadWrapper innerWrapper)
		{
			this.ffuImage = ffuImage;
			this.innerWrapper = innerWrapper;
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600022A RID: 554 RVA: 0x000136C5 File Offset: 0x000118C5
		// (set) Token: 0x0600022B RID: 555 RVA: 0x000136CD File Offset: 0x000118CD
		public byte[] CatalogData { get; private set; }

		// Token: 0x0600022C RID: 556 RVA: 0x000136D8 File Offset: 0x000118D8
		public void InitializeWrapper(long payloadSize)
		{
			if (payloadSize % (long)((ulong)this.ffuImage.ChunkSizeInBytes) != 0L)
			{
				throw new ImageCommonException("Data size not aligned with hash chunk size.");
			}
			this.sha = new SHA256CryptoServiceProvider();
			this.sha.Initialize();
			this.bytesHashed = 0;
			this.hashOffset = 0;
			uint num = (uint)(payloadSize / (long)((ulong)this.ffuImage.ChunkSizeInBytes)) * (uint)(this.sha.HashSize / 8);
			this.hashData = new byte[num];
			this.CatalogData = ImageSigner.GenerateCatalogFile(this.hashData);
			byte[] securityHeader = this.ffuImage.GetSecurityHeader(this.CatalogData, this.hashData);
			this.innerWrapper.InitializeWrapper(payloadSize + (long)securityHeader.Length);
			this.innerWrapper.Write(securityHeader);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00013794 File Offset: 0x00011994
		public void ResetPosition()
		{
			this.innerWrapper.ResetPosition();
		}

		// Token: 0x0600022E RID: 558 RVA: 0x000137A1 File Offset: 0x000119A1
		public void Write(byte[] data)
		{
			this.HashBufferAsync(data);
			this.innerWrapper.Write(data);
		}

		// Token: 0x0600022F RID: 559 RVA: 0x000137B8 File Offset: 0x000119B8
		public void FinalizeWrapper()
		{
			this.hashTask.Wait();
			this.hashTask = null;
			if (this.hashOffset != this.hashData.Length)
			{
				throw new ImageCommonException(string.Format("Failed to hash all data in the stream. hashOffset = {0}, hashData.Length = {1}, bytesHashed = {2}.", this.hashOffset, this.hashData.Length, this.bytesHashed));
			}
			this.CatalogData = ImageSigner.GenerateCatalogFile(this.hashData);
			byte[] securityHeader = this.ffuImage.GetSecurityHeader(this.CatalogData, this.hashData);
			this.innerWrapper.ResetPosition();
			this.innerWrapper.Write(securityHeader);
			this.ffuImage.CatalogData = this.CatalogData;
			this.ffuImage.HashTableData = this.hashData;
			this.innerWrapper.FinalizeWrapper();
		}

		// Token: 0x06000230 RID: 560 RVA: 0x00013888 File Offset: 0x00011A88
		private void HashBufferAsync(byte[] data)
		{
			if (this.hashTask != null)
			{
				this.hashTask.Wait();
			}
			this.hashTask = Task.Factory.StartNew(delegate()
			{
				this.HashBuffer(data);
			});
		}

		// Token: 0x06000231 RID: 561 RVA: 0x000138D8 File Offset: 0x00011AD8
		private void HashBuffer(byte[] data)
		{
			int chunkSizeInBytes = (int)this.ffuImage.ChunkSizeInBytes;
			int num = chunkSizeInBytes - this.bytesHashed;
			for (int i = 0; i < data.Length; i += chunkSizeInBytes)
			{
				int num2 = num;
				if (data.Length - i < num)
				{
					num2 = data.Length;
				}
				byte[] hash = this.sha.ComputeHash(data, i, num2);
				this.bytesHashed += num2;
				this.bytesHashed %= chunkSizeInBytes;
				if (this.bytesHashed == 0)
				{
					this.CommitHashToTable(hash);
				}
				num = chunkSizeInBytes;
			}
		}

		// Token: 0x06000232 RID: 562 RVA: 0x00013954 File Offset: 0x00011B54
		private void CommitHashToTable(byte[] hash)
		{
			hash.CopyTo(this.hashData, this.hashOffset);
			this.hashOffset += hash.Length;
		}

		// Token: 0x04000173 RID: 371
		private IPayloadWrapper innerWrapper;

		// Token: 0x04000174 RID: 372
		private FullFlashUpdateImage ffuImage;

		// Token: 0x04000175 RID: 373
		private Task hashTask;

		// Token: 0x04000176 RID: 374
		private SHA256 sha;

		// Token: 0x04000177 RID: 375
		private byte[] hashData;

		// Token: 0x04000178 RID: 376
		private int bytesHashed;

		// Token: 0x04000179 RID: 377
		private int hashOffset;
	}
}
