using System;
using System.Text.RegularExpressions;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x02000010 RID: 16
	internal static class SddlHelpers
	{
		// Token: 0x0600003C RID: 60 RVA: 0x00003AD4 File Offset: 0x00001CD4
		public static string GetSddlOwner(string SDDL)
		{
			string text = null;
			Match match = Regex.Match(SDDL + ":", "[O]:([A-Za-z0-9\\-]+)[:]");
			if (match.Success)
			{
				text = match.Value;
				text = text.Substring(2, text.Length - 2 - 2);
			}
			return text;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003B1C File Offset: 0x00001D1C
		public static string GetSddlGroup(string SDDL)
		{
			string text = null;
			Match match = Regex.Match(SDDL + ":", "[G]:([A-Za-z0-9\\-]+)[:]");
			if (match.Success)
			{
				text = match.Value;
				text = text.Substring(2, text.Length - 2 - 2);
			}
			return text;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003B64 File Offset: 0x00001D64
		public static string GetSddlDacl(string SDDL)
		{
			string text = null;
			Match match = Regex.Match(SDDL + ":", "[D]:([A-Za-z0-9\\(\\);\\-]+)[:]");
			if (match.Success)
			{
				text = match.Value;
				text = text.Substring(2, text.Length - 2 - 2);
			}
			return text;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003BAC File Offset: 0x00001DAC
		public static string GetSddlSacl(string SDDL)
		{
			string text = null;
			Match match = Regex.Match(SDDL + ":", "[S]:([A-Za-z0-9\\(\\);\\-]+)[:]");
			if (match.Success)
			{
				text = match.Value;
				text = text.Substring(2, text.Length - 2 - 1);
			}
			return text;
		}
	}
}
