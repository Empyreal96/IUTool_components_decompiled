using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy
{
	// Token: 0x02000056 RID: 86
	public static class HashCalculator
	{
		// Token: 0x060001B0 RID: 432 RVA: 0x0000A960 File Offset: 0x00008B60
		public static string CalculateSha1Hash(string Value)
		{
			return HashCalculator.CalculateHash(Value, HashCalculator.HashType.Sha1);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000A969 File Offset: 0x00008B69
		public static string CalculateSha256Hash(string Value)
		{
			return HashCalculator.CalculateHash(Value, HashCalculator.HashType.Sha256);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000A974 File Offset: 0x00008B74
		private static string CalculateHash(string Value, HashCalculator.HashType HashType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array;
			if (HashType == HashCalculator.HashType.Sha256)
			{
				array = HashCalculator.Sha256Hasher.ComputeHash(Encoding.Unicode.GetBytes(Value));
			}
			else
			{
				if (HashType != HashCalculator.HashType.Sha1)
				{
					throw new PkgGenException("Invalid hash algorithm");
				}
				array = HashCalculator.Sha1Hasher.ComputeHash(Encoding.Unicode.GetBytes(Value));
			}
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("X2", CultureInfo.InvariantCulture));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000110 RID: 272
		private static SHA256 Sha256Hasher = SHA256.Create();

		// Token: 0x04000111 RID: 273
		private static SHA1 Sha1Hasher = SHA1.Create();

		// Token: 0x0200006A RID: 106
		private enum HashType
		{
			// Token: 0x0400016E RID: 366
			Sha256,
			// Token: 0x0400016F RID: 367
			Sha1
		}
	}
}
