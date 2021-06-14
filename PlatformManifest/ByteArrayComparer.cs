using System;
using System.Collections.Generic;

namespace Microsoft.SecureBoot
{
	// Token: 0x02000005 RID: 5
	public class ByteArrayComparer : IEqualityComparer<byte[]>
	{
		// Token: 0x06000019 RID: 25 RVA: 0x000024D8 File Offset: 0x000006D8
		public bool Equals(byte[] left, byte[] right)
		{
			if (left == null || right == null)
			{
				return left == right;
			}
			if (left.Length != right.Length)
			{
				return false;
			}
			for (int i = 0; i < left.Length; i++)
			{
				if (left[i] != right[i])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002514 File Offset: 0x00000714
		public int GetHashCode(byte[] key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = 0;
			foreach (byte b in key)
			{
				num += (int)b;
			}
			return num;
		}
	}
}
