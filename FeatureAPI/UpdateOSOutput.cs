using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200000A RID: 10
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "UpdateOSOutput", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class UpdateOSOutput
	{
		// Token: 0x06000028 RID: 40 RVA: 0x000036B0 File Offset: 0x000018B0
		public static UpdateOSOutput ValidateOutput(string outputFile, IULogger logger)
		{
			UpdateOSOutput result = new UpdateOSOutput();
			XsdValidator xsdValidator = new XsdValidator();
			string text = string.Empty;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(DevicePaths.UpdateOSOutputSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new XsdValidatorException("FeatureAPI!ValidateOutput: XSD resource was not found: " + DevicePaths.UpdateOSOutputSchema);
			}
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text))
			{
				try
				{
					xsdValidator.ValidateXsd(manifestResourceStream, outputFile, logger);
				}
				catch (XsdValidatorException innerException)
				{
					throw new FeatureAPIException("FeatureAPI!ValidateOutput: Unable to validate Update OS Output XSD.", innerException);
				}
			}
			logger.LogInfo("FeatureAPI: Successfully validated the Update OS Output XML: {0}", new object[]
			{
				outputFile
			});
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(outputFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateOSOutput));
			try
			{
				result = (UpdateOSOutput)xmlSerializer.Deserialize(textReader);
			}
			catch (Exception innerException2)
			{
				throw new FeatureAPIException("FeatureAPI!ValidateOutput: Unable to parse Update OS Output XML file.", innerException2);
			}
			finally
			{
				textReader.Close();
			}
			return result;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000037E8 File Offset: 0x000019E8
		public void WriteToFile(string fileName)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				NewLineOnAttributes = true
			};
			using (XmlWriter xmlWriter = XmlWriter.Create(fileName, settings))
			{
				new XmlSerializer(typeof(UpdateOSOutput)).Serialize(xmlWriter, this);
			}
		}

		// Token: 0x04000031 RID: 49
		public string Description;

		// Token: 0x04000032 RID: 50
		public int OverallResult;

		// Token: 0x04000033 RID: 51
		public string UpdateState;

		// Token: 0x04000034 RID: 52
		[XmlArrayItem(ElementName = "Package", Type = typeof(UpdateOSOutputPackage), IsNullable = false)]
		[XmlArray]
		public List<UpdateOSOutputPackage> Packages;
	}
}
