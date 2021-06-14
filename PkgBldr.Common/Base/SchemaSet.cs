using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.CompPlat.PkgBldr.Base.Tools;
using Microsoft.CompPlat.PkgBldr.Interfaces;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000046 RID: 70
	internal class SchemaSet
	{
		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00008B86 File Offset: 0x00006D86
		// (set) Token: 0x06000157 RID: 343 RVA: 0x00008B8E File Offset: 0x00006D8E
		private List<XmlSchema> m_mergedSchemaList { get; set; }

		// Token: 0x06000158 RID: 344 RVA: 0x00008B97 File Offset: 0x00006D97
		public SchemaSet(IDeploymentLogger logger = null, PkgBldrCmd pkgBldrArgs = null)
		{
			this.m_logger = (logger ?? new Logger());
			this.m_pkgBldrArgs = (pkgBldrArgs ?? new PkgBldrCmd());
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00008BC0 File Offset: 0x00006DC0
		private void getExternalElements(XElement root, XNamespace rootns)
		{
			if (root.Name.Namespace != rootns)
			{
				if (this.SchemaIsExternal(root.Name.Namespace))
				{
					this.m_externalXmlElements.Add(root);
				}
				return;
			}
			foreach (XElement root2 in root.Elements())
			{
				this.getExternalElements(root2, rootns);
			}
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00008C44 File Offset: 0x00006E44
		private void validateStream(Stream xmlStream, XmlSchema schema)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
			{
				ValidationType = ValidationType.Schema
			};
			xmlReaderSettings.ValidationEventHandler += this.ValidationEventHandler;
			xmlReaderSettings.Schemas.Add(schema);
			XmlReader xmlReader = XmlReader.Create(xmlStream, xmlReaderSettings);
			try
			{
				while (xmlReader.Read())
				{
				}
			}
			catch (XmlException ex)
			{
				throw new PkgGenException("Line:{0} Position {1} schema validation error \n {2}", new object[]
				{
					ex.LineNumber.ToString(CultureInfo.InvariantCulture),
					ex.LinePosition.ToString(CultureInfo.InvariantCulture),
					ex.Message
				});
			}
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00008CE8 File Offset: 0x00006EE8
		private XmlSchema GetSchema(XNamespace target)
		{
			foreach (XmlSchema xmlSchema in this.m_mergedSchemaList)
			{
				if (xmlSchema.TargetNamespace == target)
				{
					return xmlSchema;
				}
			}
			throw new PkgGenException("no schema found for {0}", new object[]
			{
				target.ToString()
			});
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00008D68 File Offset: 0x00006F68
		private bool SchemaIsExternal(XNamespace target)
		{
			using (List<XmlSchema>.Enumerator enumerator = this.m_mergedSchemaList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.TargetNamespace == target)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00008DCC File Offset: 0x00006FCC
		private void validateElement(XElement element, XmlSchema schema)
		{
			Stream stream = new MemoryStream();
			element.Save(stream);
			stream.Position = 0L;
			this.validateStream(stream, schema);
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00008DF8 File Offset: 0x00006FF8
		[SuppressMessage("Microsoft.Performance", "CA1811")]
		public void ValidateXmlFile(string xmlFile)
		{
			if (LongPathFile.Exists(xmlFile))
			{
				throw new PkgGenException("xml file not found {0}", new object[]
				{
					xmlFile
				});
			}
			using (FileStream fileStream = LongPathFile.OpenRead(xmlFile))
			{
				this.ValidateStream(fileStream);
			}
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00008E4C File Offset: 0x0000704C
		private void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			XmlSeverityType severity = e.Severity;
			string format;
			if (severity == XmlSeverityType.Error)
			{
				format = string.Format(CultureInfo.InvariantCulture, "Line:{0} Postition:{1} error {2}", new object[]
				{
					e.Exception.LineNumber.ToString(CultureInfo.InvariantCulture),
					e.Exception.LinePosition.ToString(CultureInfo.InvariantCulture),
					e.Message
				});
				this.m_logger.LogError(format, new object[0]);
				throw new PkgGenException("PkgBldrCommon schema validation failed");
			}
			if (severity != XmlSeverityType.Warning)
			{
				return;
			}
			format = string.Format(CultureInfo.InvariantCulture, "Line:{0} Postition:{1} warning {2}", new object[]
			{
				e.Exception.LineNumber.ToString(CultureInfo.InvariantCulture),
				e.Exception.LinePosition.ToString(CultureInfo.InvariantCulture),
				e.Message
			});
			this.m_logger.LogWarning(format, new object[0]);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00008F44 File Offset: 0x00007144
		public void ValidateXmlDoc(XDocument xDoc)
		{
			MemoryStream memoryStream = new MemoryStream();
			xDoc.Save(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			this.ValidateStream(memoryStream);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00008F70 File Offset: 0x00007170
		private void ValidateStream(Stream xmlStream)
		{
			XElement xelement = XElement.Load(xmlStream);
			this.m_externalXmlElements = new List<XElement>();
			XElement xelement2 = xelement;
			this.getExternalElements(xelement2, xelement2.Name.Namespace);
			foreach (XElement xelement3 in this.m_externalXmlElements)
			{
				xelement3.Remove();
			}
			XmlSchema schema = this.GetSchema(xelement.Name.Namespace);
			this.validateElement(xelement, schema);
			foreach (XElement xelement4 in this.m_externalXmlElements)
			{
				schema = this.GetSchema(xelement4.Name.Namespace);
				this.validateElement(xelement4, schema);
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00009054 File Offset: 0x00007254
		public void LoadSchemasFromFiles(List<string> xsdFileList)
		{
			List<XSD> list = new List<XSD>();
			foreach (string file in xsdFileList)
			{
				list.Add(new XSD
				{
					file = file,
					ns = null
				});
			}
			this.MergeSchema(list);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x000090C4 File Offset: 0x000072C4
		public List<XmlSchema> GetMergedSchemas()
		{
			return this.m_mergedSchemaList;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x000090CC File Offset: 0x000072CC
		private void MergeSchema(List<XSD> schemaFileList)
		{
			List<string> list = new List<string>();
			XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
			List<XNamespace> list2 = new List<XNamespace>();
			this.m_mergedSchemaList = new List<XmlSchema>();
			foreach (XSD xsd in schemaFileList)
			{
				if (!list.Contains(xsd.file))
				{
					if (!LongPathFile.Exists(xsd.file))
					{
						throw new PkgGenException("failed to load schema {0}", new object[]
						{
							xsd.file
						});
					}
					try
					{
						Stream stream;
						if (xsd.ns != null)
						{
							XDocument xdocument = PkgBldrHelpers.XDocumentLoadFromLongPath(xsd.file);
							XNamespace xnamespace = xsd.ns;
							foreach (XAttribute xattribute in xdocument.Root.Attributes())
							{
								if (xattribute.Name.LocalName.Equals("targetNamespace"))
								{
									xattribute.Value = xnamespace.NamespaceName;
								}
								else if (xattribute.IsNamespaceDeclaration && !xattribute.Name.LocalName.Equals("xsd") && !xattribute.Name.LocalName.Equals("xs"))
								{
									xattribute.Value = xnamespace.ToString();
								}
							}
							stream = new MemoryStream();
							xdocument.Save(stream);
							stream.Flush();
							stream.Position = 0L;
						}
						else
						{
							stream = new FileStream(xsd.file, FileMode.Open, FileAccess.Read);
						}
						XmlSchema xmlSchema = XmlSchema.Read(XmlReader.Create(stream), null);
						XNamespace item = xmlSchema.TargetNamespace;
						if (!list2.Contains(item))
						{
							list2.Add(item);
						}
						xmlSchemaSet.Add(xmlSchema);
						list.Add(xsd.file);
					}
					catch (XmlSchemaException ex)
					{
						throw new PkgGenException(ex, "schema errors: {0}, {1}", new object[]
						{
							xsd,
							ex.Message
						});
					}
				}
			}
			xmlSchemaSet.Compile();
			XmlSchemaSet xmlSchemaSet2 = new XmlSchemaSet();
			foreach (XNamespace xnamespace2 in list2)
			{
				IEnumerable enumerable = xmlSchemaSet.Schemas(xnamespace2.ToString());
				XmlSchema xmlSchema2 = null;
				XmlSerializerNamespaces xmlSerializerNamespaces = null;
				foreach (object obj in enumerable)
				{
					XmlSchema xmlSchema3 = (XmlSchema)obj;
					if (xmlSchema2 == null)
					{
						xmlSchema2 = xmlSchema3;
						xmlSerializerNamespaces = xmlSchema3.Namespaces;
					}
					else
					{
						foreach (XmlSchemaObject item2 in xmlSchema3.Items)
						{
							xmlSchema2.Items.Add(item2);
						}
						XmlQualifiedName[] array = xmlSchema3.Namespaces.ToArray();
						XmlQualifiedName[] source = xmlSerializerNamespaces.ToArray();
						for (int i = 0; i < array.Count<XmlQualifiedName>(); i++)
						{
							XmlQualifiedName xmlQualifiedName = array[i];
							if (!source.Contains(xmlQualifiedName))
							{
								xmlSerializerNamespaces.Add(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
							}
						}
					}
				}
				xmlSchemaSet2.Add(xmlSchema2);
				this.m_mergedSchemaList.Add(xmlSchema2);
			}
			xmlSchemaSet2.Compile();
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000094C4 File Offset: 0x000076C4
		public void LoadSchemasFromPlugins(IEnumerable<IPkgPlugin> plugins)
		{
			List<XSD> list = new List<XSD>();
			foreach (IPkgPlugin pkgPlugin in plugins)
			{
				string xsdPath = this.GetXsdPath(pkgPlugin.XmlSchemaPath.ToLowerInvariant());
				list.Add(new XSD
				{
					file = xsdPath,
					ns = pkgPlugin.XmlSchemaNameSpace
				});
			}
			this.MergeSchema(list);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00009548 File Offset: 0x00007748
		private string GetXsdPath(string schemaPath)
		{
			string fileName = LongPath.GetFileName(schemaPath);
			string directoryName = LongPath.GetDirectoryName(schemaPath);
			string path;
			if (!(directoryName == "pkgbldr.csi.xsd"))
			{
				if (!(directoryName == "pkgbldr.pkg.xsd"))
				{
					if (!(directoryName == "pkgbldr.shared.xsd"))
					{
						if (!(directoryName == "pkgbldr.wm.xsd"))
						{
							throw new Exception("Unrecognized xsd type " + schemaPath);
						}
						path = Environment.ExpandEnvironmentVariables(this.m_pkgBldrArgs.wmXsdPath);
					}
					else
					{
						path = Environment.ExpandEnvironmentVariables(this.m_pkgBldrArgs.sharedXsdPath);
					}
				}
				else
				{
					path = Environment.ExpandEnvironmentVariables(this.m_pkgBldrArgs.pkgXsdPath);
				}
			}
			else
			{
				path = Environment.ExpandEnvironmentVariables(this.m_pkgBldrArgs.csiXsdPath);
			}
			return LongPath.Combine(path, fileName);
		}

		// Token: 0x0400008A RID: 138
		private List<XElement> m_externalXmlElements;

		// Token: 0x0400008B RID: 139
		private IDeploymentLogger m_logger;

		// Token: 0x0400008C RID: 140
		private PkgBldrCmd m_pkgBldrArgs;
	}
}
