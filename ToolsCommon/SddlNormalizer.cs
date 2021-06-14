using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200003E RID: 62
	internal static class SddlNormalizer
	{
		// Token: 0x06000187 RID: 391 RVA: 0x000087E4 File Offset: 0x000069E4
		private static string ToFullSddl(string sid)
		{
			if (string.IsNullOrEmpty(sid) || sid.StartsWith("S-", StringComparison.Ordinal) || SddlNormalizer.s_knownSids.Contains(sid))
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

		// Token: 0x06000188 RID: 392 RVA: 0x00008840 File Offset: 0x00006A40
		private static string FormatFullAccountSid(string matchGroupIndex, Match match)
		{
			string value = match.Value;
			string value2 = match.Groups[matchGroupIndex].Value;
			char c = value[value.Length - 1];
			return value.Remove(value.Length - (value2.Length + 1)) + SddlNormalizer.ToFullSddl(value2) + c.ToString();
		}

		// Token: 0x06000189 RID: 393 RVA: 0x000088A0 File Offset: 0x00006AA0
		public static string FixAceSddl(string sddl)
		{
			if (string.IsNullOrEmpty(sddl))
			{
				return sddl;
			}
			return Regex.Replace(sddl, "((;[^;]*){4};)(?<sid>[^;\\)]+)([;\\)])", (Match x) => SddlNormalizer.FormatFullAccountSid("sid", x));
		}

		// Token: 0x0600018A RID: 394 RVA: 0x000088D6 File Offset: 0x00006AD6
		public static string FixOwnerSddl(string sddl)
		{
			if (string.IsNullOrEmpty(sddl))
			{
				return sddl;
			}
			return Regex.Replace(sddl, "O:(?<oid>.*?)G:(?<gid>.*)", (Match x) => string.Format("O:{0}G:{1}", SddlNormalizer.ToFullSddl(x.Groups["oid"].Value), SddlNormalizer.ToFullSddl(x.Groups["gid"].Value)));
		}

		// Token: 0x040000CA RID: 202
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

		// Token: 0x040000CB RID: 203
		private static Dictionary<string, string> s_map = new Dictionary<string, string>();
	}
}
