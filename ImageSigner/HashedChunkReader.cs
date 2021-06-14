using System;
using System.IO;
using System.Security.Cryptography;

namespace Microsoft.WindowsPhone.Imaging.ImageSignerApp
{
	// Token: 0x02000002 RID: 2
	internal class HashedChunkReader
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public HashedChunkReader(byte[] hashData, FileStream imageData, uint chunkSize, long firstChunkOffset)
		{
			imageData.Seek(firstChunkOffset, SeekOrigin.Begin);
			this.hashData = hashData;
			this.chunkSize = (int)chunkSize;
			this.imageData = imageData;
			this.hasher = new SHA256Cng();
			this.curOffset = 0;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000208C File Offset: 0x0000028C
		public byte[] GetNextChunk()
		{
			byte[] array = new byte[this.chunkSize];
			if (this.imageData.Length - this.imageData.Position < (long)array.Length)
			{
				throw new HashedChunkReaderException("Unabled to read next chunk: insufficient image data remaining.  Image may be truncated.");
			}
			this.imageData.Read(array, 0, array.Length);
			byte[] array2 = this.hasher.ComputeHash(array);
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i] != this.hashData[this.curOffset])
				{
					throw new HashedChunkReaderException(string.Format("Hash data mismatch at table offset 0x{0:x}.  The hash data does not match the image data, indicating corruption.", this.curOffset));
				}
				this.curOffset++;
			}
			return array;
		}

		// Token: 0x04000001 RID: 1
		private byte[] hashData;

		// Token: 0x04000002 RID: 2
		private FileStream imageData;

		// Token: 0x04000003 RID: 3
		private int curOffset;

		// Token: 0x04000004 RID: 4
		private int chunkSize;

		// Token: 0x04000005 RID: 5
		private SHA256 hasher;
	}
}
