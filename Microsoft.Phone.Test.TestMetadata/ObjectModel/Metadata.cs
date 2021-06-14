using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Tools.IO;

namespace Microsoft.Phone.Test.TestMetadata.ObjectModel
{
	// Token: 0x02000017 RID: 23
	[XmlRoot(Namespace = "http://schemas.microsoft.com/WindowsPhone/2011/TestMetadata.xsd", IsNullable = false, ElementName = "Metadata")]
	[Serializable]
	public class Metadata
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00003691 File Offset: 0x00001891
		// (set) Token: 0x0600006D RID: 109 RVA: 0x00003699 File Offset: 0x00001899
		[XmlIgnore]
		public string FileName { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600006E RID: 110 RVA: 0x000036A4 File Offset: 0x000018A4
		// (set) Token: 0x0600006F RID: 111 RVA: 0x000036BC File Offset: 0x000018BC
		[XmlArrayItem(typeof(BinaryDependency), ElementName = "Binary")]
		[XmlArrayItem(typeof(RemoteFileDependency), ElementName = "RemoteFile")]
		[XmlArrayItem(typeof(PackageDependency), ElementName = "Package")]
		[XmlArrayItem(typeof(EnvironmentPathDependnecy), ElementName = "EnvironmentPath")]
		public HashSet<Dependency> Dependencies
		{
			get
			{
				return this._dependencies;
			}
			set
			{
				this._dependencies = value;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000070 RID: 112 RVA: 0x000036C6 File Offset: 0x000018C6
		// (set) Token: 0x06000071 RID: 113 RVA: 0x000036CE File Offset: 0x000018CE
		[XmlAttribute]
		public bool RequiresReflash { get; set; }

		// Token: 0x06000072 RID: 114 RVA: 0x000036D8 File Offset: 0x000018D8
		public void Save(string fileName)
		{
			Metadata.CleanUpEmptyLists(this, "Microsoft.Phone.Test.TestMetadata.ObjectModel");
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true
			};
			using (XmlWriter xmlWriter = XmlWriter.Create(fileName, settings))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(base.GetType());
				xmlSerializer.Serialize(xmlWriter, this);
			}
			this.FileName = fileName;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00003744 File Offset: 0x00001944
		public override string ToString()
		{
			string result;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(base.GetType());
				xmlSerializer.Serialize(stringWriter, this);
				result = stringWriter.ToString();
			}
			return result;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00003798 File Offset: 0x00001998
		public static Metadata Load(string xmlFile)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("Microsoft.Phone.Test.TestMetadata.Schema.testmetadata.xsd");
			bool flag = manifestResourceStream == null;
			if (flag)
			{
				throw new FileNotFoundException("Microsoft.Phone.Test.TestMetadata.Schema.testmetadata.xsd");
			}
			XmlSchema schema = XmlSchema.Read(manifestResourceStream, null);
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
			{
				ValidationType = ValidationType.Schema,
				ValidationFlags = (XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings),
				IgnoreComments = true,
				IgnoreWhitespace = true
			};
			xmlReaderSettings.Schemas.Add(schema);
			Metadata result;
			using (FileStream fileStream = LongPathFile.Open(xmlFile, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (XmlReader xmlReader = XmlReader.Create(fileStream, xmlReaderSettings))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(Metadata));
					Metadata metadata = (Metadata)xmlSerializer.Deserialize(xmlReader);
					metadata.FileName = xmlFile;
					result = metadata;
				}
			}
			return result;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00003888 File Offset: 0x00001A88
		private static bool CleanUpEmptyLists(object root, string objectNamespace)
		{
			bool flag = root == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Type type = root.GetType();
				bool flag2 = root is IList;
				if (flag2)
				{
					IList list = (IList)root;
					bool flag3 = list.Count == 0;
					if (flag3)
					{
						result = true;
					}
					else
					{
						foreach (object root2 in list)
						{
							Metadata.CleanUpEmptyLists(root2, objectNamespace);
						}
						result = false;
					}
				}
				else
				{
					bool flag4 = root is ISet<Dependency>;
					if (flag4)
					{
						ISet<Dependency> set = (ISet<Dependency>)root;
						bool flag5 = set.Count == 0;
						if (flag5)
						{
							result = true;
						}
						else
						{
							foreach (Dependency root3 in set)
							{
								Metadata.CleanUpEmptyLists(root3, objectNamespace);
							}
							result = false;
						}
					}
					else
					{
						bool flag6 = type.Namespace != objectNamespace;
						if (flag6)
						{
							result = false;
						}
						else
						{
							foreach (PropertyInfo propertyInfo in type.GetProperties())
							{
								object value = propertyInfo.GetValue(root, null);
								bool flag7 = Metadata.CleanUpEmptyLists(value, objectNamespace);
								if (flag7)
								{
									propertyInfo.SetValue(root, null, null);
								}
							}
							result = false;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0400008D RID: 141
		private HashSet<Dependency> _dependencies = new HashSet<Dependency>();
	}
}
