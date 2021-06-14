using System;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000009 RID: 9
	public static class NormalizedString
	{
		// Token: 0x0600001B RID: 27 RVA: 0x00002502 File Offset: 0x00000702
		public static string Get(string value)
		{
			return value.ToUpper(GlobalVariables.Culture);
		}
	}
}
