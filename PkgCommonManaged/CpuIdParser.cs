using System;
using System.Globalization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000031 RID: 49
	public static class CpuIdParser
	{
		// Token: 0x0600020E RID: 526 RVA: 0x00008DE4 File Offset: 0x00006FE4
		static CpuIdParser()
		{
			CpuIdParser.parser.Add(CpuId.X86, "X86");
			CpuIdParser.parser.Add(CpuId.ARM, "ARM");
			CpuIdParser.parser.Add(CpuId.ARM64, "ARM64");
			CpuIdParser.parser.Add(CpuId.AMD64, "AMD64");
			CpuIdParser.parser.Add(CpuId.AMD64_X86, "WOW64");
			CpuIdParser.parser.Add(CpuId.AMD64_X86, "AMD64.X86");
			CpuIdParser.parser.Add(CpuId.ARM64_ARM, "ARM64.ARM");
			CpuIdParser.parser.Add(CpuId.ARM64_X86, "ARM64.X86");
			CpuIdParser.parser.Add(CpuId.AMD64_X86, "AMD64_X86");
			CpuIdParser.parser.Add(CpuId.ARM64_ARM, "ARM64_ARM");
			CpuIdParser.parser.Add(CpuId.ARM64_X86, "ARM64_X86");
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00008EAB File Offset: 0x000070AB
		public static CpuId Parse(string value)
		{
			return CpuIdParser.parser.Parse(value.ToUpper(CultureInfo.InvariantCulture));
		}

		// Token: 0x040000DC RID: 220
		private static StringToEnum<CpuId> parser = new StringToEnum<CpuId>();
	}
}
