using System;
using System.Globalization;
using System.Text;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200000A RID: 10
	public static class SidBuilder
	{
		// Token: 0x0600001C RID: 28 RVA: 0x0000250F File Offset: 0x0000070F
		public static string BuildApplicationSidString(string name)
		{
			return SidBuilder.BuildSidString("S-1-15-2", HashCalculator.CalculateSha256Hash(name, true), 7);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002523 File Offset: 0x00000723
		public static string BuildServiceSidString(string name)
		{
			return SidBuilder.BuildSidString("S-1-5-80", HashCalculator.CalculateSha1Hash(name.ToUpper(GlobalVariables.Culture)), 8);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002540 File Offset: 0x00000740
		public static string BuildApplicationCapabilitySidString(string capabilityId)
		{
			return SidBuilder.BuildSidString("S-1-15-3-1024", HashCalculator.CalculateSha256Hash(capabilityId, true), 8);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002554 File Offset: 0x00000754
		public static string BuildSidString(string sidPrefix, string sidHash, int numberOfRids)
		{
			if (string.IsNullOrEmpty(sidPrefix) || string.IsNullOrEmpty(sidHash))
			{
				throw new PolicyCompilerInternalException("Invalid arguments");
			}
			if (sidHash.Length % 8 != 0)
			{
				throw new PolicyCompilerInternalException("sidHash length is not divisible by 8.");
			}
			StringBuilder stringBuilder = new StringBuilder(sidPrefix);
			int num = 0;
			int num2 = 0;
			while (num < sidHash.Length && num2 < numberOfRids)
			{
				stringBuilder.Append('-');
				string empty = string.Empty;
				uint num3 = uint.Parse(sidHash.Substring(num, 8), NumberStyles.HexNumber, GlobalVariables.Culture);
				uint value = (num3 & 255U) << 24 | (num3 & 65280U) << 8 | (num3 & 16711680U) >> 8 | (num3 & 4278190080U) >> 24;
				stringBuilder.Append(value);
				num += 8;
				num2++;
			}
			return stringBuilder.ToString();
		}
	}
}
