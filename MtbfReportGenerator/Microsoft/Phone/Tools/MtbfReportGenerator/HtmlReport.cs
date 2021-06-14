using System;
using System.IO;
using Microsoft.Phone.Tools.MtbfReportGenerator.Properties;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000012 RID: 18
	public class HtmlReport : IReport
	{
		// Token: 0x06000071 RID: 113 RVA: 0x000032DC File Offset: 0x000014DC
		public void Generate(MtbfReport mtbfReport, string outputFile, string templateFile)
		{
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.DefaultHtmlTemplate);
			XsltHelper.TransformReport(mtbfReport, outputFile, string.IsNullOrEmpty(templateFile) ? text : templateFile);
		}
	}
}
