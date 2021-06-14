using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200001F RID: 31
	internal static class SddlNormalizer
	{
		// Token: 0x06000136 RID: 310 RVA: 0x000061FC File Offset: 0x000043FC
		private static string ToFullSddl(string sid)
		{
			if (string.IsNullOrEmpty(sid) || sid.StartsWith("S-", StringComparison.OrdinalIgnoreCase) || SddlNormalizer.s_knownSids.Contains(sid))
			{
				return sid;
			}
			string text = null;
			if (!SddlNormalizer.s_map.TryGetValue(sid, out text))
			{
				text = new SecurityIdentifier(sid).ToString();
				SddlNormalizer.s_map.Add(sid, text);
			}
			return text;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00006258 File Offset: 0x00004458
		public static string FixAceSddl(string sddl)
		{
			if (string.IsNullOrEmpty(sddl))
			{
				return sddl;
			}
			return Regex.Replace(sddl, ";(?<sid>[^;]*?)\\)", (Match x) => string.Format(CultureInfo.InvariantCulture, ";{0})", new object[]
			{
				SddlNormalizer.ToFullSddl(x.Groups["sid"].Value)
			}));
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0000628E File Offset: 0x0000448E
		public static string FixOwnerSddl(string sddl)
		{
			if (string.IsNullOrEmpty(sddl))
			{
				return sddl;
			}
			return Regex.Replace(sddl, "O:(?<oid>.*?)G:(?<gid>.*?)", (Match x) => string.Format(CultureInfo.InvariantCulture, "O:{0}G:{1}", new object[]
			{
				SddlNormalizer.ToFullSddl(x.Groups["oid"].Value),
				SddlNormalizer.ToFullSddl(x.Groups["gid"].Value)
			}));
		}

		// Token: 0x04000059 RID: 89
		private static readonly HashSet<string> s_knownSids = new HashSet<string>
		{
			"AN",
			"AO",
			"AU",
			"BA",
			"BG",
			"BO",
			"BU",
			"CA",
			"CD",
			"CG",
			"CO",
			"CY",
			"DA",
			"DC",
			"DD",
			"DG",
			"DU",
			"EA",
			"ED",
			"ER",
			"IS",
			"IU",
			"LA",
			"LG",
			"LS",
			"LU",
			"MU",
			"NO",
			"NS",
			"NU",
			"OW",
			"PA",
			"PO",
			"PS",
			"PU",
			"RC",
			"RD",
			"RE",
			"RO",
			"RS",
			"RU",
			"SA",
			"SO",
			"SU",
			"SY",
			"WD",
			"WR"
		};

		// Token: 0x0400005A RID: 90
		private static Dictionary<string, string> s_map = new Dictionary<string, string>();
	}
}
