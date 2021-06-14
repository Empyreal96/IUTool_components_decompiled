using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000029 RID: 41
	public class XmlValidator
	{
		// Token: 0x0600015B RID: 347 RVA: 0x000080E0 File Offset: 0x000062E0
		private static void OnSchemaValidationEvent(object sender, ValidationEventArgs e)
		{
			Console.WriteLine(e.Message);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x000080ED File Offset: 0x000062ED
		public XmlValidator() : this(new ValidationEventHandler(XmlValidator.OnSchemaValidationEvent))
		{
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00008104 File Offset: 0x00006304
		public XmlValidator(ValidationEventHandler eventHandler)
		{
			this._validationEventHandler = eventHandler;
			this._xmlReaderSettings = new XmlReaderSettings();
			this._xmlReaderSettings.ValidationType = ValidationType.Schema;
			this._xmlReaderSettings.ValidationFlags |= (XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints);
			this._xmlReaderSettings.ValidationEventHandler += this._validationEventHandler;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000815A File Offset: 0x0000635A
		public void AddSchema(XmlSchema schema)
		{
			this._xmlReaderSettings.Schemas.Add(schema);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00008170 File Offset: 0x00006370
		public void AddSchema(string xsdFile)
		{
			using (Stream stream = LongPathFile.OpenRead(xsdFile))
			{
				this.AddSchema(stream);
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x000081A8 File Offset: 0x000063A8
		public void AddSchema(Stream xsdStream)
		{
			using (XmlReader xmlReader = XmlReader.Create(xsdStream))
			{
				this.AddSchema(XmlSchema.Read(xmlReader, this._validationEventHandler));
			}
		}

		// Token: 0x06000161 RID: 353 RVA: 0x000081EC File Offset: 0x000063EC
		public void Validate(string xmlFile)
		{
			using (Stream stream = LongPathFile.OpenRead(xmlFile))
			{
				this.Validate(stream);
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00008224 File Offset: 0x00006424
		public void Validate(Stream xmlStream)
		{
			XmlReader xmlReader = XmlReader.Create(xmlStream, this._xmlReaderSettings);
			while (xmlReader.Read())
			{
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00008248 File Offset: 0x00006448
		public void Validate(XElement element)
		{
			while (element != null && element.GetSchemaInfo() == null)
			{
				element = element.Parent;
			}
			if (element == null)
			{
				throw new ArgumentException("Argument has no SchemaInfo anywhere in the document. Validate the XDocument first.");
			}
			IXmlSchemaInfo schemaInfo = element.GetSchemaInfo();
			element.Validate(schemaInfo.SchemaElement, this._xmlReaderSettings.Schemas, this._validationEventHandler, true);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000829D File Offset: 0x0000649D
		public void Validate(XDocument document)
		{
			document.Validate(this._xmlReaderSettings.Schemas, this._validationEventHandler, true);
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000082B7 File Offset: 0x000064B7
		public XmlReader GetXmlReader(string xmlFile)
		{
			return XmlReader.Create(LongPathFile.OpenRead(xmlFile), this._xmlReaderSettings);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x000082CA File Offset: 0x000064CA
		public XmlReader GetXmlReader(Stream xmlStream)
		{
			return XmlReader.Create(xmlStream, this._xmlReaderSettings);
		}

		// Token: 0x04000080 RID: 128
		protected ValidationEventHandler _validationEventHandler;

		// Token: 0x04000081 RID: 129
		protected XmlReaderSettings _xmlReaderSettings;
	}
}
