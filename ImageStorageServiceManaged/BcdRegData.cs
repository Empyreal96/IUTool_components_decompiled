using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200005D RID: 93
	public class BcdRegData
	{
		// Token: 0x0600044B RID: 1099 RVA: 0x0001347D File Offset: 0x0001167D
		public void AddRegKey(string regKey)
		{
			if (!this._regKeys.ContainsKey(regKey))
			{
				this._regKeys.Add(regKey, new List<BcdRegValue>());
			}
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x0001349E File Offset: 0x0001169E
		public Dictionary<string, List<BcdRegValue>> RegKeys()
		{
			return this._regKeys;
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x000134A8 File Offset: 0x000116A8
		public void AddRegValue(string regKey, string name, string value, string type)
		{
			if (!this._regKeys.ContainsKey(regKey))
			{
				this.AddRegKey(regKey);
			}
			BcdRegValue item = new BcdRegValue(name, value, type);
			this._regKeys[regKey].Add(item);
		}

		// Token: 0x04000263 RID: 611
		private Dictionary<string, List<BcdRegValue>> _regKeys = new Dictionary<string, List<BcdRegValue>>(StringComparer.OrdinalIgnoreCase);
	}
}
