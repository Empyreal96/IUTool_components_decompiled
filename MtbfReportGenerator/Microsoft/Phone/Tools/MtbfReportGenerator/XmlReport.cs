using System;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000014 RID: 20
	public class XmlReport : IReport
	{
		// Token: 0x06000074 RID: 116 RVA: 0x00003318 File Offset: 0x00001518
		public void Generate(MtbfReport mtbfReport, string outputFile, string templateFile)
		{
			if (string.IsNullOrEmpty(templateFile))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(MtbfReport));
				using (StreamWriter streamWriter = new StreamWriter(outputFile))
				{
					xmlSerializer.Serialize(streamWriter, mtbfReport);
					return;
				}
			}
			XsltHelper.TransformReport(mtbfReport, outputFile, templateFile);
		}
	}
}
