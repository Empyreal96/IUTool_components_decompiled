using System;
using System.Collections.Generic;

namespace Microsoft.MetadataReader.Internal
{
	// Token: 0x02000003 RID: 3
	internal class HashSet<T>
	{
		// Token: 0x06000007 RID: 7 RVA: 0x000020E8 File Offset: 0x000002E8
		public bool Contains(T element)
		{
			return this._contents.ContainsKey(element);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002106 File Offset: 0x00000306
		public void Add(T element)
		{
			this._contents[element] = null;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002118 File Offset: 0x00000318
		public int Count
		{
			get
			{
				return this._contents.Count;
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002138 File Offset: 0x00000338
		public void CopyTo(T[] array)
		{
			int num = 0;
			foreach (KeyValuePair<T, object> keyValuePair in this._contents)
			{
				array[num] = keyValuePair.Key;
				num++;
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000021A0 File Offset: 0x000003A0
		public void UnionWith(IEnumerable<T> other)
		{
			foreach (T element in other)
			{
				this.Add(element);
			}
		}

		// Token: 0x04000001 RID: 1
		private readonly Dictionary<T, object> _contents = new Dictionary<T, object>();
	}
}
