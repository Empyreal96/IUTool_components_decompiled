using System;
using System.Globalization;

namespace Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy
{
	// Token: 0x02000048 RID: 72
	public static class GlobalVariables
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000179 RID: 377 RVA: 0x00009BE2 File Offset: 0x00007DE2
		public static CultureInfo Culture
		{
			get
			{
				return GlobalVariables.GlobalCulture;
			}
		}

		// Token: 0x04000093 RID: 147
		private static CultureInfo GlobalCulture = new CultureInfo("en-US", false);
	}
}
