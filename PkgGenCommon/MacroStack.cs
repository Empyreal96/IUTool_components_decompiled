using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x02000005 RID: 5
	public class MacroStack
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00002144 File Offset: 0x00000344
		public string GetValue(string name)
		{
			if (name == null)
			{
				throw new PkgGenException("null for parameter name of MacroResolver.GetValue", new object[0]);
			}
			Macro macro = null;
			using (Stack<Dictionary<string, Macro>>.Enumerator enumerator = this._dictionaries.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.TryGetValue(name, out macro))
					{
						break;
					}
				}
			}
			if (macro == null)
			{
				return null;
			}
			return macro.StringValue;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000021BC File Offset: 0x000003BC
		public bool RemoveName(string name)
		{
			if (name == null)
			{
				throw new PkgGenException("null for parameter name of MacroResolver.RemoveName", new object[0]);
			}
			using (Stack<Dictionary<string, Macro>>.Enumerator enumerator = this._dictionaries.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Remove(name))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04000005 RID: 5
		protected Stack<Dictionary<string, Macro>> _dictionaries = new Stack<Dictionary<string, Macro>>();
	}
}
