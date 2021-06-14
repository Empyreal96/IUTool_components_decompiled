using System;
using System.Text;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200003F RID: 63
	internal static class StringBuilderPool
	{
		// Token: 0x06000469 RID: 1129 RVA: 0x0000EB34 File Offset: 0x0000CD34
		public static StringBuilder Get()
		{
			return StringBuilderPool.Get(128);
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x0000EB50 File Offset: 0x0000CD50
		public static StringBuilder Get(int capacity)
		{
			StringBuilder stringBuilder = null;
			object obj = StringBuilderPool.s_synclock;
			lock (obj)
			{
				for (int i = 0; i < StringBuilderPool.s_pool.Length; i++)
				{
					bool flag2 = StringBuilderPool.s_pool[i] != null;
					if (flag2)
					{
						stringBuilder = StringBuilderPool.s_pool[i];
						StringBuilderPool.s_pool[i] = null;
						break;
					}
				}
			}
			bool flag3 = stringBuilder == null;
			if (flag3)
			{
				stringBuilder = new StringBuilder(capacity);
			}
			stringBuilder.Length = 0;
			stringBuilder.EnsureCapacity(capacity);
			return stringBuilder;
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x0000EBF8 File Offset: 0x0000CDF8
		public static void Release(ref StringBuilder builder)
		{
			bool flag = builder != null && builder.Capacity < 4096;
			if (flag)
			{
				object obj = StringBuilderPool.s_synclock;
				lock (obj)
				{
					for (int i = 0; i < StringBuilderPool.s_pool.Length; i++)
					{
						bool flag3 = StringBuilderPool.s_pool[i] == null;
						if (flag3)
						{
							StringBuilderPool.s_pool[i] = builder;
							break;
						}
					}
				}
			}
			builder = null;
		}

		// Token: 0x040000DC RID: 220
		private const int DefaultCapacity = 128;

		// Token: 0x040000DD RID: 221
		private const int MaxListSize = 5;

		// Token: 0x040000DE RID: 222
		private const int MaxCapacity = 4096;

		// Token: 0x040000DF RID: 223
		private static readonly StringBuilder[] s_pool = new StringBuilder[5];

		// Token: 0x040000E0 RID: 224
		private static readonly object s_synclock = new object();
	}
}
