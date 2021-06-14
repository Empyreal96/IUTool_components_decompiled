using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x02000013 RID: 19
	internal class XmlFileHandler
	{
		// Token: 0x06000067 RID: 103 RVA: 0x00004734 File Offset: 0x00002934
		public static XDocument LoadXmlDoc(ref List<XmlFile> files)
		{
			if (files == null || files.Count <= 0)
			{
				return null;
			}
			XmlFileHandler.xmlFiles = files;
			if (XmlFileHandler.mergeFileList != null)
			{
				XmlFileHandler.mergeFileList.Clear();
			}
			try
			{
				XmlFileHandler.GenerateMergeFileList();
				XmlFileHandler.MergeFiles();
				XDocument xdocument = XDocument.Load(Settings.MergeFilePath);
				TraceLogger.LogMessage(TraceLevel.Info, "Merged File Contents: ", true);
				TraceLogger.LogMessage(TraceLevel.Info, Environment.NewLine + xdocument.ToString(), true);
				files.Clear();
				files.AddRange(XmlFileHandler.mergeFileList);
				return xdocument;
			}
			catch (Exception ex)
			{
				TraceLogger.LogMessage(TraceLevel.Warn, "There was a failure in loading xml documents.", true);
				TraceLogger.LogMessage(TraceLevel.Info, ex.ToString(), true);
			}
			return null;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000047E8 File Offset: 0x000029E8
		private static void GenerateMergeFileList()
		{
			if (XmlFileHandler.mergeFileList == null)
			{
				XmlFileHandler.mergeFileList = new List<XmlFile>();
			}
			foreach (XmlFile xmlFile in XmlFileHandler.xmlFiles)
			{
				XmlFileHandler.AddIncludesToMergeFileList(xmlFile);
				if (xmlFile.Validate())
				{
					XmlFileHandler.mergeFileList.Add(xmlFile);
				}
				else
				{
					TraceLogger.LogMessage(TraceLevel.Warn, "Skipping merge of " + xmlFile.Filename, true);
				}
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004878 File Offset: 0x00002A78
		private static void AddIncludesToMergeFileList(XmlFile file)
		{
			try
			{
				foreach (XElement xelement in XDocument.Load(file.Filename).Elements().Descendants<XElement>())
				{
					if (xelement.Name.LocalName == "include")
					{
						XmlFile xmlFile = new XmlFile(xelement.Attribute("href").Value, file.Schema);
						TraceLogger.LogMessage(TraceLevel.Info, "FOUND INCLUDE: " + xmlFile.Filename, true);
						xmlFile.ExpandFilePath();
						if (File.Exists(xmlFile.Filename))
						{
							TraceLogger.LogMessage(TraceLevel.Info, string.Format("Adding include file from local filesystem: {0} to merge list.", xmlFile.Filename), true);
							XmlFileHandler.AddIncludesToMergeFileList(xmlFile);
							if (xmlFile.Validate())
							{
								XmlFileHandler.mergeFileList.Add(xmlFile);
							}
							else
							{
								TraceLogger.LogMessage(TraceLevel.Warn, "Skipping merge of " + xmlFile.Filename, true);
							}
						}
						else if (Uri.IsWellFormedUriString(xmlFile.Filename, UriKind.RelativeOrAbsolute))
						{
							TraceLogger.LogMessage(TraceLevel.Info, string.Format("Adding include file from remote uri: {0} to merge list.", xmlFile.Filename), true);
							XmlFileHandler.AddIncludesToMergeFileList(xmlFile);
							if (xmlFile.Validate())
							{
								XmlFileHandler.mergeFileList.Add(xmlFile);
							}
							else
							{
								TraceLogger.LogMessage(TraceLevel.Warn, "Skipping merge of " + xmlFile.Filename, true);
							}
						}
						else
						{
							TraceLogger.LogMessage(TraceLevel.Warn, string.Format("The include file {0} specified was not found. This file will be skipped during merge.", xmlFile.Filename), true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				TraceLogger.LogMessage(TraceLevel.Warn, string.Format("Error encountered when processing file {0}.", file.Filename), true);
				TraceLogger.LogMessage(TraceLevel.Info, ex.ToString(), true);
				if (ex.InnerException != null)
				{
					TraceLogger.LogMessage(TraceLevel.Info, ex.InnerException.ToString(), true);
				}
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004A60 File Offset: 0x00002C60
		private static void MergeFiles()
		{
			XmlTextReader reader = null;
			DataSet dataSet = new DataSet();
			dataSet.Locale = CultureInfo.InvariantCulture;
			DataSet dataSet2 = new DataSet();
			dataSet2.Locale = CultureInfo.InvariantCulture;
			try
			{
				foreach (XmlFile xmlFile in XmlFileHandler.mergeFileList)
				{
					TraceLogger.LogMessage(TraceLevel.Info, "Merging: " + xmlFile.Filename, true);
					reader = new XmlTextReader(xmlFile.Filename);
					Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(xmlFile.Schema);
					if (manifestResourceStream == null)
					{
						throw new SystemException("Failed to load the embedded schema file: " + xmlFile.Schema);
					}
					using (XmlTextReader xmlTextReader = new XmlTextReader(manifestResourceStream))
					{
						dataSet2.ReadXmlSchema(xmlTextReader);
					}
					dataSet2.ReadXml(reader);
					dataSet.Merge(dataSet2);
					dataSet2.Clear();
				}
				dataSet.WriteXml(Settings.MergeFilePath);
			}
			catch (Exception ex)
			{
				TraceLogger.LogMessage(TraceLevel.Error, "Exception: " + ex.ToString(), true);
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004B9C File Offset: 0x00002D9C
		public static bool ValidateSchema(XElement doc, string schemaFilename)
		{
			bool result;
			try
			{
				Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaFilename);
				if (manifestResourceStream == null)
				{
					throw new SystemException("Failed to load the embedded schema file: " + schemaFilename);
				}
				ValidationEventHandler validationEventHandler = new ValidationEventHandler(XmlFileHandler.ValidationEventHandler);
				XmlSchema schema = null;
				using (XmlTextReader xmlTextReader = new XmlTextReader(manifestResourceStream))
				{
					schema = XmlSchema.Read(xmlTextReader, validationEventHandler);
				}
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.Schemas.Add(schema);
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
				XmlReader reader = XmlReader.Create(doc.CreateReader(), xmlReaderSettings);
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(reader);
				xmlDocument.Validate(validationEventHandler);
				result = true;
			}
			catch (Exception ex)
			{
				TraceLogger.LogMessage(TraceLevel.Warn, "There are schema validation errors in:" + schemaFilename + Environment.NewLine + ex.ToString(), true);
				result = false;
			}
			return result;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004C84 File Offset: 0x00002E84
		private static void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			XmlSeverityType severity = e.Severity;
			if (severity == XmlSeverityType.Error)
			{
				Console.WriteLine("Error: {0}", e.Message);
				return;
			}
			if (severity != XmlSeverityType.Warning)
			{
				return;
			}
			Console.WriteLine("Warning: {0}", e.Message);
		}

		// Token: 0x04000051 RID: 81
		private static List<XmlFile> mergeFileList;

		// Token: 0x04000052 RID: 82
		private static List<XmlFile> xmlFiles;
	}
}
