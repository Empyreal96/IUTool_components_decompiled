using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000009 RID: 9
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "UpdateOSInput", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class UpdateOSInput
	{
		// Token: 0x06000025 RID: 37 RVA: 0x000034B0 File Offset: 0x000016B0
		public static UpdateOSInput ValidateInput(string inputFile, IULogger logger)
		{
			UpdateOSInput updateOSInput = new UpdateOSInput();
			string text = string.Empty;
			string updateOSInputSchema = DevicePaths.UpdateOSInputSchema;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(updateOSInputSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new XsdValidatorException("ToolsCommon!XsdValidator::ValidateXsd: XSD resource was not found: " + updateOSInputSchema);
			}
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text))
			{
				XsdValidator xsdValidator = new XsdValidator();
				try
				{
					xsdValidator.ValidateXsd(manifestResourceStream, inputFile, logger);
				}
				catch (XsdValidatorException innerException)
				{
					throw new FeatureAPIException("FeatureAPI!ValidateInput: Unable to validate update input XSD.", innerException);
				}
			}
			logger.LogInfo("FeatureAPI: Successfully validated the update input XML: {0}", new object[]
			{
				inputFile
			});
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(inputFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateOSInput));
			try
			{
				updateOSInput = (UpdateOSInput)xmlSerializer.Deserialize(textReader);
				for (int j = 0; j < updateOSInput.PackageFiles.Count; j++)
				{
					updateOSInput.PackageFiles[j] = Environment.ExpandEnvironmentVariables(updateOSInput.PackageFiles[j]);
					updateOSInput.PackageFiles[j] = updateOSInput.PackageFiles[j].Trim();
				}
			}
			catch (Exception innerException2)
			{
				throw new FeatureAPIException("FeatureAPI!ValidateInput: Unable to parse Update OS Input XML file.", innerException2);
			}
			finally
			{
				textReader.Close();
			}
			return updateOSInput;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003640 File Offset: 0x00001840
		public void WriteToFile(string fileName)
		{
			TextWriter textWriter = new StreamWriter(LongPathFile.OpenWrite(fileName));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateOSInput));
			try
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			catch (Exception innerException)
			{
				throw new FeatureAPIException("FeatureAPI!WriteToFile: Unable to write Update OS Input XML file '" + fileName + "'", innerException);
			}
			finally
			{
				textWriter.Close();
			}
		}

		// Token: 0x0400002E RID: 46
		public string Description;

		// Token: 0x0400002F RID: 47
		public string DateTime;

		// Token: 0x04000030 RID: 48
		[XmlArrayItem(ElementName = "PackageFile", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		public List<string> PackageFiles;
	}
}
