using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000009 RID: 9
	public class XmlValidator
	{
		// Token: 0x0600005C RID: 92 RVA: 0x00004834 File Offset: 0x00002A34
		public void ValidateXmlAndAddDefaults(string xsdFile, string xmlFile, IULogger logger)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
			string text = string.Empty;
			if (!File.Exists(xmlFile))
			{
				throw new XmlValidatorException("XmlValidator::ValidateXml: XML file was not found: " + xmlFile);
			}
			this._logger = logger;
			foreach (string text2 in manifestResourceNames)
			{
				if (text2.Contains(xsdFile))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new XmlValidatorException("XmlValidator::ValidateXml: XSD resource was not found: " + xsdFile);
			}
			this._fileIsValid = true;
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text))
			{
				if (manifestResourceStream == null)
				{
					throw new XmlValidatorException("XmlValidator::ValidateXml: Failed to load the embeded schema file: " + text);
				}
				try
				{
					XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
					XmlSchema schema = XmlSchema.Read(manifestResourceStream, new ValidationEventHandler(this.ValidationHandler));
					xmlSchemaSet.Add(schema);
					XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
					xmlReaderSettings.ValidationType = ValidationType.Schema;
					xmlReaderSettings.Schemas = xmlSchemaSet;
					xmlReaderSettings.ValidationEventHandler += this.ValidationHandler;
					using (XmlReader xmlReader = XmlReader.Create(xmlFile, xmlReaderSettings))
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.Load(xmlReader);
						xmlDocument.Schemas = xmlSchemaSet;
						xmlDocument.Validate(new ValidationEventHandler(this.ValidationHandler));
					}
				}
				catch (XmlSchemaException innerException)
				{
					throw new XmlValidatorException("XmlValidator::ValidateXml: Unable to use the schema provided " + text, innerException);
				}
				catch (Exception innerException2)
				{
					throw new XmlValidatorException("XmlValidator::ValidateXml: There was a problem validating the XML file " + xmlFile, innerException2);
				}
			}
			if (!this._fileIsValid)
			{
				throw new XmlValidatorException(string.Format(CultureInfo.InvariantCulture, "XmlValidator::ValidateXml: Validation of {0} failed", new object[]
				{
					xmlFile
				}));
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000049F8 File Offset: 0x00002BF8
		private void ValidationHandler(object sender, ValidationEventArgs args)
		{
			string format = string.Format(CultureInfo.InvariantCulture, "\nXmlValidator::ValidateXml: XML Validation {0}: {1}", new object[]
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

		// Token: 0x04000045 RID: 69
		private bool _fileIsValid = true;

		// Token: 0x04000046 RID: 70
		private IULogger _logger;
	}
}
