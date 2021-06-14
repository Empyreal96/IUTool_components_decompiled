using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000008 RID: 8
	public static class HashCalculator
	{
		// Token: 0x06000017 RID: 23 RVA: 0x00002406 File Offset: 0x00000606
		public static string CalculateSha1Hash(string value)
		{
			return HashCalculator.CalculateHash(value, HashCalculator.HashType.Sha1);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000240F File Offset: 0x0000060F
		public static string CalculateSha256Hash(string value, bool forceNormalize)
		{
			if (forceNormalize)
			{
				return HashCalculator.CalculateHash(NormalizedString.Get(value), HashCalculator.HashType.Sha256);
			}
			return HashCalculator.CalculateHash(value, HashCalculator.HashType.Sha256);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002428 File Offset: 0x00000628
		private static string CalculateHash(string value, HashCalculator.HashType hashType)
		{
			byte[] array = null;
			StringBuilder stringBuilder = new StringBuilder(null);
			try
			{
				if (hashType == HashCalculator.HashType.Sha256)
				{
					array = HashCalculator.Sha256Hasher.ComputeHash(Encoding.Unicode.GetBytes(value));
				}
				if (hashType == HashCalculator.HashType.Sha1)
				{
					array = HashCalculator.Sha1Hasher.ComputeHash(Encoding.Unicode.GetBytes(value));
				}
			}
			catch (ArgumentNullException originalException)
			{
				throw new PolicyCompilerInternalException("SecurityPolicyCompiler Internal Error: Calculating Hash for: " + value, originalException);
			}
			catch (ObjectDisposedException originalException2)
			{
				throw new PolicyCompilerInternalException("SecurityPolicyCompiler Internal Error: Calculating Hash for: " + value, originalException2);
			}
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("X2", GlobalVariables.Culture));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x040000AF RID: 175
		private static SHA256 Sha256Hasher = SHA256.Create();

		// Token: 0x040000B0 RID: 176
		private static SHA1 Sha1Hasher = SHA1.Create();

		// Token: 0x02000047 RID: 71
		private enum HashType
		{
			// Token: 0x0400014F RID: 335
			Sha256,
			// Token: 0x04000150 RID: 336
			Sha1
		}
	}
}
