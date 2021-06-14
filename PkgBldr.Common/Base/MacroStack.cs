using System;
using System.Collections.Generic;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000043 RID: 67
	public class MacroStack
	{
		// Token: 0x0600013E RID: 318 RVA: 0x00008698 File Offset: 0x00006898
		public string GetValue(string name)
		{
			if (name == null)
			{
				throw new PkgGenException("null for parameter name of MacroResolver.GetValue");
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

		// Token: 0x0600013F RID: 319 RVA: 0x0000870C File Offset: 0x0000690C
		public bool RemoveName(string name)
		{
			if (name == null)
			{
				throw new PkgGenException("null for parameter name of MacroResolver.RemoveName");
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

		// Token: 0x04000082 RID: 130
		protected Stack<Dictionary<string, Macro>> _dictionaries = new Stack<Dictionary<string, Macro>>();
	}
}
