using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x02000005 RID: 5
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "CompDBChunkMapping", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class CompDBChunkMapping
	{
		// Token: 0x06000027 RID: 39 RVA: 0x00003ED4 File Offset: 0x000020D4
		public CompDBChunkMapItem FindChunk(string path)
		{
			CompDBChunkMapItem result = null;
			foreach (string text in this._lookupTable.Keys)
			{
				if (path.StartsWith(text, StringComparison.OrdinalIgnoreCase))
				{
					result = this._lookupTable[text];
					break;
				}
			}
			return result;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00003F44 File Offset: 0x00002144
		public static CompDBChunkMapping ValidateAndLoad(string xmlFile, List<string> languages, IULogger logger)
		{
			CompDBChunkMapping compDBChunkMapping = new CompDBChunkMapping();
			string text = string.Empty;
			string compDBChunkMappingSchema = BuildPaths.CompDBChunkMappingSchema;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(compDBChunkMappingSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ImageCommonException("ImageCommon!CompDBChunkMapping::ValidateAndLoad: XSD resource was not found: " + compDBChunkMappingSchema);
			}
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text))
			{
				XsdValidator xsdValidator = new XsdValidator();
				try
				{
					xsdValidator.ValidateXsd(manifestResourceStream, xmlFile, logger);
				}
				catch (XsdValidatorException innerException)
				{
					throw new ImageCommonException("ImageCommon!CompDBChunkMapping::ValidateAndLoad: Unable to validate CompDB Chunk Mapping XSD for file '" + xmlFile + "'.", innerException);
				}
			}
			logger.LogInfo("ImageCommon: Successfully validated the CompDB Chunk Mappingt XML: {0}", new object[]
			{
				xmlFile
			});
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(CompDBChunkMapping));
			try
			{
				compDBChunkMapping = (CompDBChunkMapping)xmlSerializer.Deserialize(textReader);
			}
			catch (Exception innerException2)
			{
				throw new ImageCommonException("ImageCommon!CompDBChunkMapping::ValidateAndLoad: Unable to parse CompDB Chunk Mapping XML file '" + xmlFile + "'.", innerException2);
			}
			finally
			{
				textReader.Close();
			}
			compDBChunkMapping.Languages = languages;
			compDBChunkMapping.LoadLookUpTable();
			return compDBChunkMapping;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000040A0 File Offset: 0x000022A0
		private void LoadLookUpTable()
		{
			foreach (CompDBChunkMapItem compDBChunkMapItem in this.ChunkMappings)
			{
				if (compDBChunkMapItem.Path.ToUpper(CultureInfo.InvariantCulture).Contains(CompDBChunkMapping.c_LangVariable.ToUpper(CultureInfo.InvariantCulture)))
				{
					using (List<string>.Enumerator enumerator2 = this.Languages.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							string newValue = enumerator2.Current;
							CompDBChunkMapItem compDBChunkMapItem2 = new CompDBChunkMapItem(compDBChunkMapItem);
							compDBChunkMapItem2.ChunkName = compDBChunkMapItem2.ChunkName.Replace(CompDBChunkMapping.c_LangVariable, newValue, StringComparison.OrdinalIgnoreCase);
							compDBChunkMapItem2.Path = compDBChunkMapItem2.Path.Replace(CompDBChunkMapping.c_LangVariable, newValue, StringComparison.OrdinalIgnoreCase);
							this._lookupTable[compDBChunkMapItem2.Path] = compDBChunkMapItem2;
						}
						continue;
					}
				}
				this._lookupTable[compDBChunkMapItem.Path] = compDBChunkMapItem;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000041B8 File Offset: 0x000023B8
		public void WriteToFile(string xmlFile)
		{
			TextWriter textWriter = new StreamWriter(LongPathFile.OpenWrite(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(CompDBChunkMapping));
			try
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("CompDBChunkMapping!WriteToFile: Unable to write CompDB Chunk Mapping XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textWriter.Close();
			}
		}

		// Token: 0x04000020 RID: 32
		public static string c_LangVariable = "$(Lang)";

		// Token: 0x04000021 RID: 33
		[XmlArrayItem(ElementName = "Mapping", Type = typeof(CompDBChunkMapItem), IsNullable = false)]
		[XmlArray]
		public List<CompDBChunkMapItem> ChunkMappings = new List<CompDBChunkMapItem>();

		// Token: 0x04000022 RID: 34
		[XmlIgnore]
		public List<string> Languages;

		// Token: 0x04000023 RID: 35
		private Dictionary<string, CompDBChunkMapItem> _lookupTable = new Dictionary<string, CompDBChunkMapItem>(StringComparer.OrdinalIgnoreCase);
	}
}
