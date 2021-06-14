using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x0200002F RID: 47
	public class StringToEnum<T>
	{
		// Token: 0x0600020A RID: 522 RVA: 0x00008CEE File Offset: 0x00006EEE
		public void Add(T value, string name)
		{
			this._stringToValue.Add(name, value);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x00008D00 File Offset: 0x00006F00
		public T Parse(string name)
		{
			T result;
			if (!this._stringToValue.TryGetValue(name, out result))
			{
				throw new PackageException("Incorrect value '{0}' for type '{1}', possbile values are '{2}'", new object[]
				{
					name,
					typeof(T).Name,
					string.Join(",", this._stringToValue.Keys)
				});
			}
			return result;
		}

		// Token: 0x040000DA RID: 218
		private Dictionary<string, T> _stringToValue = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
	}
}
