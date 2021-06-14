using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PackageGenerator
{
	// Token: 0x02000002 RID: 2
	internal class MergingSchemaValidator : XmlValidator
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public MergingSchemaValidator(ValidationEventHandler eventHandler) : base(eventHandler)
		{
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002059 File Offset: 0x00000259
		public XmlSchema AddSchemaWithPlugins(Stream baseSchemaStream, IEnumerable<IPkgPlugin> plugins)
		{
			return this.AddSchemaWithPlugins(XmlSchema.Read(baseSchemaStream, this._validationEventHandler), plugins);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002070 File Offset: 0x00000270
		public XmlSchema AddSchemaWithPlugins(XmlSchema baseSchema, IEnumerable<IPkgPlugin> plugins)
		{
			List<KeyValuePair<Assembly, string>> list = new List<KeyValuePair<Assembly, string>>();
			XmlSchemaElement schemaComponentsType = this.GetSchemaComponentsType(baseSchema);
			if (schemaComponentsType == null)
			{
				throw new PkgGenException("Cannot load plugins into base schema, does not contain 'Components'.", new object[0]);
			}
			base.AddSchema(baseSchema);
			XmlSchemaChoice xmlSchemaChoice = (XmlSchemaChoice)((XmlSchemaComplexType)schemaComponentsType.SchemaType).Particle;
			foreach (IPkgPlugin pkgPlugin in plugins)
			{
				try
				{
					KeyValuePair<Assembly, string> item = new KeyValuePair<Assembly, string>(pkgPlugin.GetType().Assembly, pkgPlugin.XmlSchemaPath);
					if (!list.Contains(item))
					{
						XmlSchema xmlSchema = XmlSchema.Read(item.Key.GetManifestResourceStream(item.Value), null);
						if (!xmlSchema.TargetNamespace.Equals(baseSchema.TargetNamespace, StringComparison.InvariantCulture))
						{
							throw new PkgGenException("Plugin '{0}' returned a schema that does not target the '{1}' namespace. It must target this namespace.", new object[]
							{
								pkgPlugin.Name,
								baseSchema.TargetNamespace
							});
						}
						foreach (XmlSchemaObject item2 in xmlSchema.Items)
						{
							baseSchema.Items.Add(item2);
						}
						list.Add(item);
					}
					if (!this.SchemaHasElementName(baseSchema, pkgPlugin.XmlElementName))
					{
						throw new PkgGenException("Plugin '{0}' does not seem to contain an element called '{1}' in the schema. Make sure to use <xs:element name=\"{1}\">", new object[]
						{
							pkgPlugin.Name,
							pkgPlugin.XmlElementName
						});
					}
					XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
					xmlSchemaElement.RefName = new XmlQualifiedName(pkgPlugin.XmlElementName, baseSchema.TargetNamespace);
					xmlSchemaChoice.Items.Add(xmlSchemaElement);
					if (pkgPlugin.XmlElementUniqueXPath != null)
					{
						XmlSchemaUnique xmlSchemaUnique = new XmlSchemaUnique();
						xmlSchemaUnique.Name = pkgPlugin.XmlElementName;
						xmlSchemaUnique.Selector = new XmlSchemaXPath();
						xmlSchemaUnique.Selector.XPath = "ps:" + pkgPlugin.XmlElementName;
						XmlSchemaXPath xmlSchemaXPath = new XmlSchemaXPath();
						xmlSchemaXPath.XPath = pkgPlugin.XmlElementUniqueXPath;
						xmlSchemaUnique.Fields.Add(xmlSchemaXPath);
						schemaComponentsType.Constraints.Add(xmlSchemaUnique);
					}
					this._xmlReaderSettings.Schemas.Reprocess(baseSchema);
				}
				catch (PkgGenException)
				{
					throw;
				}
				catch (XmlSchemaException ex)
				{
					throw new PkgGenException(ex, "Plugin '{0}' has schema errors: {1}", new object[]
					{
						pkgPlugin.Name,
						ex.Message
					});
				}
				catch (Exception innerException)
				{
					throw new PkgGenException(innerException, "Plugin '{0}' does not provide a schema snippet, or it could not be loaded.", new object[]
					{
						pkgPlugin.Name
					});
				}
			}
			return baseSchema;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002368 File Offset: 0x00000568
		private bool SchemaHasElementName(XmlSchema schema, string xmlElementName)
		{
			using (IEnumerator<XmlSchemaElement> enumerator = schema.Items.OfType<XmlSchemaElement>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Name.Equals(xmlElementName, StringComparison.InvariantCulture))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000023C8 File Offset: 0x000005C8
		private XmlSchemaElement GetSchemaComponentsType(XmlSchema schema)
		{
			XmlSchemaElement result = null;
			foreach (XmlSchemaElement xmlSchemaElement in schema.Items.OfType<XmlSchemaElement>())
			{
				if (xmlSchemaElement.Name.Equals("Components", StringComparison.InvariantCulture))
				{
					result = xmlSchemaElement;
					break;
				}
			}
			return result;
		}
	}
}
