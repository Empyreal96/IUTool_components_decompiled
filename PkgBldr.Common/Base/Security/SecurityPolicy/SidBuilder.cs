using System;
using System.Globalization;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy
{
	// Token: 0x02000055 RID: 85
	public static class SidBuilder
	{
		// Token: 0x060001A9 RID: 425 RVA: 0x0000A801 File Offset: 0x00008A01
		public static string BuildApplicationSidString(string Name)
		{
			return SidBuilder.BuildSidString("S-1-15-2", HashCalculator.CalculateSha256Hash(Name.ToLower(GlobalVariables.Culture)), 7);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000A81E File Offset: 0x00008A1E
		public static string BuildLegacyApplicationSidString(string Name)
		{
			return SidBuilder.BuildSidString("S-1-15-2", HashCalculator.CalculateSha256Hash(Name.ToUpper(GlobalVariables.Culture)), 7);
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000A83B File Offset: 0x00008A3B
		public static string BuildTaskSidString(string Name)
		{
			return SidBuilder.BuildApplicationSidString(Name);
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000A843 File Offset: 0x00008A43
		public static string BuildServiceSidString(string Name)
		{
			return SidBuilder.BuildSidString("S-1-5-80", HashCalculator.CalculateSha1Hash(Name.ToUpper(GlobalVariables.Culture)), 5);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000A860 File Offset: 0x00008A60
		public static string BuildApplicationCapabilitySidString(string CapabilityId)
		{
			string str;
			if (ConstantStrings.LegacyApplicationCapabilityRids.TryGetValue(CapabilityId, out str))
			{
				return "S-1-15-3-" + str;
			}
			return SidBuilder.BuildSidString("S-1-15-3-1024", HashCalculator.CalculateSha256Hash(CapabilityId.ToUpper(GlobalVariables.Culture)), 8);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000A8A3 File Offset: 0x00008AA3
		public static string BuildServiceCapabilitySidString(string CapabilityId)
		{
			return SidBuilder.BuildSidString("S-1-5-32", HashCalculator.CalculateSha256Hash(CapabilityId.ToUpper(GlobalVariables.Culture)), 8);
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000A8C0 File Offset: 0x00008AC0
		public static string BuildSidString(string SidPrefix, string HashString, int RidCount)
		{
			StringBuilder stringBuilder = new StringBuilder(SidPrefix);
			if (HashString.Length < RidCount * 8)
			{
				throw new PkgGenException("Insufficient hash bytes to generate SID");
			}
			int num = 0;
			int num2 = 0;
			while (num < HashString.Length && num2 < RidCount)
			{
				stringBuilder.Append('-');
				uint num3 = uint.Parse(HashString.Substring(num, 8), NumberStyles.HexNumber, GlobalVariables.Culture);
				uint value = (num3 & 255U) << 24 | (num3 & 65280U) << 8 | (num3 & 16711680U) >> 8 | (num3 & 4278190080U) >> 24;
				stringBuilder.Append(value);
				num += 8;
				num2++;
			}
			return stringBuilder.ToString();
		}
	}
}
