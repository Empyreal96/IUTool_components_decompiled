using System;
using System.Security.Cryptography;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000026 RID: 38
	public class CRC32 : HashAlgorithm
	{
		// Token: 0x06000119 RID: 281 RVA: 0x00006370 File Offset: 0x00004570
		static CRC32()
		{
			for (uint num = 0U; num < 256U; num += 1U)
			{
				uint num2 = num;
				for (int i = 0; i < 8; i++)
				{
					if ((num2 & 1U) != 0U)
					{
						num2 = (3988292384U ^ num2 >> 1);
					}
					else
					{
						num2 >>= 1;
					}
				}
				CRC32._crc32Table[(int)num] = num2;
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x000063C7 File Offset: 0x000045C7
		public CRC32()
		{
			this.InitializeVariables();
		}

		// Token: 0x0600011B RID: 283 RVA: 0x000063D5 File Offset: 0x000045D5
		public override void Initialize()
		{
			this.InitializeVariables();
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000063DD File Offset: 0x000045DD
		private void InitializeVariables()
		{
			this._crc32Value = uint.MaxValue;
			this._hashCoreCalled = false;
			this._hashFinalCalled = false;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x000063F4 File Offset: 0x000045F4
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (this._hashFinalCalled)
			{
				throw new CryptographicException("Hash not valid for use in specified state.");
			}
			this._hashCoreCalled = true;
			for (int i = ibStart; i < ibStart + cbSize; i++)
			{
				byte b = (byte)(this._crc32Value ^ (uint)array[i]);
				this._crc32Value = (CRC32._crc32Table[(int)b] ^ (this._crc32Value >> 8 & 16777215U));
			}
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000645F File Offset: 0x0000465F
		protected override byte[] HashFinal()
		{
			this._hashFinalCalled = true;
			return this.Hash;
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600011F RID: 287 RVA: 0x0000646E File Offset: 0x0000466E
		public override byte[] Hash
		{
			get
			{
				if (!this._hashCoreCalled)
				{
					throw new NullReferenceException();
				}
				if (!this._hashFinalCalled)
				{
					throw new CryptographicException("Hash must be finalized before the hash value is retrieved.");
				}
				byte[] bytes = BitConverter.GetBytes(~this._crc32Value);
				Array.Reverse(bytes);
				return bytes;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000120 RID: 288 RVA: 0x000064A3 File Offset: 0x000046A3
		public override int HashSize
		{
			get
			{
				return 32;
			}
		}

		// Token: 0x040000F3 RID: 243
		private static readonly uint[] _crc32Table = new uint[256];

		// Token: 0x040000F4 RID: 244
		private uint _crc32Value;

		// Token: 0x040000F5 RID: 245
		private bool _hashCoreCalled;

		// Token: 0x040000F6 RID: 246
		private bool _hashFinalCalled;
	}
}
