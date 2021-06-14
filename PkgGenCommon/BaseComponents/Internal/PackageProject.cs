using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000073 RID: 115
	[XmlRoot(Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00", ElementName = "Package")]
	public class PackageProject
	{
		// Token: 0x1700009A RID: 154
		// (get) Token: 0x0600025A RID: 602 RVA: 0x000092BF File Offset: 0x000074BF
		// (set) Token: 0x0600025B RID: 603 RVA: 0x000092C7 File Offset: 0x000074C7
		[XmlAttribute("Owner")]
		public string Owner { get; set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x0600025C RID: 604 RVA: 0x000092D0 File Offset: 0x000074D0
		// (set) Token: 0x0600025D RID: 605 RVA: 0x000092D8 File Offset: 0x000074D8
		[XmlAttribute("Component")]
		public string Component { get; set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x0600025E RID: 606 RVA: 0x000092E1 File Offset: 0x000074E1
		// (set) Token: 0x0600025F RID: 607 RVA: 0x000092E9 File Offset: 0x000074E9
		[XmlAttribute("SubComponent")]
		public string SubComponent { get; set; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000260 RID: 608 RVA: 0x000092F2 File Offset: 0x000074F2
		[XmlIgnore]
		public string Name
		{
			get
			{
				return PackageTools.BuildPackageName(this.Owner, this.Component, this.SubComponent);
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000261 RID: 609 RVA: 0x0000930B File Offset: 0x0000750B
		// (set) Token: 0x06000262 RID: 610 RVA: 0x00009313 File Offset: 0x00007513
		[XmlAttribute("OwnerType")]
		public OwnerType OwnerType { get; set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000263 RID: 611 RVA: 0x0000931C File Offset: 0x0000751C
		// (set) Token: 0x06000264 RID: 612 RVA: 0x00009324 File Offset: 0x00007524
		[XmlAttribute("ReleaseType")]
		public ReleaseType ReleaseType { get; set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000265 RID: 613 RVA: 0x0000932D File Offset: 0x0000752D
		// (set) Token: 0x06000266 RID: 614 RVA: 0x00009335 File Offset: 0x00007535
		[XmlAttribute("Platform")]
		public string Platform { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000267 RID: 615 RVA: 0x0000933E File Offset: 0x0000753E
		// (set) Token: 0x06000268 RID: 616 RVA: 0x00009346 File Offset: 0x00007546
		[XmlAttribute("Partition")]
		public string Partition { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000269 RID: 617 RVA: 0x0000934F File Offset: 0x0000754F
		// (set) Token: 0x0600026A RID: 618 RVA: 0x00009357 File Offset: 0x00007557
		[XmlAttribute("GroupingKey")]
		public string GroupingKey { get; set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x0600026B RID: 619 RVA: 0x00009360 File Offset: 0x00007560
		// (set) Token: 0x0600026C RID: 620 RVA: 0x00009368 File Offset: 0x00007568
		[XmlAttribute("Description")]
		public string Description { get; set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600026D RID: 621 RVA: 0x00009371 File Offset: 0x00007571
		// (set) Token: 0x0600026E RID: 622 RVA: 0x00009379 File Offset: 0x00007579
		[XmlAttribute("BinaryPartition")]
		public bool IsBinaryPartition { get; set; }

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600026F RID: 623 RVA: 0x00009382 File Offset: 0x00007582
		// (set) Token: 0x06000270 RID: 624 RVA: 0x0000938A File Offset: 0x0000758A
		[XmlElement("Macros")]
		public MacroTable MacroTable { get; set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000271 RID: 625 RVA: 0x00009393 File Offset: 0x00007593
		[XmlArray("CustomMetadata")]
		[XmlArrayItem(typeof(PackageProject.MetadataField), ElementName = "Field")]
		public List<PackageProject.MetadataField> CustomMetadata { get; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000272 RID: 626 RVA: 0x0000939B File Offset: 0x0000759B
		// (set) Token: 0x06000273 RID: 627 RVA: 0x000093A3 File Offset: 0x000075A3
		[XmlIgnore]
		public string ProjectFilePath { get; protected set; }

		// Token: 0x06000274 RID: 628 RVA: 0x000093AC File Offset: 0x000075AC
		public PackageProject()
		{
			this.IsBinaryPartition = false;
			this.Partition = PkgConstants.c_strMainOsPartition;
			this.ReleaseType = ReleaseType.Production;
			this.OwnerType = OwnerType.Microsoft;
			this.Components = new List<PkgObject>();
			this.CustomMetadata = new List<PackageProject.MetadataField>();
		}

		// Token: 0x06000275 RID: 629 RVA: 0x000093EC File Offset: 0x000075EC
		private void Preprocess(IMacroResolver macroResolver)
		{
			this.Owner = macroResolver.Resolve(this.Owner);
			this.Component = macroResolver.Resolve(this.Component);
			this.SubComponent = macroResolver.Resolve(this.SubComponent);
			this.Platform = macroResolver.Resolve(this.Platform);
			this.Partition = macroResolver.Resolve(this.Partition);
			this.GroupingKey = macroResolver.Resolve(this.GroupingKey);
			this.Description = macroResolver.Resolve(this.Description);
			if (string.IsNullOrEmpty(this.Partition))
			{
				this.Partition = PkgConstants.c_strMainOsPartition;
			}
			if (this.MacroTable != null)
			{
				macroResolver.Register(this.MacroTable.Values);
			}
			this.Components.ForEach(delegate(PkgObject x)
			{
				x.Preprocess(this, macroResolver);
			});
		}

		// Token: 0x06000276 RID: 630 RVA: 0x000094FC File Offset: 0x000076FC
		private void Validate()
		{
			if (this.IsBinaryPartition)
			{
				if (this.Components == null || this.Components.Count != 1 || !(this.Components[0] is BinaryPartitionPkgObject))
				{
					throw new PkgGenProjectException(this.ProjectFilePath, "BinaryPartition package requires exactly one BinaryPartition object", new object[0]);
				}
				if (this.Partition.Equals(PkgConstants.c_strMainOsPartition))
				{
					throw new PkgGenProjectException(this.ProjectFilePath, "A package in MainOS partition can't be binary partition", new object[0]);
				}
			}
			else if (this.Components != null && this.Components.OfType<BinaryPartitionPkgObject>().Count<BinaryPartitionPkgObject>() != 0)
			{
				throw new PkgGenProjectException(this.ProjectFilePath, "BinaryPartition object can only be included in a package with BinaryPartition attribute set", new object[0]);
			}
			if (this.OwnerType != OwnerType.Microsoft && this.Platform == null)
			{
				throw new PkgGenProjectException(this.ProjectFilePath, "Platform needs to be specified for all non-Microsoft packages", new object[0]);
			}
		}

		// Token: 0x06000277 RID: 631 RVA: 0x000095D3 File Offset: 0x000077D3
		private static void OnUnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			LogUtil.Diagnostic("Unknown attribute {0} at line {1}", new object[]
			{
				e.Attr.Name,
				e.LineNumber
			});
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00009601 File Offset: 0x00007801
		private static void OnUnknownElement(object sender, XmlElementEventArgs e)
		{
			LogUtil.Diagnostic("Unknown element {0} at line {1}", new object[]
			{
				e.Element.Name,
				e.LineNumber
			});
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000962F File Offset: 0x0000782F
		private static void OnUnknownNode(object sender, XmlNodeEventArgs e)
		{
			LogUtil.Diagnostic("Unknown node {0} at line {1}", new object[]
			{
				e.Name,
				e.LineNumber
			});
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00009658 File Offset: 0x00007858
		private static void OnUnreferencedObject(object sender, UnreferencedObjectEventArgs e)
		{
			LogUtil.Diagnostic("Unreferenced object {0}", new object[]
			{
				e.UnreferencedId
			});
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00009674 File Offset: 0x00007874
		public void Build(IPackageGenerator pkgGen)
		{
			try
			{
				this.Components.ForEach(delegate(PkgObject x)
				{
					x.Build(pkgGen);
				});
				this.BuildCustomMetadata(pkgGen);
			}
			catch (Exception ex)
			{
				throw new PkgGenProjectException(ex, this.ProjectFilePath, ex.Message, new object[0]);
			}
		}

		// Token: 0x0600027C RID: 636 RVA: 0x000096E0 File Offset: 0x000078E0
		public void AddToCapabilities(XElement element)
		{
			try
			{
				this.SecurityCapabilities.Add(element);
				this.Validator.Validate(element);
			}
			catch (Exception ex)
			{
				if (element.Parent != null)
				{
					element.Remove();
				}
				throw ex;
			}
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00009728 File Offset: 0x00007928
		public void AddToAuthorization(XElement element)
		{
			try
			{
				this.SecurityAuthorization.Add(element);
				this.Validator.Validate(element);
			}
			catch (Exception ex)
			{
				if (element.Parent != null)
				{
					element.Remove();
				}
				throw ex;
			}
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00009770 File Offset: 0x00007970
		public XmlDocument ToXmlDocument()
		{
			XmlDocument xmlDocument = new XmlDocument();
			using (XmlReader xmlReader = this.Document.CreateReader())
			{
				xmlDocument.Load(xmlReader);
			}
			return xmlDocument;
		}

		// Token: 0x0600027F RID: 639 RVA: 0x000097B4 File Offset: 0x000079B4
		private void BuildCustomMetadata(IPackageGenerator pkgGen)
		{
			if (this.CustomMetadata == null || this.CustomMetadata.Count <= 0)
			{
				return;
			}
			string path = PackageTools.BuildPackageName(this.Owner, this.Component, this.SubComponent) + PkgConstants.c_strCustomMetadataExtension;
			string text = Path.Combine(pkgGen.TempDirectory, path);
			string devicePath = Path.Combine(PkgConstants.c_strCustomMetadataDeviceFolder, path);
			XNamespace ns = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00";
			XElement xelement = new XElement(ns + "CustomMetadata");
			foreach (PackageProject.MetadataField metadataField in this.CustomMetadata)
			{
				string value = pkgGen.MacroResolver.Resolve(metadataField.Key);
				string text2 = pkgGen.MacroResolver.Resolve(metadataField.Value);
				XElement content = new XElement(ns + "Field", new object[]
				{
					new XAttribute("Name", value),
					text2
				});
				xelement.Add(content);
			}
			new XDocument(new object[]
			{
				xelement
			}).Save(text);
			pkgGen.AddFile(text, devicePath, PkgConstants.c_defaultAttributes);
		}

		// Token: 0x06000280 RID: 640 RVA: 0x000098F8 File Offset: 0x00007AF8
		public static PackageProject Load(string projPath, Dictionary<string, IPkgPlugin> plugins, IPackageGenerator packageGenerator)
		{
			PackageProject packageProject = null;
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(projPath))
				{
					packageProject = PackageProject.Load(xmlReader, plugins, packageGenerator);
					packageProject.ProjectFilePath = projPath;
				}
			}
			catch (PkgXmlException ex)
			{
				IXmlLineInfo xmlElement = ex.XmlElement;
				if (xmlElement.HasLineInfo())
				{
					throw new PkgGenProjectException(projPath, xmlElement.LineNumber, xmlElement.LinePosition, ex.MessageTrace, new object[0]);
				}
				throw new PkgGenProjectException(projPath, ex.MessageTrace, new object[0]);
			}
			catch (XmlSchemaValidationException ex2)
			{
				throw new PkgGenProjectException(ex2, projPath, ex2.LineNumber, ex2.LinePosition, ex2.Message, new object[0]);
			}
			catch (XmlException innerException)
			{
				throw new PkgGenProjectException(innerException, projPath, "Failed to load XML file.", new object[0]);
			}
			packageProject.Preprocess(packageGenerator.MacroResolver);
			packageProject.Validate();
			return packageProject;
		}

		// Token: 0x06000281 RID: 641 RVA: 0x000099F0 File Offset: 0x00007BF0
		private static PackageProject Load(XmlReader projXmlReader, Dictionary<string, IPkgPlugin> plugins, IPackageGenerator packageGenerator)
		{
			packageGenerator.MacroResolver.BeginLocal();
			PackageProject result;
			try
			{
				XDocument xdocument = XDocument.Load(projXmlReader, LoadOptions.SetLineInfo);
				packageGenerator.XmlValidator.Validate(xdocument);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(PackageProject));
				PackageProject packageProject = null;
				using (XmlReader xmlReader = xdocument.CreateReader())
				{
					packageProject = (PackageProject)xmlSerializer.Deserialize(xmlReader);
					packageProject.Validator = packageGenerator.XmlValidator;
					packageProject.Document = xdocument;
					packageProject.SecurityCapabilities = (xdocument.Root.LocalElement("Capabilities") ?? new XElement("Capabilities"));
					packageProject.SecurityAuthorization = (xdocument.Root.LocalElement("Authorization") ?? new XElement("Authorization"));
				}
				if (packageProject.MacroTable != null)
				{
					packageGenerator.MacroResolver.Register(packageProject.MacroTable.Values);
				}
				XElement xelement = xdocument.Root.LocalElement("Components");
				if (xelement != null)
				{
					XElement xelement2 = new XElement(xelement);
					xelement2.RemoveNodes();
					xelement.ReplaceWith(xelement2);
					packageProject.Validator.Validate(xelement2);
					IEnumerable<IGrouping<string, XElement>> enumerable = from element in xelement.Elements()
					group element by element.Name.LocalName;
					PackageWrapper packageGenerator2 = new PackageWrapper(packageGenerator, packageProject);
					foreach (IGrouping<string, XElement> grouping in enumerable)
					{
						IPkgPlugin pkgPlugin = null;
						if (!plugins.TryGetValue(grouping.Key, out pkgPlugin))
						{
							throw new PkgXmlException(grouping.First<XElement>(), "Unknown component '{0}' used.", new object[]
							{
								grouping.Key
							});
						}
						pkgPlugin.ValidateEntries(packageGenerator2, new List<XElement>(grouping));
					}
					foreach (IGrouping<string, XElement> grouping2 in enumerable)
					{
						IPkgPlugin pkgPlugin2 = plugins[grouping2.Key];
						IEnumerable<PkgObject> enumerable2 = pkgPlugin2.ProcessEntries(packageGenerator2, grouping2);
						if (enumerable2 != null)
						{
							packageProject.Components.AddRange(enumerable2);
						}
						if (pkgPlugin2.UseSecurityCompilerPassthrough)
						{
							xelement2.Add(grouping2);
						}
						else if (enumerable2 != null)
						{
							foreach (PkgObject pkgObject in enumerable2)
							{
								XElement xelement3 = pkgObject.ToXElement<PkgObject>();
								try
								{
									xelement2.Add(xelement3);
									packageProject.Validator.Validate(xelement3);
								}
								catch (XmlSchemaValidationException ex)
								{
									throw new PkgGenProjectException(ex, packageProject.ProjectFilePath, "Plugin '{0}' returned a component with invalid resulting XML: {1} \n{2}", new object[]
									{
										pkgPlugin2.Name,
										ex.Message,
										xelement3.ToString()
									});
								}
							}
						}
					}
				}
				result = packageProject;
			}
			catch (InvalidOperationException ex2)
			{
				if (ex2.InnerException != null)
				{
					throw ex2.InnerException;
				}
				throw;
			}
			finally
			{
				packageGenerator.MacroResolver.EndLocal();
			}
			return result;
		}

		// Token: 0x040001A8 RID: 424
		[XmlIgnore]
		private XmlValidator Validator;

		// Token: 0x040001A9 RID: 425
		[XmlIgnore]
		private XDocument Document;

		// Token: 0x040001AA RID: 426
		[XmlIgnore]
		private XElement SecurityCapabilities;

		// Token: 0x040001AB RID: 427
		[XmlIgnore]
		private XElement SecurityAuthorization;

		// Token: 0x040001AC RID: 428
		[XmlIgnore]
		private List<PkgObject> Components;

		// Token: 0x020000AC RID: 172
		[XmlRoot(Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00", ElementName = "Field")]
		public class MetadataField
		{
			// Token: 0x170000C1 RID: 193
			// (get) Token: 0x06000379 RID: 889 RVA: 0x0000B05F File Offset: 0x0000925F
			// (set) Token: 0x0600037A RID: 890 RVA: 0x0000B067 File Offset: 0x00009267
			[XmlAttribute("Name")]
			public string Key { get; set; }

			// Token: 0x170000C2 RID: 194
			// (get) Token: 0x0600037B RID: 891 RVA: 0x0000B070 File Offset: 0x00009270
			// (set) Token: 0x0600037C RID: 892 RVA: 0x0000B078 File Offset: 0x00009278
			[XmlText]
			public string Value { get; set; }
		}
	}
}
