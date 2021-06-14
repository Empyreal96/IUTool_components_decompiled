using System;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000013 RID: 19
	public interface IReport
	{
		// Token: 0x06000073 RID: 115
		void Generate(MtbfReport mtbfReport, string outputFile, string templateFile);
	}
}
