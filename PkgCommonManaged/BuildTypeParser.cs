using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000030 RID: 48
	public static class BuildTypeParser
	{
		// Token: 0x0600020C RID: 524 RVA: 0x00008D60 File Offset: 0x00006F60
		static BuildTypeParser()
		{
			BuildTypeParser.parser.Add(BuildType.Retail, "Retail");
			BuildTypeParser.parser.Add(BuildType.Retail, "fre");
			BuildTypeParser.parser.Add(BuildType.Checked, "Checked");
			BuildTypeParser.parser.Add(BuildType.Checked, "chk");
			BuildTypeParser.parser.Add(BuildType.Debug, "Debug");
			BuildTypeParser.parser.Add(BuildType.Debug, "dbg");
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00008DD7 File Offset: 0x00006FD7
		public static BuildType Parse(string value)
		{
			return BuildTypeParser.parser.Parse(value);
		}

		// Token: 0x040000DB RID: 219
		private static StringToEnum<BuildType> parser = new StringToEnum<BuildType>();
	}
}
