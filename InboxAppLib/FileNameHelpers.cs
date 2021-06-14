using System;
using System.IO;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000030 RID: 48
	public static class FileNameHelpers
	{
		// Token: 0x060000AC RID: 172 RVA: 0x000046C8 File Offset: 0x000028C8
		public static string RemoveMpapPrefix(this string str)
		{
			int length = "MPAP_".Length;
			string result;
			if (str.StartsWith("MPAP_"))
			{
				result = str.Substring(length);
			}
			else
			{
				result = str;
			}
			return result;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000046FC File Offset: 0x000028FC
		public static string RemoveSrcExtension(this string str)
		{
			string result;
			if (".src".Equals(Path.GetExtension(str), StringComparison.OrdinalIgnoreCase))
			{
				result = Path.GetFileNameWithoutExtension(str);
			}
			else
			{
				result = str;
			}
			return result;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00004728 File Offset: 0x00002928
		public static string CleanFileNameForUpdate(this string str, bool isEarly)
		{
			str = (isEarly ? "early" : "") + "_" + str.CleanFileName();
			return str;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000474C File Offset: 0x0000294C
		public static string CleanFileName(this string str)
		{
			str = str.RemoveMpapPrefix().RemoveSrcExtension().Replace(".provxml", "_Infused.provxml");
			return str;
		}
	}
}
