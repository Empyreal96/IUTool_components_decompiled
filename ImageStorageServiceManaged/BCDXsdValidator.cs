using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000072 RID: 114
	public class BCDXsdValidator
	{
		// Token: 0x060004C8 RID: 1224 RVA: 0x00014B88 File Offset: 0x00012D88
		[CLSCompliant(false)]
		public void ValidateXsd(Stream bcdSchemaStream, string xmlFile, IULogger logger)
		{
			if (!File.Exists(xmlFile))
			{
				throw new BCDXsdValidatorException("ImageServices!BCDXsdValidator::ValidateXsd: XML file was not found: " + xmlFile);
			}
			this._logger = logger;
			this._fileIsValid = true;
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ValidationEventHandler += this.ValidationHandler;
			xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(bcdSchemaStream))
				{
					XmlSchema schema = XmlSchema.Read(xmlReader, new ValidationEventHandler(this.ValidationHandler));
					xmlReaderSettings.Schemas.Add(schema);
				}
			}
			catch (XmlSchemaException innerException)
			{
				throw new BCDXsdValidatorException("ImageServices!BCDXsdValidator::ValidateXsd: Unable to use the schema provided", innerException);
			}
			XmlTextReader xmlTextReader = null;
			XmlReader xmlReader2 = null;
			try
			{
				try
				{
					xmlTextReader = new XmlTextReader(xmlFile);
				}
				catch (Exception innerException2)
				{
					throw new BCDXsdValidatorException("ImageServices!BCDXsdValidator::ValidateXsd: Unable to access the given XML file", innerException2);
				}
				try
				{
					xmlReader2 = XmlReader.Create(xmlTextReader, xmlReaderSettings);
					while (xmlReader2.Read())
					{
					}
				}
				catch (XmlException innerException3)
				{
					throw new BCDXsdValidatorException("ImageServices!BCDXsdValidator::ValidateXsd: There was a problem validating the XML file", innerException3);
				}
			}
			finally
			{
				if (xmlReader2 != null)
				{
					xmlReader2.Close();
				}
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
			if (!this._fileIsValid)
			{
				throw new BCDXsdValidatorException(string.Format(CultureInfo.InvariantCulture, "ImageServices!BCDXsdValidator::ValidateXsd: Validation of {0} failed", new object[]
				{
					xmlFile
				}));
			}
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x00014CE8 File Offset: 0x00012EE8
		private void ValidationHandler(object sender, ValidationEventArgs args)
		{
			string format = string.Format(CultureInfo.InvariantCulture, "\nImageServices!BCDXsdValidator::ValidateXsd: XML Validation {0}: {1}", new object[]
			{
				args.Severity,
				args.Message
			});
			if (args.Severity == XmlSeverityType.Error)
			{
				if (this._logger != null)
				{
					this._logger.LogError(format, new object[0]);
				}
				this._fileIsValid = false;
				return;
			}
			if (this._logger != null)
			{
				this._logger.LogWarning(format, new object[0]);
			}
		}

		// Token: 0x0400029C RID: 668
		private bool _fileIsValid = true;

		// Token: 0x0400029D RID: 669
		private IULogger _logger;
	}
}
