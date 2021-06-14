using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x0200000B RID: 11
	public interface IMacroResolver
	{
		// Token: 0x06000026 RID: 38
		void BeginLocal();

		// Token: 0x06000027 RID: 39
		void Register(string name, string value);

		// Token: 0x06000028 RID: 40
		void Register(string name, object value, MacroDelegate del);

		// Token: 0x06000029 RID: 41
		void Register(IEnumerable<KeyValuePair<string, Macro>> macros);

		// Token: 0x0600002A RID: 42
		void EndLocal();

		// Token: 0x0600002B RID: 43
		string GetValue(string name);

		// Token: 0x0600002C RID: 44
		string Resolve(string input);

		// Token: 0x0600002D RID: 45
		string Resolve(string input, MacroResolveOptions option);
	}
}
