using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000015 RID: 21
	public static class XsltHelper
	{
		// Token: 0x06000076 RID: 118 RVA: 0x00003370 File Offset: 0x00001570
		public static void TransformReport(MtbfReport mtbfReport, string outputFile, string templateFile)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new XmlSerializer(typeof(MtbfReport)).Serialize(memoryStream, mtbfReport);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (XmlReader xmlReader = XmlReader.Create(memoryStream))
				{
					using (StreamWriter streamWriter = new StreamWriter(outputFile))
					{
						XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
						xslCompiledTransform.Load(templateFile, XsltSettings.TrustedXslt, new XmlUrlResolver());
						xslCompiledTransform.Transform(xmlReader, null, streamWriter);
					}
				}
			}
		}
	}
}
