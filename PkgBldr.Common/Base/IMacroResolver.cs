using System;
using System.Collections.Generic;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000040 RID: 64
	public interface IMacroResolver
	{
		// Token: 0x06000127 RID: 295
		void BeginLocal();

		// Token: 0x06000128 RID: 296
		void Register(string name, string value);

		// Token: 0x06000129 RID: 297
		void Register(string name, object value, MacroDelegate del);

		// Token: 0x0600012A RID: 298
		void Register(IEnumerable<KeyValuePair<string, Macro>> macros, bool allowOverride = false);

		// Token: 0x0600012B RID: 299
		void EndLocal();

		// Token: 0x0600012C RID: 300
		string GetValue(string name);

		// Token: 0x0600012D RID: 301
		string Resolve(string input);

		// Token: 0x0600012E RID: 302
		string Resolve(string input, MacroResolveOptions option);
	}
}
